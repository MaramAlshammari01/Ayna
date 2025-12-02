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
    [Route("Farmer/Reports")]
    public class ReportController : BaseController
    {
        public ReportController(AynaDbContext context, ILogger<ReportController> logger)
            : base(context, logger)
        {
        }

        // GET: Farmer/Reports/CropInventory
        [HttpGet("CropInventory")]
        public async Task<IActionResult> CropInventoryReport(DateTime? startDate, DateTime? endDate)
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

                // Default date range: last 30 days
                var start = startDate ?? DateTime.Now.AddDays(-30);
                var end = endDate ?? DateTime.Now;

                // Get crops with expiration analysis
                var crops = await _context.Crops
                    .Where(c => c.FarId == farmer.FarId)
                    .Include(c => c.BasketCrops)
                        .ThenInclude(bc => bc.Bas)
                    .OrderBy(c => c.ExpiredAt)
                    .ToListAsync();

                var cropReports = crops.Select(crop => new CropInventoryItem
                {
                    CropId = crop.CroId,
                    CropName = crop.CroName,
                    CropType = crop.CroType,
                    Quantity = crop.CroQuantity ?? 0,
                    Unit = crop.CroUnit,
                    Weight = crop.CroWeight ?? 0,
                    ExpiredAt = crop.ExpiredAt ?? DateTime.Now,
                    AddedAt = crop.AddedAt,
                    DaysUntilExpiry = crop.ExpiredAt.HasValue ? (int)(crop.ExpiredAt.Value - DateTime.Now).TotalDays : 0,
                    ExpiryStatus = crop.ExpiredAt.HasValue ? GetExpiryStatus((int)(crop.ExpiredAt.Value - DateTime.Now).TotalDays) : "unknown",
                    UsedInBaskets = crop.BasketCrops.Sum(bc => bc.BcQty),
                    RemainingQuantity = (crop.CroQuantity ?? 0) - crop.BasketCrops.Sum(bc => bc.BcQty)
                }).ToList();

                var viewModel = new CropInventoryReportViewModel
                {
                    Farmer = farmer,
                    Crops = cropReports,
                    StartDate = start,
                    EndDate = end,
                    TotalCrops = cropReports.Count,
                    ExpiringSoon = cropReports.Count(c => c.ExpiryStatus == "expiring_soon"),
                    Expired = cropReports.Count(c => c.ExpiryStatus == "expired"),
                    TotalQuantity = cropReports.Sum(c => c.Quantity),
                    TotalValue = cropReports.Sum(c => c.Quantity * c.Weight * 10) // Mock value calculation
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading crop inventory report");
                SetErrorMessage("حدث خطأ أثناء تحميل تقرير جرد المحاصيل");
                return RedirectToAction("Dashboard", "Farmer");
            }
        }

        // GET: Farmer/Reports/Performance
        [HttpGet("Performance")]
        public async Task<IActionResult> PerformanceReport(DateTime? startDate, DateTime? endDate)
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

                // Default date range: last 30 days
                var start = startDate ?? DateTime.Now.AddDays(-30);
                var end = endDate ?? DateTime.Now;

                // Get donations
                var donations = await _context.Donations
                    .Where(d => d.FarId == farmer.FarId &&
                      d.DonDate >= start &&  // Direct comparison
                      d.DonDate <= end)
                    //.Where(d => d.FarId == farmer.FarId &&
                    //       d.DonDate.HasValue &&
                    //       d.DonDate.Value >= start &&
                    //       d.DonDate.Value <= end)
                    .Include(d => d.DonationCrops)
                        .ThenInclude(dc => dc.Cro)
                    .ToListAsync();

                var donationStats = new DonationPerformanceStats
                {
                    TotalDonations = donations.Count,
                    PendingDonations = donations.Count(d => d.DonStatus == "Pending"),
                    ApprovedDonations = donations.Count(d => d.DonStatus == "Approved"),
                    DeliveredDonations = donations.Count(d => d.DonStatus == "Delivered"),
                    TotalDonatedQuantity = donations.Sum(d => d.DonationCrops.Sum(dc => dc.DcQuantity)),
                    DonationImpact = donations.Count(d => d.DonStatus == "Delivered") * 10 // Mock impact
                };

                // Get baskets and sales
                var baskets = await _context.Baskets
                    .Where(b => b.FarId == farmer.FarId)
                    .Include(b => b.Carts)
                        .ThenInclude(c => c.Ord)
                    .Include(b => b.BasketCrops)
                        .ThenInclude(bc => bc.Cro)
                    .ToListAsync();

                var salesStats = new SalesPerformanceStats
                {
                    TotalBaskets = baskets.Count,
                    ActiveBaskets = baskets.Count(b => b.BasQty > 0),
                    SoldBaskets = baskets.Sum(b => b.Carts.Sum(c => c.CartQty)),
                    TotalRevenue = baskets.Sum(b => b.Carts.Sum(c => c.CartQty * (b.BasPrice ?? 0))),
                    AverageBasketPrice = baskets.Any() ? baskets.Average(b => b.BasPrice ?? 0) : 0
                };

                // Monthly donation trends
                var monthlyDonations = donations
                    .GroupBy(d => d.DonDate.Month)
                    .Select(g => new MonthlyDonationTrend
                    {
                        Month = g.Key,
                        Count = g.Count(),
                        DeliveredCount = g.Count(d => d.DonStatus == "Delivered")
                    })
                    .ToList();

                // Monthly sales trends
                var monthlySales = baskets
                    .GroupBy(b => b.AddedAt.Month)
                    .Select(g => new MonthlySalesTrend
                    {
                        Month = g.Key,
                        Count = g.Count(),
                        Revenue = g.Sum(b => b.Carts.Sum(c => c.CartQty * (b.BasPrice ?? 0)))
                    })
                    .ToList();

                var viewModel = new PerformanceReportViewModel
                {
                    Farmer = farmer,
                    StartDate = start,
                    EndDate = end,
                    Donations = donations,
                    Baskets = baskets,
                    DonationStats = donationStats,
                    SalesStats = salesStats,
                    MonthlyDonations = monthlyDonations,
                    MonthlySales = monthlySales
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading performance report");
                SetErrorMessage("حدث خطأ أثناء تحميل تقرير الأداء");
                return RedirectToAction("Dashboard", "Farmer");
            }
        }

        // GET: Farmer/Reports/ExportCropInventory
        [HttpGet("ExportCropInventory")]
        public async Task<IActionResult> ExportCropInventory(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var farmer = await _context.Farmers
                    .FirstOrDefaultAsync(f => f.UserId == userId);

                if (farmer == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات المزارع");
                    return RedirectToAction(nameof(CropInventoryReport));
                }

                var start = startDate ?? DateTime.Now.AddDays(-30);
                var end = endDate ?? DateTime.Now;

                var crops = await _context.Crops
                    .Where(c => c.FarId == farmer.FarId)
                    .Include(c => c.BasketCrops)
                    .ToListAsync();

                // Generate CSV content
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("اسم المحصول,النوع,الكمية,الوحدة,الوزن,تاريخ الإضافة,تاريخ الانتهاء,الأيام حتى الانتهاء,الحالة");

                foreach (var crop in crops)
                {
                    var daysUntilExpiry = crop.ExpiredAt.HasValue ? (int)(crop.ExpiredAt.Value - DateTime.Now).TotalDays : 0;
                    var status = GetExpiryStatus(daysUntilExpiry);
                    csv.AppendLine($"{crop.CroName},{crop.CroType},{crop.CroQuantity ?? 0},{crop.CroUnit},{crop.CroWeight ?? 0},{crop.AddedAt:yyyy-MM-dd},{crop.ExpiredAt?.ToString("yyyy-MM-dd") ?? ""},{daysUntilExpiry},{status}");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                var fileName = $"crop_inventory_{DateTime.Now:yyyyMMdd}.csv";

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error exporting crop inventory");
                SetErrorMessage("حدث خطأ أثناء تصدير التقرير");
                return RedirectToAction(nameof(CropInventoryReport));
            }
        }

        /// <summary>
        /// Helper method to get expiry status
        /// </summary>
        private string GetExpiryStatus(int daysUntilExpiry)
        {
            if (daysUntilExpiry < 0)
                return "expired";
            else if (daysUntilExpiry <= 7)
                return "expiring_soon";
            else if (daysUntilExpiry <= 30)
                return "expiring_later";
            else
                return "good";
        }
    }
}