using Ayna.Data;
using Ayna.Models;
using Ayna.ViewModels.FarmerVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ayna.Controllers.Farmer
{
    [Authorize(Policy = "FarmerOnly")]
    [Route("Farmer")]
    public class FarmerController : BaseController
    {
        public FarmerController(AynaDbContext context, ILogger<FarmerController> logger)
            : base(context, logger)
        {
        }

        // GET: Farmer/Dashboard
        [HttpGet("")]
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var farmer = await _context.Farmers
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(f => f.UserId == userId);

                if (farmer == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات المزارع");
                    return RedirectToAction("Login", "Account");
                }

                // Calculate statistics
                var totalCrops = await _context.Crops
                    .Where(c => c.FarId == farmer.FarId)
                    .CountAsync();

                var activeBaskets = await _context.Baskets
                    .Where(b => b.FarId == farmer.FarId && b.BasQty > 0)
                    .CountAsync();

                var pendingDonations = await _context.Donations
                    .Where(d => d.FarId == farmer.FarId && d.DonStatus == "Pending")
                    .CountAsync();

                var completedOrders = await _context.Carts
                    .Include(c => c.Bas)
                    .Include(c => c.Ord)
                    .Where(c => c.Bas.FarId == farmer.FarId && c.Ord.OrdStatus == "Completed")
                    .Select(c => c.OrdId)
                    .Distinct()
                    .CountAsync();

                // Get recent crops
                var recentCrops = await _context.Crops
                    .Where(c => c.FarId == farmer.FarId)
                    .OrderByDescending(c => c.AddedAt)
                    .Take(5)
                    .ToListAsync();

                // Get recent donations
                var recentDonations = await _context.Donations
                    .Where(d => d.FarId == farmer.FarId)
                    .Include(d => d.DonationCrops)
                        .ThenInclude(dc => dc.Cro)
                    .OrderByDescending(d => d.AddedAt)
                    .Take(5)
                    .ToListAsync();

                var viewModel = new FarmerDashboardViewModel
                {
                    Farmer = farmer,
                    TotalCrops = totalCrops,
                    ActiveBaskets = activeBaskets,
                    PendingDonations = pendingDonations,
                    CompletedOrders = completedOrders,
                    RecentCrops = recentCrops,
                    RecentDonations = recentDonations
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading farmer dashboard");
                SetErrorMessage("حدث خطأ أثناء تحميل لوحة التحكم");
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Farmer/Profile
        [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var farmer = await _context.Farmers
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(f => f.UserId == userId);

                if (farmer == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات المزارع");
                    return RedirectToAction(nameof(Dashboard));
                }

                var viewModel = new UpdateFarmerProfileViewModel
                {
                    FarFirstName = farmer.FarFirstName,
                    FarLastName = farmer.FarLastName,
                    FarLocation = farmer.FarLocation,
                    UserEmail = farmer.User.UserEmail,
                    UserPhone = farmer.User.UserPhone
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading farmer profile");
                SetErrorMessage("حدث خطأ أثناء تحميل الملف الشخصي");
                return RedirectToAction(nameof(Dashboard));
            }
        }

        // POST: Farmer/Profile
        [HttpPost("Profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateFarmerProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = GetModelStateErrors();
                SetErrorMessage(string.Join(", ", errors));
                return View("Profile", new FarmerProfileViewModel
                {
                    FarFirstName = model.FarFirstName,
                    FarLastName = model.FarLastName,
                    FarLocation = model.FarLocation,
                    UserEmail = model.UserEmail,
                    UserPhone = model.UserPhone
                });
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var farmer = await _context.Farmers
                    .Include(f => f.User)
                    .FirstOrDefaultAsync(f => f.UserId == userId);

                if (farmer == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات المزارع");
                    return RedirectToAction(nameof(Dashboard));
                }

                // Check if email is already taken by another user
                var emailExists = await _context.Users
                    .AnyAsync(u => u.UserEmail == model.UserEmail && u.UserId != userId);

                if (emailExists)
                {
                    SetErrorMessage("البريد الإلكتروني مستخدم بالفعل");
                    return View("Profile", new FarmerProfileViewModel
                    {
                        FarFirstName = model.FarFirstName,
                        FarLastName = model.FarLastName,
                        FarLocation = model.FarLocation,
                        UserEmail = model.UserEmail,
                        UserPhone = model.UserPhone
                    });
                }

                // Update farmer information
                farmer.FarFirstName = model.FarFirstName;
                farmer.FarLastName = model.FarLastName;
                farmer.FarLocation = model.FarLocation;

                // Update user information
                farmer.User.UserEmail = model.UserEmail;
                farmer.User.UserPhone = model.UserPhone;

                // Update password if provided
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    farmer.User.UserPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                }

                _context.Farmers.Update(farmer);
                await _context.SaveChangesAsync();

                var successMessage = "تم تحديث الملف الشخصي بنجاح";
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    successMessage += " وتغيير كلمة المرور";
                }

                SetSuccessMessage(successMessage);
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                LogError(ex, "Error updating farmer profile");
                SetErrorMessage("حدث خطأ أثناء تحديث الملف الشخصي");
                return View("Profile", new FarmerProfileViewModel
                {
                    FarFirstName = model.FarFirstName,
                    FarLastName = model.FarLastName,
                    FarLocation = model.FarLocation,
                    UserEmail = model.UserEmail,
                    UserPhone = model.UserPhone
                });
            }
        }

        // GET: Farmer/Statistics
        [HttpGet("Statistics")]
        public async Task<IActionResult> GetStatistics()
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

                // Calculate various statistics
                var cropStats = await _context.Crops
                    .Where(c => c.FarId == farmer.FarId)
                    .GroupBy(c => c.CroType)
                    .Select(g => new
                    {
                        type = g.Key,
                        count = g.Count(),
                        totalQuantity = g.Sum(c => c.CroQuantity)
                    })
                    .ToListAsync();

                var basketSales = await _context.Carts
                    .Include(c => c.Bas)
                    .Where(c => c.Bas.FarId == farmer.FarId)
                       //.GroupBy(c => c.Ord.OrdDate.HasValue ? c.Ord.OrdDate.Value.Month : 0)
                    .GroupBy(c => c.Ord.OrdDate.Month)
                    .Select(g => new
                    {
                        month = g.Key,
                        totalSales = g.Sum(c => c.CartPrice * c.CartQty),
                        orderCount = g.Select(c => c.OrdId).Distinct().Count()
                    })
                    .ToListAsync();

                var donationStats = await _context.Donations
                    .Where(d => d.FarId == farmer.FarId)
                    .GroupBy(d => d.DonStatus)
                    .Select(g => new
                    {
                        status = g.Key,
                        count = g.Count()
                    })
                    .ToListAsync();

                return JsonSuccess("تم تحميل الإحصائيات بنجاح", new
                {
                    cropStats,
                    basketSales,
                    donationStats
                });
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading farmer statistics");
                return JsonError("حدث خطأ أثناء تحميل الإحصائيات");
            }
        }
    }
}