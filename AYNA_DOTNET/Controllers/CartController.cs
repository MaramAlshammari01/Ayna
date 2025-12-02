using Ayna.Data;
using Ayna.Models;
using Ayna.Support;
using Ayna.ViewModels.DonorVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ayna.Controllers.Donor
{
    [Authorize(Policy = "DonorOnly")]
    [Route("Donor/Cart")]
    public class CartController : BaseController
    {
        public CartController(AynaDbContext context, ILogger<CartController> logger)
            : base(context, logger)
        {
        }

        [HttpPost("Add/{basketId}")]
        public async Task<IActionResult> AddToCart(int basketId)
        {
            try
            {
                var basket = await _context.Baskets
                    .Include(b => b.BasketCrops)
                        .ThenInclude(bc => bc.Cro)
                    .Include(b => b.Far)
                    .FirstOrDefaultAsync(b => b.BasId == basketId);

                if (basket == null)
                    return JsonError("السلة غير موجودة");

                // Get cart from session
                var cart = GetCartFromSession();

                // Check if already in cart
                if (cart.ContainsKey(basketId))
                    return JsonError("هذه السلة موجودة بالفعل في سلة التسوق");

                // Check basket quantity
                if (basket.BasQty <= 0)
                    return JsonError("عذراً، هذه السلة غير متاحة حالياً");

                // Add to cart
                var cartItem = new CartItemDto
                {
                    BasketId = basket.BasId,
                    Name = basket.BasContent,
                    Price = basket.BasPrice ?? 0,
                    Quantity = 1,
                    FarmerName = basket.Far.FarFirstName + " " + basket.Far.FarLastName,
                    Crops = basket.BasketCrops.Select(bc =>
                        $"{bc.Cro.CroName} ({bc.BcQty})").ToList()
                };

                cart[basketId] = cartItem;
                SaveCart(cart);

                return JsonSuccess("تمت إضافة السلة إلى سلة التسوق", new { cart_count = cart.Count });
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error adding basket {basketId} to cart");
                return JsonError("حدث خطأ أثناء إضافة السلة");
            }
        }

        [HttpPost("Remove/{basketId}")]
        public IActionResult RemoveFromCart(int basketId)
        {
            try
            {
                var cart = GetCartFromSession();

                if (cart.ContainsKey(basketId))
                {
                    cart.Remove(basketId);
                    SaveCart(cart);
                    return JsonSuccess("تمت إزالة السلة من سلة التسوق", new { cart_count = cart.Count });
                }

                return JsonError("السلة غير موجودة في سلة التسوق");
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error removing basket {basketId} from cart");
                return JsonError("حدث خطأ أثناء إزالة السلة");
            }
        }

        [HttpGet("")]
        public IActionResult GetCartData()
        {
            try
            {
                var cart = GetCartFromSession();
                var total = cart.Values.Sum(item => item.Price * item.Quantity);

                return JsonSuccess("تم تحميل سلة التسوق بنجاح", new
                {
                    cart = cart.Values.ToList(),
                    total = total,
                    count = cart.Count
                });
            }
            catch (Exception ex)
            {
                LogError(ex, "Error getting cart");
                return JsonError("حدث خطأ أثناء تحميل سلة التسوق");
            }
        }

        [HttpPost("UpdateQuantity/{basketId}")]
        public async Task<IActionResult> UpdateCartQuantity(int basketId, [FromBody] UpdateQuantityViewModel model)
        {
            if (!ModelState.IsValid)
                return JsonError("الكمية غير صالحة");

            try
            {
                var cart = GetCartFromSession();

                if (!cart.ContainsKey(basketId))
                    return JsonError("السلة غير موجودة في سلة التسوق");

                var basket = await _context.Baskets.FindAsync(basketId);
                if (basket == null)
                    return JsonError("السلة غير موجودة");

                if (basket.BasQty < model.Quantity)
                    return JsonError("الكمية المطلوبة غير متاحة");

                cart[basketId].Quantity = model.Quantity;
                SaveCart(cart);

                return JsonSuccess("تم تحديث الكمية", new { cart_count = cart.Count });
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error updating quantity for basket {basketId}");
                return JsonError("حدث خطأ أثناء تحديث الكمية");
            }
        }

        [HttpPost("Clear")]
        public IActionResult ClearCart()
        {
            try
            {
                HttpContext.Session.Remove("donor_cart");
                return JsonSuccess("تم تفريغ سلة التسوق", new { cart_count = 0 });
            }
            catch (Exception ex)
            {
                LogError(ex, "Error clearing cart");
                return JsonError("حدث خطأ أثناء تفريغ سلة التسوق");
            }
        }

        [HttpPost("Checkout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout([FromBody] CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
                return JsonError("يرجى التحقق من البيانات المدخلة");

            try
            {
                if (!CurrentUserId.HasValue)
                    return JsonError("يجب تسجيل الدخول أولاً");

                var donor = await _context.Donors
                    .FirstOrDefaultAsync(d => d.UserId == CurrentUserId.Value);

                if (donor == null)
                    return JsonError("لم يتم العثور على بيانات المتبرع");

                var cart = GetCartFromSession();
                if (cart.Count == 0)
                    return JsonError("سلة التسوق فارغة");

                // Calculate total
                var totalPrice = cart.Values.Sum(item => item.Price * item.Quantity);

                // Create order
                var order = new Order
                {
                    OrdDate = DateOnly.FromDateTime(DateTime.Now),
                    OrdTime = TimeOnly.FromDateTime(DateTime.Now),
                    OrdPrice = totalPrice,
                    OrdStatus = Constants.ORDER_STATUS_PENDING,
                    DonorId = donor.DonorId,
                    CharId = model.CharId,
                    AddedAt = DateTime.Now
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create cart items
                foreach (var item in cart.Values)
                {
                    var basket = await _context.Baskets.FindAsync(item.BasketId);

                    if (basket == null || basket.BasQty < item.Quantity)
                    {
                        // Rollback
                        _context.Carts.RemoveRange(_context.Carts.Where(c => c.OrdId == order.OrdId));
                        _context.Orders.Remove(order);
                        await _context.SaveChangesAsync();

                        return JsonError($"عذراً، السلة \"{item.Name}\" لم تعد متاحة بالكمية المطلوبة");
                    }

                    var cartItem = new Cart
                    {
                        CartQty = item.Quantity,
                        CartPrice = item.Price,
                        OrdId = order.OrdId,
                        BasId = item.BasketId,
                        AddedAt = DateTime.Now
                    };

                    _context.Carts.Add(cartItem);

                    // Decrease basket quantity
                    basket.BasQty -= item.Quantity;
                }

                // Create payment
                var payment = new Payment
                {
                    PayMethod = model.PayMethod,
                    PayDate = DateOnly.FromDateTime(DateTime.Now),
                    PayTime = TimeOnly.FromDateTime(DateTime.Now),
                    PayStatus = Constants.PAYMENT_STATUS_COMPLETED,
                    PayAmount = totalPrice,
                    TransactionId = $"TXN{DateTimeOffset.Now.ToUnixTimeSeconds()}{order.OrdId}",
                    OrdId = order.OrdId,
                    AddedAt = DateTime.Now
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Clear cart
                HttpContext.Session.Remove("donor_cart");

                return JsonSuccess("تم تأكيد تبرعك بنجاح! شكراً لك على مساهمتك.", new
                {
                    order_id = order.OrdId,
                    redirect_url = Url.Action("Donations", "Donor")
                });
            }
            catch (Exception ex)
            {
                LogError(ex, "Error processing checkout");
                return JsonError($"حدث خطأ أثناء تأكيد التبرع: {ex.Message}");
            }
        }

        [HttpGet("CheckoutData")]
        public async Task<IActionResult> GetCheckoutData()
        {
            try
            {
                var charities = await _context.Charities
                    .Select(c => new CharityDto
                    {
                        CharId = c.CharId,
                        CharName = c.CharName
                    })
                    .ToListAsync();

                var cart = GetCartFromSession();
                var total = cart.Values.Sum(item => item.Price * item.Quantity);

                return JsonSuccess("تم تحميل بيانات الدفع بنجاح", new
                {
                    charities = charities,
                    cart_total = total,
                    cart_count = cart.Count
                });
            }
            catch (Exception ex)
            {
                LogError(ex, "Error getting checkout data");
                return JsonError("حدث خطأ أثناء تحميل بيانات الدفع");
            }
        }

        #region Helper Methods

        private Dictionary<int, CartItemDto> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("donor_cart");
            if (string.IsNullOrEmpty(cartJson))
                return new Dictionary<int, CartItemDto>();

            return JsonSerializer.Deserialize<Dictionary<int, CartItemDto>>(cartJson)
                ?? new Dictionary<int, CartItemDto>();
        }

        private void SaveCart(Dictionary<int, CartItemDto> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("donor_cart", cartJson);
        }

        #endregion
    }
}
