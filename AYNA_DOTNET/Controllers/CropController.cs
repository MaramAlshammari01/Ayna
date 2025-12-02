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
    [Route("Farmer/Crops")]
    public class CropController : BaseController
    {
        public CropController(AynaDbContext context, ILogger<CropController> logger)
            : base(context, logger)
        {
        }

        // GET: Farmer/Crops
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

                var crops = await _context.Crops
                    .Where(c => c.FarId == farmer.FarId)
                    .OrderByDescending(c => c.AddedAt)
                    .ToListAsync();

                var viewModel = new CropIndexViewModel
                {
                    Crops = crops,
                    Farmer = farmer
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading crops");
                SetErrorMessage("حدث خطأ أثناء تحميل المحاصيل");
                return RedirectToAction("Dashboard", "Farmer");
            }
        }

        // POST: Farmer/Crops/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCropViewModel model)
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

                // Validate expiration date is in the future
                if (model.ExpiredAt <= DateTime.Now)
                {
                    SetErrorMessage("تاريخ الانتهاء يجب أن يكون في المستقبل");
                    return RedirectToAction(nameof(Index));
                }

                var crop = new Crop
                {
                    CroName = model.CroName,
                    CroType = model.CroType,
                    CroWeight = model.CroWeight,
                    CroQuantity = model.CroQuantity,
                    CroUnit = model.CroUnit,
                    ExpiredAt = model.ExpiredAt,
                    FarId = farmer.FarId,
                    AddedAt = DateTime.Now
                };

                _context.Crops.Add(crop);
                await _context.SaveChangesAsync();

                SetSuccessMessage("تم إضافة المحصول بنجاح");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                LogError(ex, "Error creating crop");
                SetErrorMessage("حدث خطأ أثناء إضافة المحصول");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Farmer/Crops/Update/{id}
        [HttpPost("Update/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, UpdateCropViewModel model)
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

                var crop = await _context.Crops
                    .FirstOrDefaultAsync(c => c.CroId == id && c.FarId == farmer.FarId);

                if (crop == null)
                {
                    SetErrorMessage("لم يتم العثور على المحصول");
                    return RedirectToAction(nameof(Index));
                }

                // Validate expiration date
                if (model.ExpiredAt <= DateTime.Now)
                {
                    SetErrorMessage("تاريخ الانتهاء يجب أن يكون في المستقبل");
                    return RedirectToAction(nameof(Index));
                }

                // Update crop
                crop.CroName = model.CroName;
                crop.CroType = model.CroType;
                crop.CroWeight = model.CroWeight;
                crop.CroQuantity = model.CroQuantity;
                crop.CroUnit = model.CroUnit;
                crop.ExpiredAt = model.ExpiredAt;

                _context.Crops.Update(crop);
                await _context.SaveChangesAsync();

                SetSuccessMessage("تم تحديث المحصول بنجاح");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                LogError(ex, "Error updating crop");
                SetErrorMessage("حدث خطأ أثناء تحديث المحصول");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Farmer/Crops/Delete/{id}
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

                var crop = await _context.Crops
                    .Include(c => c.BasketCrops)
                    .Include(c => c.DonationCrops)
                    .FirstOrDefaultAsync(c => c.CroId == id && c.FarId == farmer.FarId);

                if (crop == null)
                {
                    SetErrorMessage("لم يتم العثور على المحصول");
                    return RedirectToAction(nameof(Index));
                }

                // Check if crop is used in any baskets or donations
                if (crop.BasketCrops.Any() || crop.DonationCrops.Any())
                {
                    SetErrorMessage("لا يمكن حذف المحصول لأنه مستخدم في سلال أو تبرعات");
                    return RedirectToAction(nameof(Index));
                }

                _context.Crops.Remove(crop);
                await _context.SaveChangesAsync();

                SetSuccessMessage("تم حذف المحصول بنجاح");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                LogError(ex, "Error deleting crop");
                SetErrorMessage(GetDatabaseErrorMessage(ex));
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Farmer/Crops/Details/{id}
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

                var crop = await _context.Crops
                    .FirstOrDefaultAsync(c => c.CroId == id && c.FarId == farmer.FarId);

                if (crop == null)
                {
                    return JsonError("لم يتم العثور على المحصول");
                }

                return JsonSuccess("تم تحميل بيانات المحصول بنجاح", new
                {
                    crop = new
                    {
                        id = crop.CroId,
                        name = crop.CroName,
                        type = crop.CroType,
                        weight = crop.CroWeight,
                        quantity = crop.CroQuantity,
                        unit = crop.CroUnit,
                        expiredAt = crop.ExpiredAt,
                        addedAt = crop.AddedAt,
                        shelfLife = crop.CroShelfLife
                    }
                });
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading crop details");
                return JsonError("حدث خطأ أثناء تحميل بيانات المحصول");
            }
        }

        // GET: Farmer/Crops/Available
        [HttpGet("Available")]
        public async Task<IActionResult> GetAvailableCrops()
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

                var crops = await _context.Crops
                    .Where(c => c.FarId == farmer.FarId && c.CroQuantity > 0)
                    .Select(c => new
                    {
                        id = c.CroId,
                        name = c.CroName,
                        type = c.CroType,
                        quantity = c.CroQuantity,
                        unit = c.CroUnit,
                        expiredAt = c.ExpiredAt
                    })
                    .ToListAsync();

                return JsonSuccess("تم تحميل المحاصيل المتاحة", crops);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading available crops");
                return JsonError("حدث خطأ أثناء تحميل المحاصيل");
            }
        }
    }
}