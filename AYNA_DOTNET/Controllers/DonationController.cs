using Ayna.Data;
using Ayna.Models;
using Ayna.Support;
using Ayna.ViewModels.FarmerVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ayna.Controllers.Farmer
{
    [Authorize(Policy = "FarmerOnly")]
    [Route("Farmer/Donations")]
    public class DonationController : BaseController
    {
        public DonationController(AynaDbContext context, ILogger<DonationController> logger)
            : base(context, logger)
        {
        }

        // GET: Farmer/Donations
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

                var donations = await _context.Donations
                    .Where(d => d.FarId == farmer.FarId)
                    .Include(d => d.DonationCrops)
                        .ThenInclude(dc => dc.Cro)
                    .Include(d => d.DonatReqs)
                        .ThenInclude(dr => dr.Char)
                    .OrderByDescending(d => d.AddedAt)
                    .ToListAsync();

                var charities = await _context.Charities.ToListAsync();

                var crops = await _context.Crops
                    .Where(c => c.FarId == farmer.FarId && c.CroQuantity > 0)
                    .ToListAsync();

                var viewModel = new DonationIndexViewModel
                {
                    Donations = donations,
                    Farmer = farmer,
                    Charities = charities,
                    AvailableCrops = crops
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading donations");
                SetErrorMessage("حدث خطأ أثناء تحميل التبرعات");
                return RedirectToAction("Dashboard", "Farmer");
            }
        }

        // POST: Farmer/Donations/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDonationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = GetModelStateErrors();
                SetErrorMessage(string.Join(", ", errors));
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

                // Create donation
                var donation = new Donation
                {
                    DonDescription = model.DonDescription,
                    DonStatus = Constants.DONATION_STATUS_PENDING,
                    FarId = farmer.FarId,
                    DonDate = DateTime.Now,
                    AddedAt = DateTime.Now
                };

                _context.Donations.Add(donation);
                await _context.SaveChangesAsync();

                // Add crops to donation
                foreach (var cropItem in model.Crops)
                {
                    var donationCrop = new DonationCrop
                    {
                        DonId = donation.DonId,
                        CroId = cropItem.CropId,
                        DcQuantity = cropItem.Quantity,
                        AddedAt = DateTime.Now
                    };

                    _context.DonationCrops.Add(donationCrop);

                    // Decrease crop quantity
                    var crop = await _context.Crops.FindAsync(cropItem.CropId);
                    crop.CroQuantity -= cropItem.Quantity;
                    _context.Crops.Update(crop);
                }

                // Create donation request
                var donationRequest = new DonatReq
                {
                    ReqStatus = Constants.REQUEST_STATUS_PENDING,
                    ReqDonation = model.DonDescription,
                    DonationRequestPickupDuration = model.PickupDuration,
                    DonId = donation.DonId,
                    CharId = model.CharId,
                    AddedAt = DateTime.Now
                };

                _context.DonatReqs.Add(donationRequest);
                await _context.SaveChangesAsync();

                SetSuccessMessage("تم إنشاء طلب التبرع بنجاح وتم إرساله للجمعية الخيرية");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                LogError(ex, "Error creating donation");
                SetErrorMessage("حدث خطأ أثناء إنشاء طلب التبرع");
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Farmer/Donations/Details/{id}
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

                var donation = await _context.Donations
                    .Where(d => d.FarId == farmer.FarId && d.DonId == id)
                    .Include(d => d.DonationCrops)
                        .ThenInclude(dc => dc.Cro)
                    .Include(d => d.DonatReqs)
                        .ThenInclude(dr => dr.Char)
                    .FirstOrDefaultAsync();

                if (donation == null)
                {
                    return JsonError("لم يتم العثور على التبرع");
                }

                return JsonSuccess("تم تحميل بيانات التبرع بنجاح", new
                {
                    donation = new
                    {
                        id = donation.DonId,
                        description = donation.DonDescription,
                        status = donation.DonStatus,
                        date = donation.DonDate,
                        crops = donation.DonationCrops.Select(dc => new
                        {
                            cropName = dc.Cro.CroName,
                            quantity = dc.DcQuantity,
                            unit = dc.Cro.CroUnit
                        }),
                        requests = donation.DonatReqs.Select(dr => new
                        {
                            charityName = dr.Char.CharName,
                            status = dr.ReqStatus,
                            pickupDuration = dr.DonationRequestPickupDuration
                        })
                    }
                });
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading donation details");
                return JsonError("حدث خطأ أثناء تحميل بيانات التبرع");
            }
        }

        // POST: Farmer/Donations/UpdateStatus/{id}
        [HttpPost("UpdateStatus/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, UpdateDonationStatusViewModel model)
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

                var donation = await _context.Donations
                    .FirstOrDefaultAsync(d => d.DonId == id && d.FarId == farmer.FarId);

                if (donation == null)
                {
                    SetErrorMessage("لم يتم العثور على التبرع");
                    return RedirectToAction(nameof(Index));
                }

                donation.DonStatus = model.DonStatus;
                _context.Donations.Update(donation);
                await _context.SaveChangesAsync();

                SetSuccessMessage("تم تحديث حالة التبرع بنجاح");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                LogError(ex, "Error updating donation status");
                SetErrorMessage("حدث خطأ أثناء تحديث حالة التبرع");
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Farmer/Donations/Reports
        [HttpGet("Reports")]
        public async Task<IActionResult> Reports()
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

                var donations = await _context.Donations
                    .Where(d => d.FarId == farmer.FarId)
                    .Include(d => d.DonationCrops)
                        .ThenInclude(dc => dc.Cro)
                    .Include(d => d.DonatReqs)
                        .ThenInclude(dr => dr.Char)
                    .OrderByDescending(d => d.AddedAt)
                    .ToListAsync();

                var viewModel = new DonationReportsViewModel
                {
                    Farmer = farmer,
                    Donations = donations,
                    TotalDonations = donations.Count,
                    PendingDonations = donations.Count(d => d.DonStatus == Constants.DONATION_STATUS_PENDING),
                    ApprovedDonations = donations.Count(d => d.DonStatus == Constants.DONATION_STATUS_APPROVED),
                    DeliveredDonations = donations.Count(d => d.DonStatus == Constants.DONATION_STATUS_DELIVERED)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading donation reports");
                SetErrorMessage("حدث خطأ أثناء تحميل تقارير التبرعات");
                return RedirectToAction("Dashboard", "Farmer");
            }
        }

        // POST: Farmer/Donations/AssignToCharity/{id}
        [HttpPost("AssignToCharity/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToCharity(int id, AssignDonationToCharityViewModel model)
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

                var donation = await _context.Donations
                    .FirstOrDefaultAsync(d => d.DonId == id && d.FarId == farmer.FarId);

                if (donation == null)
                {
                    SetErrorMessage("لم يتم العثور على التبرع");
                    return RedirectToAction(nameof(Index));
                }

                if (donation.DonStatus != Constants.DONATION_STATUS_PENDING)
                {
                    SetErrorMessage("لا يمكن تعيين تبرع تم قبوله أو تسليمه مسبقاً");
                    return RedirectToAction(nameof(Index));
                }

                // Create new donation request
                var donationRequest = new DonatReq
                {
                    ReqStatus = Constants.REQUEST_STATUS_PENDING,
                    ReqDonation = donation.DonDescription,
                    DonationRequestPickupDuration = model.PickupDuration,
                    DonId = donation.DonId,
                    CharId = model.CharId,
                    AddedAt = DateTime.Now
                };

                _context.DonatReqs.Add(donationRequest);

                // Update donation status
                donation.DonStatus = Constants.DONATION_STATUS_PENDING;
                _context.Donations.Update(donation);

                await _context.SaveChangesAsync();

                SetSuccessMessage("تم تعيين التبرع للجمعية الجديدة بنجاح");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                LogError(ex, "Error assigning donation to charity");
                SetErrorMessage("حدث خطأ أثناء تعيين التبرع");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}