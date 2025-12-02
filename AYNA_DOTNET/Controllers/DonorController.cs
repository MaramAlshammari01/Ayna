using Ayna.Controllers;
using Ayna.Data;
using Ayna.Support;
using Ayna.ViewModels.DonorVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

[Authorize(Policy = "DonorOnly")]
[Route("Donor")]
public class DonorController : BaseController
{
    public DonorController(AynaDbContext context, ILogger<DonorController> logger)
        : base(context, logger)
    {
    }

    #region Dashboard

    [HttpGet("Dashboard")]
    [HttpGet("")]
    public async Task<IActionResult> Dashboard()
    {
        try
        {
            if (!CurrentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var donor = await _context.Donors
                .FirstOrDefaultAsync(d => d.UserId == CurrentUserId.Value);

            if (donor == null)
            {
                SetErrorMessage("لم يتم العثور على بيانات المتبرع");
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new DonorDashboardViewModel
            {
                DonorInfo = new DonorInfoDto
                {
                    DonorId = donor.DonorId,
                    DonorFirstName = donor.DonorFirstName,
                    DonorLastName = donor.DonorLastName
                },

                // Statistics
                TotalDonations = await _context.Orders
                    .CountAsync(o => o.DonorId == donor.DonorId),

                TotalAmount = await _context.Orders
                    .Where(o => o.DonorId == donor.DonorId)
                    .SumAsync(o => o.OrdPrice ?? 0),

                ActivePayments = await _context.Payments
                    .CountAsync(p => p.PayStatus == Constants.PAYMENT_STATUS_COMPLETED),

                SupportedCharities = await _context.Orders
                    .Where(o => o.DonorId == donor.DonorId)
                    .Select(o => o.CharId)
                    .Distinct()
                    .CountAsync(),

                // Recent donations
                RecentDonations = await _context.Orders
                    .Where(o => o.DonorId == donor.DonorId)
                    .Include(o => o.Char)
                    .Include(o => o.Carts)
                        .ThenInclude(c => c.Bas)
                    .OrderByDescending(o => o.OrdDate)
                    .Take(5)
                    .Select(o => new RecentOrderDto
                    {
                        OrdId = o.OrdId,
                        CharityName = o.Char != null ? o.Char.CharName : "تبرع عام",
                        OrdPrice = o.OrdPrice,
                        OrdStatus = o.OrdStatus,
                        OrdDate = o.OrdDate,
                        ItemsCount = o.Carts.Count
                    })
                    .ToListAsync(),

                // Featured baskets
                FeaturedBaskets = await _context.Baskets
                    .Where(b => b.BasQty > 0)
                    .Include(b => b.BasketCrops)
                        .ThenInclude(bc => bc.Cro)
                    .Include(b => b.Far)
                    .OrderByDescending(b => b.AddedAt)
                    .Take(6)
                    .Select(b => new FeaturedBasketDto
                    {
                        BasId = b.BasId,
                        BasContent = b.BasContent,
                        BasPrice = b.BasPrice,
                        BasQty = b.BasQty,
                        FarmerName = b.Far.FarFirstName + " " + b.Far.FarLastName,
                        CropsCount = b.BasketCrops.Count
                    })
                    .ToListAsync(),

                CartCount = GetCartCount()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            LogError(ex, "Error loading donor dashboard");
            SetErrorMessage("حدث خطأ أثناء تحميل لوحة التحكم");
            return View(new DonorDashboardViewModel());
        }
    }

    #endregion

    #region Profile

    [HttpGet("Profile")]
    public async Task<IActionResult> Profile()
    {
        try
        {
            if (!CurrentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var donor = await _context.Donors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == CurrentUserId.Value);

            if (donor == null)
            {
                SetErrorMessage("لم يتم العثور على بيانات المتبرع");
                return RedirectToAction(nameof(Dashboard));
            }

            var viewModel = new DonorProfileViewModel
            {
                DonorId = donor.DonorId,
                DonorFirstName = donor.DonorFirstName,
                DonorLastName = donor.DonorLastName,
                UserEmail = donor.User.UserEmail,
                UserPhone = donor.User.UserPhone,
                AddedAt = donor.AddedAt
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            LogError(ex, "Error loading donor profile");
            SetErrorMessage("حدث خطأ أثناء تحميل الملف الشخصي");
            return RedirectToAction(nameof(Dashboard));
        }
    }

    [HttpPost("Profile")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(UpdateDonorProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            SetErrorMessage("يرجى التحقق من البيانات المدخلة");
            return View("Profile", model);
        }

        try
        {
            if (!CurrentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var donor = await _context.Donors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == CurrentUserId.Value);

            if (donor == null)
            {
                SetErrorMessage("لم يتم العثور على بيانات المتبرع");
                return RedirectToAction(nameof(Dashboard));
            }

            // Check email uniqueness
            var emailExists = await _context.Users
                .AnyAsync(u => u.UserEmail == model.UserEmail && u.UserId != CurrentUserId.Value);

            if (emailExists)
            {
                ModelState.AddModelError(nameof(model.UserEmail), "البريد الإلكتروني مستخدم من قبل");
                return View("Profile", model);
            }

            // Update donor info
            donor.DonorFirstName = model.DonorFirstName;
            donor.DonorLastName = model.DonorLastName;

            // Update user info
            donor.User.UserEmail = model.UserEmail;
            donor.User.UserPhone = model.UserPhone;

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                donor.User.UserPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }

            await _context.SaveChangesAsync();

            SetSuccessMessage("تم تحديث الملف الشخصي بنجاح" +
                (!string.IsNullOrWhiteSpace(model.Password) ? " وتغيير كلمة المرور" : ""));
            return RedirectToAction(nameof(Profile));
        }
        catch (Exception ex)
        {
            LogError(ex, "Error updating donor profile");
            SetErrorMessage("حدث خطأ أثناء تحديث الملف الشخصي");
            return View("Profile", model);
        }
    }

    #endregion

    #region Donations

    [HttpGet("Donations")]
    public async Task<IActionResult> Donations()
    {
        try
        {
            if (!CurrentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var donor = await _context.Donors
                .FirstOrDefaultAsync(d => d.UserId == CurrentUserId.Value);

            if (donor == null)
            {
                SetErrorMessage("لم يتم العثور على بيانات المتبرع");
                return RedirectToAction(nameof(Dashboard));
            }

            var donations = await _context.Orders
                .Where(o => o.DonorId == donor.DonorId)
                .Include(o => o.Char)
                .Include(o => o.Carts)
                    .ThenInclude(c => c.Bas)
                        .ThenInclude(b => b.BasketCrops)
                            .ThenInclude(bc => bc.Cro)
                .Include(o => o.Payments)
                .OrderByDescending(o => o.OrdDate)
                .Select(o => new DonationDto
                {
                    OrdId = o.OrdId,
                    CharityName = o.Char != null ? o.Char.CharName : "تبرع عام",
                    OrdPrice = o.OrdPrice,
                    OrdStatus = o.OrdStatus,
                    OrdDate = o.OrdDate,
                    OrdTime = o.OrdTime,
                    Items = o.Carts.Select(c => new DonationItemDto
                    {
                        BasketName = c.Bas.BasContent,
                        Quantity = c.CartQty,
                        Price = c.CartPrice,
                        Crops = c.Bas.BasketCrops.Select(bc => new CropInfoDto
                        {
                            CropName = bc.Cro.CroName,
                            Quantity = bc.BcQty
                        }).ToList()
                    }).ToList(),
                    PaymentInfo = o.Payments.Select(p => new PaymentInfoDto
                    {
                        PayMethod = p.PayMethod,
                        PayStatus = p.PayStatus,
                        TransactionId = p.TransactionId
                    }).FirstOrDefault()
                })
                .ToListAsync();

            var viewModel = new DonationsViewModel
            {
                Donations = donations,
                DonorInfo = new DonorInfoDto
                {
                    DonorId = donor.DonorId,
                    DonorFirstName = donor.DonorFirstName,
                    DonorLastName = donor.DonorLastName
                }
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            LogError(ex, "Error loading donations");
            SetErrorMessage("حدث خطأ أثناء تحميل التبرعات");
            return View(new DonationsViewModel());
        }
    }

    #endregion

    #region Payments

    [HttpGet("Payments")]
    public async Task<IActionResult> Payments()
    {
        try
        {
            if (!CurrentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var donor = await _context.Donors
                .FirstOrDefaultAsync(d => d.UserId == CurrentUserId.Value);

            if (donor == null)
            {
                SetErrorMessage("لم يتم العثور على بيانات المتبرع");
                return RedirectToAction(nameof(Dashboard));
            }

            var payments = await _context.Payments
                .Include(p => p.Ord)
                    .ThenInclude(o => o.Char)
                .Where(p => p.Ord.DonorId == donor.DonorId)
                .OrderByDescending(p => p.PayDate)
                .Select(p => new PaymentDto
                {
                    PayId = p.PayId,
                    CharityName = p.Ord.Char != null ? p.Ord.Char.CharName : "تبرع عام",
                    PayMethod = p.PayMethod,
                    PayAmount = p.PayAmount,
                    PayStatus = p.PayStatus,
                    PayDate = p.PayDate,
                    PayTime = p.PayTime,
                    TransactionId = p.TransactionId,
                    OrdId = p.OrdId
                })
                .ToListAsync();

            var viewModel = new PaymentsViewModel
            {
                Payments = payments,
                DonorInfo = new DonorInfoDto
                {
                    DonorId = donor.DonorId,
                    DonorFirstName = donor.DonorFirstName,
                    DonorLastName = donor.DonorLastName
                }
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            LogError(ex, "Error loading payments");
            SetErrorMessage("حدث خطأ أثناء تحميل المدفوعات");
            return View(new PaymentsViewModel());
        }
    }

    #endregion

    #region Helper Methods

    private int GetCartCount()
    {
        var cartJson = HttpContext.Session.GetString("donor_cart");
        if (string.IsNullOrEmpty(cartJson))
            return 0;

        var cart = JsonSerializer.Deserialize<Dictionary<int, CartItemDto>>(cartJson);
        return cart?.Count ?? 0;
    }

    #endregion
}