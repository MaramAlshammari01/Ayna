using Ayna.Data;
using Ayna.Models;
using Ayna.ViewModels.FarmerVMs;
using Ayna.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ayna.Controllers.Farmer
{
    [Authorize(Policy = "FarmerOnly")]
    [Route("Farmer/Baskets")]
    public class BasketController : BaseController
    {
        public BasketController(AynaDbContext context, ILogger<BasketController> logger)
            : base(context, logger)
        {
        }

        // GET: Farmer/Baskets
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var farmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.UserId == userId);

                if (farmer == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات المزارع");
                    return RedirectToAction("Dashboard", "Farmer");
                }

                var baskets = await _context.Baskets
                    .Where(b => b.FarId == farmer.FarId)
                    .Include(b => b.BasketCrops)
                        .ThenInclude(bc => bc.Cro)
                    .OrderByDescending(b => b.AddedAt)
                    .ToListAsync();

                var crops = await _context.Crops
                    .Where(c => c.FarId == farmer.FarId && c.CroQuantity > 0)
                    .ToListAsync();

                var viewModel = new BasketIndexViewModel
                {
                    Baskets = baskets,
                    Farmer = farmer,
                    AvailableCrops = crops
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading baskets");
                SetErrorMessage("حدث خطأ أثناء تحميل السلال");
                return RedirectToAction("Dashboard", "Farmer");
            }
        }

        // POST: Farmer/Baskets/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBasketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("يرجى التحقق من البيانات المدخلة");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var farmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.UserId == userId);

                if (farmer == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات المزارع");
                    return RedirectToAction(nameof(Index));
                }

                // Validate crops availability
                foreach (var cropItem in model.Crops)
                {
                    var crop = await _context.Crops.FindAsync(cropItem.CropId);
                    if (crop == null || crop.FarId != farmer.FarId)
                    {
                        SetErrorMessage("أحد المحاصيل المحددة غير صالح");
                        return RedirectToAction(nameof(Index));
                    }

                    if (crop.CroQuantity < cropItem.Quantity)
                    {
                        SetErrorMessage($"الكمية المطلوبة من {crop.CroName} غير متاحة");
                        return RedirectToAction(nameof(Index));
                    }
                }

                // Create basket
                var basket = new Basket
                {
                    BasContent = model.BasContent,
                    BasPrice = model.BasPrice,
                    BasQty = model.BasQty,
                    FarId = farmer.FarId,
                    AddedAt = DateTime.Now
                };

                _context.Baskets.Add(basket);
                await _context.SaveChangesAsync();

                // Add crops to basket
                foreach (var cropItem in model.Crops)
                {
                    var basketCrop = new BasketCrop
                    {
                        BasId = basket.BasId,
                        CroId = cropItem.CropId,
                        BcQty = cropItem.Quantity,
                        AddedAt = DateTime.Now
                    };

                    _context.BasketCrops.Add(basketCrop);
                }

                await _context.SaveChangesAsync();

                SetSuccessMessage("تم إنشاء السلة بنجاح");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                LogError(ex, "Error creating basket");
                SetErrorMessage("حدث خطأ أثناء إنشاء السلة");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Farmer/Baskets/Update/{id}
        [HttpPost("Update/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, UpdateBasketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("يرجى التحقق من البيانات المدخلة");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var farmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.UserId == userId);

                if (farmer == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات المزارع");
                    return RedirectToAction(nameof(Index));
                }

                var basket = await _context.Baskets
                    .FirstOrDefaultAsync(b => b.BasId == id && b.FarId == farmer.FarId);

                if (basket == null)
                {
                    SetErrorMessage("لم يتم العثور على السلة");
                    return RedirectToAction(nameof(Index));
                }

                // Update basket
                basket.BasContent = model.BasContent;
                basket.BasPrice = model.BasPrice;
                basket.BasQty = model.BasQty;

                _context.Baskets.Update(basket);
                await _context.SaveChangesAsync();

                SetSuccessMessage("تم تحديث السلة بنجاح");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                LogError(ex, "Error updating basket");
                SetErrorMessage("حدث خطأ أثناء تحديث السلة");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Farmer/Baskets/Delete/{id}
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var farmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.UserId == userId);

                if (farmer == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات المزارع");
                    return RedirectToAction(nameof(Index));
                }

                var basket = await _context.Baskets
                    .Include(b => b.BasketCrops)
                    .FirstOrDefaultAsync(b => b.BasId == id && b.FarId == farmer.FarId);

                if (basket == null)
                {
                    SetErrorMessage("لم يتم العثور على السلة");
                    return RedirectToAction(nameof(Index));
                }

                // Check if basket has associated carts
                var hasOrders = await _context.Carts.AnyAsync(c => c.BasId == basket.BasId);
                if (hasOrders)
                {
                    SetErrorMessage("لا يمكن حذف السلة لأنها مرتبطة بطلبات");
                    return RedirectToAction(nameof(Index));
                }

                // Delete basket crops first
                _context.BasketCrops.RemoveRange(basket.BasketCrops);

                // Delete basket
                _context.Baskets.Remove(basket);
                await _context.SaveChangesAsync();

                SetSuccessMessage("تم حذف السلة بنجاح");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                LogError(ex, "Error deleting basket");
                SetErrorMessage(GetDatabaseErrorMessage(ex));
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Farmer/Baskets/Details/{id}
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var farmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.UserId == userId);

                if (farmer == null)
                {
                    return JsonError("لم يتم العثور على بيانات المزارع");
                }

                var basket = await _context.Baskets
                    .Include(b => b.BasketCrops)
                        .ThenInclude(bc => bc.Cro)
                    .FirstOrDefaultAsync(b => b.BasId == id && b.FarId == farmer.FarId);

                if (basket == null)
                {
                    return JsonError("لم يتم العثور على السلة");
                }

                return JsonSuccess("تم تحميل بيانات السلة بنجاح", new
                {
                    basket = new
                    {
                        id = basket.BasId,
                        content = basket.BasContent,
                        price = basket.BasPrice,
                        quantity = basket.BasQty,
                        addedAt = basket.AddedAt,
                        crops = basket.BasketCrops.Select(bc => new
                        {
                            cropId = bc.CroId,
                            cropName = bc.Cro.CroName,
                            quantity = bc.BcQty,
                            unit = bc.Cro.CroUnit
                        })
                    }
                });
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading basket details");
                return JsonError("حدث خطأ أثناء تحميل بيانات السلة");
            }
        }
    }
}