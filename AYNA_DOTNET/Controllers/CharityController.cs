using Ayna.Data;
using Ayna.Models;
using Ayna.ViewModels.CharityVMs;
using Ayna.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ayna.Controllers.Charity
{
    
    [Authorize(Policy = "CharityOnly")]
    [Route("Charity")]
    public class CharityController : BaseController
    {
        public CharityController(AynaDbContext context, ILogger<CharityController> logger)
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

                var charity = await _context.Charities
                    .FirstOrDefaultAsync(c => c.UserId == CurrentUserId.Value);

                if (charity == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات الجمعية الخيرية");
                    return RedirectToAction("Login", "Account");
                }

                var viewModel = new CharityDashboardViewModel
                {
                    CharityInfo = new CharityInfoDto
                    {
                        CharId = charity.CharId,
                        CharName = charity.CharName,
                        CharContact = charity.CharContact,
                        CharCr = charity.CharCr,
                        CharLocation = charity.CharLocation
                    },

                    // Statistics
                    TotalOrders = await _context.Orders
                        .CountAsync(o => o.CharId == charity.CharId),

                    TotalDonationRequests = await _context.DonatReqs
                        .CountAsync(dr => dr.CharId == charity.CharId),

                    PendingRequests = await _context.DonatReqs
                        .CountAsync(dr => dr.CharId == charity.CharId && dr.ReqStatus == Constants.REQUEST_STATUS_PENDING),

                    CompletedOrders = await _context.Orders
                        .CountAsync(o => o.CharId == charity.CharId && o.OrdStatus == Constants.ORDER_STATUS_COMPLETED),

                    // Recent Orders
                    RecentOrders = await _context.Orders
                        .Where(o => o.CharId == charity.CharId)
                        .Include(o => o.Donor)
                        .OrderByDescending(o => o.AddedAt)
                        .Take(5)
                        .Select(o => new RecentOrderDto
                        {
                            OrdId = o.OrdId,
                            DonorName = o.Donor.DonorFirstName + " " + o.Donor.DonorLastName,
                            OrdPrice = o.OrdPrice,
                            OrdStatus = o.OrdStatus,
                            OrdDate = o.OrdDate,
                            OrdTime = o.OrdTime
                        })
                        .ToListAsync(),

                    // Recent Donation Requests
                    RecentDonationRequests = await _context.DonatReqs
                        .Where(dr => dr.CharId == charity.CharId)
                        .Include(dr => dr.Don)
                            .ThenInclude(d => d.Far)
                        .OrderByDescending(dr => dr.AddedAt)
                        .Take(5)
                        .Select(dr => new RecentDonationRequestDto
                        {
                            ReqId = dr.ReqId,
                            DonId = dr.DonId,
                            FarmerName = dr.Don.Far.FarFirstName + " " + dr.Don.Far.FarLastName,
                            ReqDonation = dr.ReqDonation,
                            ReqStatus = dr.ReqStatus,
                            AddedAt = dr.AddedAt
                        })
                        .ToListAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading charity dashboard");
                SetErrorMessage("حدث خطأ أثناء تحميل لوحة التحكم");
                return View(new CharityDashboardViewModel());
            }
        }

        #endregion

        #region Profile Management
        [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {
            try
            {
                if (!CurrentUserId.HasValue)
                    return RedirectToAction("Login", "Account");

                var charity = await _context.Charities
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.UserId == CurrentUserId.Value);

                if (charity == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات الجمعية");
                    return RedirectToAction(nameof(Dashboard));
                }

                var viewModel = new CharityProfileViewModel
                {
                    CharId = charity.CharId,
                    CharName = charity.CharName,
                    CharContact = charity.CharContact,
                    CharCr = charity.CharCr,
                    CharLocation = charity.CharLocation,
                    UserEmail = charity.User.UserEmail,
                    UserPhone = charity.User.UserPhone,
                    AddedAt = charity.AddedAt
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading charity profile");
                SetErrorMessage("حدث خطأ أثناء تحميل الملف الشخصي");
                return RedirectToAction(nameof(Dashboard));
            }
        }
        [HttpPost("Profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateCharityProfileViewModel model)
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

                var charity = await _context.Charities
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.UserId == CurrentUserId.Value);

                if (charity == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات الجمعية");
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

                // Update charity info
                charity.CharName = model.CharName;
                charity.CharContact = model.CharContact;
                charity.CharCr = model.CharCr;
                charity.CharLocation = model.CharLocation;

                // Update user info
                charity.User.UserEmail = model.UserEmail;
                charity.User.UserPhone = model.UserPhone;

                // Update password if provided
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    charity.User.UserPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                }

                await _context.SaveChangesAsync();

                SetSuccessMessage("تم تحديث المعلومات الشخصية بنجاح");
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                LogError(ex, "Error updating charity profile");
                SetErrorMessage("حدث خطأ أثناء تحديث الملف الشخصي");
                return View("Profile", model);
            }
        }

        #endregion

        #region Available Donations

        [HttpGet("Donations/Available")]
        public async Task<IActionResult> AvailableDonations(string search, string cropType)
        {
            try
            {
                var query = _context.Donations
                    .Where(d => d.DonStatus == Constants.DONATION_STATUS_APPROVED)
                    .Include(d => d.Far)
                    .Include(d => d.DonationCrops)
                        .ThenInclude(dc => dc.Cro)
                    .AsQueryable();

                // Search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(d =>
                        d.DonDescription.Contains(search) ||
                        d.Far.FarFirstName.Contains(search) ||
                        d.Far.FarLastName.Contains(search));
                }

                // Crop type filter
                if (!string.IsNullOrWhiteSpace(cropType))
                {
                    query = query.Where(d => d.DonationCrops.Any(dc => dc.Cro.CroType == cropType));
                }

                var donations = await query
                    .OrderByDescending(d => d.AddedAt)
                    .Select(d => new AvailableDonationDto
                    {
                        DonId = d.DonId,
                        FarmerName = d.Far.FarFirstName + " " + d.Far.FarLastName,
                        FarmerLocation = d.Far.FarLocation,
                        DonDescription = d.DonDescription,
                        DonDate = d.DonDate,
                        Crops = d.DonationCrops.Select(dc => new DonationCropDto
                        {
                            CropName = dc.Cro.CroName,
                            CropType = dc.Cro.CroType,
                            Quantity = dc.DcQuantity,
                            Unit = dc.Cro.CroUnit
                        }).ToList()
                    })
                    .ToListAsync();

                var viewModel = new AvailableDonationsViewModel
                {
                    Donations = donations,
                    SearchQuery = search,
                    CropTypeFilter = cropType
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading available donations");
                SetErrorMessage("حدث خطأ أثناء تحميل التبرعات المتاحة");
                return View(new AvailableDonationsViewModel());
            }
        }

        [HttpGet("Donations/{id}")]
        public async Task<IActionResult> DonationDetails(int id)
        {
            try
            {
                var donation = await _context.Donations
                    .Include(d => d.Far)
                    .Include(d => d.DonationCrops)
                        .ThenInclude(dc => dc.Cro)
                    .Include(d => d.PickUps)
                    .FirstOrDefaultAsync(d => d.DonId == id);

                if (donation == null)
                {
                    SetErrorMessage("التبرع غير موجود");
                    return RedirectToAction(nameof(AvailableDonations));
                }

                var viewModel = new DonationDetailsViewModel
                {
                    DonId = donation.DonId,
                    FarmerName = donation.Far.FarFirstName + " " + donation.Far.FarLastName,
                    FarmerLocation = donation.Far.FarLocation,
                    DonDescription = donation.DonDescription,
                    DonStatus = donation.DonStatus,
                    DonDate = donation.DonDate,
                    AddedAt = donation.AddedAt,
                    Crops = donation.DonationCrops.Select(dc => new DonationCropDto
                    {
                        CropName = dc.Cro.CroName,
                        CropType = dc.Cro.CroType,
                        Quantity = dc.DcQuantity,
                        Unit = dc.Cro.CroUnit,
                        Weight = dc.Cro.CroWeight,
                        ShelfLife = dc.Cro.CroShelfLife
                    }).ToList(),
                    PickupInfo = donation.PickUps.Select(p => new PickupInfoDto
                    {
                        PickupLocation = p.PickLocation,
                        PickupTime = p.PickTime
                    }).FirstOrDefault()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error loading donation details for {id}");
                SetErrorMessage("حدث خطأ أثناء تحميل تفاصيل التبرع");
                return RedirectToAction(nameof(AvailableDonations));
            }
        }

        #endregion

        #region Donation Requests

        [HttpPost("Donations/{id}/Request")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDonationRequest(int id, CreateDonationRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("يرجى التحقق من البيانات المدخلة");
                return RedirectToAction(nameof(DonationDetails), new { id });
            }

            try
            {
                if (!CurrentUserId.HasValue)
                    return RedirectToAction("Login", "Account");

                var charity = await _context.Charities
                    .FirstOrDefaultAsync(c => c.UserId == CurrentUserId.Value);

                if (charity == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات الجمعية");
                    return RedirectToAction(nameof(Dashboard));
                }

                // Check if donation exists and is approved
                var donation = await _context.Donations
                    .FirstOrDefaultAsync(d => d.DonId == id && d.DonStatus == Constants.DONATION_STATUS_APPROVED);

                if (donation == null)
                {
                    SetErrorMessage("التبرع غير متاح للطلب");
                    return RedirectToAction(nameof(AvailableDonations));
                }

                // Check if already requested
                var existingRequest = await _context.DonatReqs
                    .AnyAsync(dr => dr.DonId == id && dr.CharId == charity.CharId && dr.ReqStatus == Constants.REQUEST_STATUS_PENDING);

                if (existingRequest)
                {
                    SetErrorMessage("لديك طلب معلق بالفعل لهذا التبرع");
                    return RedirectToAction(nameof(DonationDetails), new { id });
                }

                // Create donation request
                var donatReq = new DonatReq
                {
                    CharId = charity.CharId,
                    DonId = id,
                    ReqDonation = model.ReqDonation,
                    DonationRequestPickupDuration = model.PickupDuration,
                    ReqStatus = Constants.REQUEST_STATUS_PENDING,
                    AddedAt = DateTime.Now
                };

                _context.DonatReqs.Add(donatReq);
                await _context.SaveChangesAsync();

                SetSuccessMessage("تم إرسال طلب التبرع بنجاح");
                return RedirectToAction(nameof(MyRequests));
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error creating donation request for donation {id}");
                SetErrorMessage("حدث خطأ أثناء إرسال طلب التبرع");
                return RedirectToAction(nameof(DonationDetails), new { id });
            }
        }

        [HttpGet("Requests")]
        public async Task<IActionResult> MyRequests(string status)
        {
            try
            {
                if (!CurrentUserId.HasValue)
                    return RedirectToAction("Login", "Account");

                var charity = await _context.Charities
                    .FirstOrDefaultAsync(c => c.UserId == CurrentUserId.Value);

                if (charity == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات الجمعية");
                    return RedirectToAction(nameof(Dashboard));
                }

                var query = _context.DonatReqs
                    .Where(dr => dr.CharId == charity.CharId)
                    .Include(dr => dr.Don)
                        .ThenInclude(d => d.Far)
                    .Include(dr => dr.Don.DonationCrops)
                        .ThenInclude(dc => dc.Cro)
                    .AsQueryable();

                // Filter by status
                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(dr => dr.ReqStatus == status);
                }

                var requests = await query
                    .OrderByDescending(dr => dr.AddedAt)
                    .Select(dr => new DonationRequestDto
                    {
                        ReqId = dr.ReqId,
                        DonId = dr.DonId,
                        FarmerName = dr.Don.Far.FarFirstName + " " + dr.Don.Far.FarLastName,
                        ReqDonation = dr.ReqDonation,
                        ReqStatus = dr.ReqStatus,
                        PickupDuration = dr.DonationRequestPickupDuration,
                        AddedAt = dr.AddedAt,
                        CropsCount = dr.Don.DonationCrops.Count
                    })
                    .ToListAsync();

                var viewModel = new MyRequestsViewModel
                {
                    Requests = requests,
                    FilterStatus = status
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading donation requests");
                SetErrorMessage("حدث خطأ أثناء تحميل طلبات التبرع");
                return View(new MyRequestsViewModel());
            }
        }

        [HttpPost("Requests/{id}/Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRequest(int id)
        {
            try
            {
                if (!CurrentUserId.HasValue)
                    return RedirectToAction("Login", "Account");

                var charity = await _context.Charities
                    .FirstOrDefaultAsync(c => c.UserId == CurrentUserId.Value);

                if (charity == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات الجمعية");
                    return RedirectToAction(nameof(Dashboard));
                }

                var request = await _context.DonatReqs
                    .FirstOrDefaultAsync(dr => dr.ReqId == id && dr.CharId == charity.CharId);

                if (request == null)
                {
                    SetErrorMessage("الطلب غير موجود");
                    return RedirectToAction(nameof(MyRequests));
                }

                if (request.ReqStatus != Constants.REQUEST_STATUS_PENDING)
                {
                    SetErrorMessage("لا يمكن إلغاء هذا الطلب");
                    return RedirectToAction(nameof(MyRequests));
                }

                _context.DonatReqs.Remove(request);
                await _context.SaveChangesAsync();

                SetSuccessMessage("تم إلغاء الطلب بنجاح");
                return RedirectToAction(nameof(MyRequests));
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error canceling donation request {id}");
                SetErrorMessage("حدث خطأ أثناء إلغاء الطلب");
                return RedirectToAction(nameof(MyRequests));
            }
        }

        #endregion

        #region Orders Management

        [HttpGet("Orders")]
        public async Task<IActionResult> Orders(string status)
        {
            try
            {
                if (!CurrentUserId.HasValue)
                    return RedirectToAction("Login", "Account");

                var charity = await _context.Charities
                    .FirstOrDefaultAsync(c => c.UserId == CurrentUserId.Value);

                if (charity == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات الجمعية");
                    return RedirectToAction(nameof(Dashboard));
                }

                var query = _context.Orders
                    .Where(o => o.CharId == charity.CharId)
                    .Include(o => o.Donor)
                    .Include(o => o.Carts)
                        .ThenInclude(c => c.Bas)
                    .AsQueryable();

                // Filter by status
                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(o => o.OrdStatus == status);
                }

                var orders = await query
                    .OrderByDescending(o => o.AddedAt)
                    .Select(o => new OrderDto
                    {
                        OrdId = o.OrdId,
                        DonorName = o.Donor != null ? o.Donor.DonorFirstName + " " + o.Donor.DonorLastName : "غير محدد",
                        OrdPrice = o.OrdPrice,
                        OrdStatus = o.OrdStatus,
                        OrdDate = o.OrdDate,
                        OrdTime = o.OrdTime,
                        ItemsCount = o.Carts.Count
                    })
                    .ToListAsync();

                var viewModel = new OrdersViewModel
                {
                    Orders = orders,
                    FilterStatus = status
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading orders");
                SetErrorMessage("حدث خطأ أثناء تحميل الطلبات");
                return View(new OrdersViewModel());
            }
        }

        [HttpGet("Orders/{id}")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                if (!CurrentUserId.HasValue)
                    return RedirectToAction("Login", "Account");

                var charity = await _context.Charities
                    .FirstOrDefaultAsync(c => c.UserId == CurrentUserId.Value);

                if (charity == null)
                {
                    SetErrorMessage("لم يتم العثور على بيانات الجمعية");
                    return RedirectToAction(nameof(Dashboard));
                }

                var order = await _context.Orders
                    .Include(o => o.Donor)
                    .Include(o => o.Carts)
                        .ThenInclude(c => c.Bas)
                            .ThenInclude(b => b.BasketCrops)
                                .ThenInclude(bc => bc.Cro)
                    .Include(o => o.Payments)
                    .FirstOrDefaultAsync(o => o.OrdId == id && o.CharId == charity.CharId);

                if (order == null)
                {
                    SetErrorMessage("الطلب غير موجود");
                    return RedirectToAction(nameof(Orders));
                }

                var viewModel = new OrderDetailsViewModel
                {
                    OrdId = order.OrdId,
                    DonorName = order.Donor != null ? order.Donor.DonorFirstName + " " + order.Donor.DonorLastName : "غير محدد",
                    OrdPrice = order.OrdPrice,
                    OrdStatus = order.OrdStatus,
                    OrdDate = order.OrdDate,
                    OrdTime = order.OrdTime,
                    AddedAt = order.AddedAt,
                    Items = order.Carts.Select(c => new OrderItemDto
                    {
                        BasketName = c.Bas.BasContent,
                        Quantity = c.CartQty,
                        Price = c.CartPrice,
                        TotalPrice = c.CartPrice * c.CartQty
                    }).ToList(),
                    PaymentInfo = order.Payments.Select(p => new PaymentInfoDto
                    {
                        PayMethod = p.PayMethod,
                        PayAmount = p.PayAmount,
                        PayStatus = p.PayStatus,
                        PayDate = p.PayDate,
                        TransactionId = p.TransactionId
                    }).FirstOrDefault()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error loading order details for {id}");
                SetErrorMessage("حدث خطأ أثناء تحميل تفاصيل الطلب");
                return RedirectToAction(nameof(Orders));
            }
        }

        #endregion
    }
}