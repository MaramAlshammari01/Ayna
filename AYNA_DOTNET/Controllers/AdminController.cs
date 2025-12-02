using Ayna.Data;
using Ayna.ViewModels.AdminVMs;
using Ayna.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Ayna.Controllers.Admin
{

    [Authorize(Policy = "AdminOnly")]
    [Route("Admin")]
    public class AdminController : BaseController
    {
        public AdminController(AynaDbContext context, ILogger<AdminController> logger)
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
                var viewModel = new DashboardViewModel
                {
                    // User Statistics
                    TotalUsers = await _context.Users.CountAsync(),
                    TotalFarmers = await _context.Farmers.CountAsync(),
                    TotalCharities = await _context.Charities.CountAsync(),
                    TotalDonors = await _context.Donors.CountAsync(),

                    // Donation Statistics
                    TotalDonations = await _context.Donations.CountAsync(),
                    TotalOrders = await _context.Orders.CountAsync(),
                    PendingDonations = await _context.Donations
                        .CountAsync(d => d.DonStatus == Constants.DONATION_STATUS_PENDING),
                    CompletedOrders = await _context.Orders
                        .CountAsync(o => o.OrdStatus == Constants.ORDER_STATUS_COMPLETED),

                    // Recent Activities
                    RecentUsers = await _context.Users
                        .Include(u => u.Farmers)
                        .Include(u => u.Charities)
                        .Include(u => u.Donors)
                        .OrderByDescending(u => u.AddedAt)
                        .Take(5)
                        .Select(u => new RecentUserDto
                        {
                            UserId = u.UserId,
                            UserEmail = u.UserEmail,
                            UserType = u.UserType,
                            UserStatus = u.UserStatus,
                            AddedAt = u.AddedAt,
                            Name = u.UserType == Constants.USER_TYPE_FARMER
                                ? u.Farmers.Select(f => f.FarFirstName + " " + f.FarLastName).FirstOrDefault()
                                : u.UserType == Constants.USER_TYPE_CHARITY
                                    ? u.Charities.Select(c => c.CharName).FirstOrDefault()
                                    : u.Donors.Select(d => d.DonorFirstName + " " + d.DonorLastName).FirstOrDefault()
                        })
                        .ToListAsync(),

                    RecentDonations = await _context.Donations
                        .Include(d => d.Far)
                        .Include(d => d.DonationCrops)
                            .ThenInclude(dc => dc.Cro)
                        .OrderByDescending(d => d.AddedAt)
                        .Take(5)
                        .Select(d => new RecentDonationDto
                        {
                            DonId = d.DonId,
                            FarmerName = d.Far.FarFirstName + " " + d.Far.FarLastName,
                            DonDescription = d.DonDescription,
                            DonStatus = d.DonStatus,
                            DonDate = d.DonDate,
                            CropCount = d.DonationCrops.Count
                        })
                        .ToListAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading admin dashboard");
                SetErrorMessage("حدث خطأ أثناء تحميل لوحة التحكم");
                return View(new DashboardViewModel());
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

                var admin = await _context.Users.FindAsync(CurrentUserId.Value);
                if (admin == null)
                    return NotFound();

                var viewModel = new AdminProfileViewModel
                {
                    UserId = admin.UserId,
                    UserEmail = admin.UserEmail,
                    UserPhone = admin.UserPhone,
                    UserType = admin.UserType,
                    UserStatus = admin.UserStatus,
                    AddedAt = admin.AddedAt
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading admin profile");
                SetErrorMessage("حدث خطأ أثناء تحميل الملف الشخصي");
                return RedirectToAction(nameof(Dashboard));
            }
        }

        [HttpPost("Profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
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

                var admin = await _context.Users.FindAsync(CurrentUserId.Value);
                if (admin == null)
                    return NotFound();

                // Check email uniqueness
                var emailExists = await _context.Users
                    .AnyAsync(u => u.UserEmail == model.UserEmail && u.UserId != admin.UserId);

                if (emailExists)
                {
                    ModelState.AddModelError(nameof(model.UserEmail), "البريد الإلكتروني مستخدم من قبل");
                    return View("Profile", model);
                }

                // Update profile data
                admin.UserEmail = model.UserEmail;
                admin.UserPhone = model.UserPhone;

                // Update password if provided
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    admin.UserPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                }

                await _context.SaveChangesAsync();

                SetSuccessMessage("تم تحديث المعلومات الشخصية بنجاح");
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                LogError(ex, "Error updating admin profile");
                SetErrorMessage("حدث خطأ أثناء تحديث الملف الشخصي");
                return View("Profile", model);
            }
        }

        #endregion

        #region Users Management

        [HttpGet("Users")]
        public async Task<IActionResult> Users(string type, string status)
        {
            try
            {
                var query = _context.Users
                    .Include(u => u.Farmers)
                    .Include(u => u.Charities)
                    .Include(u => u.Donors)
                    .AsQueryable();

                // Filter by user type
                if (!string.IsNullOrWhiteSpace(type))
                {
                    query = query.Where(u => u.UserType == type);
                }

                // Filter by status
                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(u => u.UserStatus == status);
                }

                var users = await query
                    .OrderByDescending(u => u.AddedAt)
                    .Select(u => new UserListItemViewModel
                    {
                        UserId = u.UserId,
                        UserEmail = u.UserEmail,
                        UserPhone = u.UserPhone,
                        UserType = u.UserType,
                        UserStatus = u.UserStatus,
                        AddedAt = u.AddedAt,
                        Name = u.UserType == Constants.USER_TYPE_FARMER
                            ? u.Farmers.Select(f => f.FarFirstName + " " + f.FarLastName).FirstOrDefault()
                            : u.UserType == Constants.USER_TYPE_CHARITY
                                ? u.Charities.Select(c => c.CharName).FirstOrDefault()
                                : u.Donors.Select(d => d.DonorFirstName + " " + d.DonorLastName).FirstOrDefault()
                    })
                    .ToListAsync();

                var viewModel = new UsersIndexViewModel
                {
                    Users = users,
                    FilterType = type,
                    FilterStatus = status
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading users list");
                SetErrorMessage("حدث خطأ أثناء تحميل قائمة المستخدمين");
                return View(new UsersIndexViewModel());
            }
        }

        [HttpGet("Users/{id}")]
        public async Task<IActionResult> ShowUser(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Farmers)
                    .Include(u => u.Charities)
                    .Include(u => u.Donors)
                    .FirstOrDefaultAsync(u => u.UserId == id);

                if (user == null)
                {
                    SetErrorMessage("المستخدم غير موجود");
                    return RedirectToAction(nameof(Users));
                }

                var viewModel = new UserDetailsViewModel
                {
                    UserId = user.UserId,
                    UserEmail = user.UserEmail,
                    UserPhone = user.UserPhone,
                    UserType = user.UserType,
                    UserStatus = user.UserStatus,
                    AddedAt = user.AddedAt
                };

                // Set type-specific details
                if (user.UserType == Constants.USER_TYPE_FARMER)
                {
                    var farmer = user.Farmers.FirstOrDefault();
                    if (farmer != null)
                    {
                        viewModel.FarmerDetails = new FarmerDetailsDto
                        {
                            FarId = farmer.FarId,
                            FarFirstName = farmer.FarFirstName,
                            FarLastName = farmer.FarLastName,
                            FarLocation = farmer.FarLocation,
                            CropsCount = await _context.Crops.CountAsync(c => c.FarId == farmer.FarId),
                            DonationsCount = await _context.Donations.CountAsync(d => d.FarId == farmer.FarId)
                        };
                    }
                }
                else if (user.UserType == Constants.USER_TYPE_CHARITY)
                {
                    var charity = user.Charities.FirstOrDefault();
                    if (charity != null)
                    {
                        viewModel.CharityDetails = new CharityDetailsDto
                        {
                            CharId = charity.CharId,
                            CharName = charity.CharName,
                            CharContact = charity.CharContact,
                            CharCr = charity.CharCr,
                            CharLocation = charity.CharLocation,
                            OrdersCount = await _context.Orders.CountAsync(o => o.CharId == charity.CharId),
                            RequestsCount = await _context.DonatReqs.CountAsync(r => r.CharId == charity.CharId)
                        };
                    }
                }
                else if (user.UserType == Constants.USER_TYPE_DONOR)
                {
                    var donor = user.Donors.FirstOrDefault();
                    if (donor != null)
                    {
                        viewModel.DonorDetails = new DonorDetailsDto
                        {
                            DonorId = donor.DonorId,
                            DonorFirstName = donor.DonorFirstName,
                            DonorLastName = donor.DonorLastName,
                            OrdersCount = await _context.Orders.CountAsync(o => o.DonorId == donor.DonorId)
                        };
                    }
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error loading user details for user {id}");
                SetErrorMessage("حدث خطأ أثناء تحميل تفاصيل المستخدم");
                return RedirectToAction(nameof(Users));
            }
        }

        [HttpPost("Users/{id}/UpdateStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserStatus(int id, UpdateUserStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("حالة المستخدم غير صالحة");
                return RedirectToAction(nameof(ShowUser), new { id });
            }

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    SetErrorMessage("المستخدم غير موجود");
                    return RedirectToAction(nameof(Users));
                }

                user.UserStatus = model.UserStatus;
                await _context.SaveChangesAsync();

                SetSuccessMessage("تم تحديث حالة المستخدم بنجاح");
                return RedirectToAction(nameof(ShowUser), new { id });
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error updating status for user {id}");
                SetErrorMessage("حدث خطأ أثناء تحديث حالة المستخدم");
                return RedirectToAction(nameof(ShowUser), new { id });
            }
        }

        [HttpPost("Users/{id}/Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUser(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Farmers)
                    .Include(u => u.Charities)
                    .Include(u => u.Donors)
                    .FirstOrDefaultAsync(u => u.UserId == id);

                if (user == null)
                {
                    SetErrorMessage("المستخدم غير موجود");
                    return RedirectToAction(nameof(Users));
                }

                // Remove related records based on user type
                if (user.UserType == Constants.USER_TYPE_FARMER)
                {
                    var farmer = user.Farmers.FirstOrDefault();
                    if (farmer != null)
                    {
                        _context.Farmers.Remove(farmer);
                    }
                }
                else if (user.UserType == Constants.USER_TYPE_CHARITY)
                {
                    var charity = user.Charities.FirstOrDefault();
                    if (charity != null)
                    {
                        _context.Charities.Remove(charity);
                    }
                }
                else if (user.UserType == Constants.USER_TYPE_DONOR)
                {
                    var donor = user.Donors.FirstOrDefault();
                    if (donor != null)
                    {
                        _context.Donors.Remove(donor);
                    }
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                SetSuccessMessage("تم حذف المستخدم بنجاح");
                return RedirectToAction(nameof(Users));
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error removing user {id}");
                SetErrorMessage(GetDatabaseErrorMessage(ex));
                return RedirectToAction(nameof(ShowUser), new { id });
            }
        }

        #endregion

        #region Donations Management

        [HttpGet("Donations")]
        public async Task<IActionResult> Donations(string status, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var query = _context.Donations
                    .Include(d => d.Far)
                    .Include(d => d.DonationCrops)
                        .ThenInclude(dc => dc.Cro)
                    .Include(d => d.DonatReqs)
                        .ThenInclude(dr => dr.Char)
                    .AsQueryable();

                // Filter by status
                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(d => d.DonStatus == status);
                }

                // Filter by date range
                if (startDate.HasValue)
                {
                    query = query.Where(d => d.AddedAt >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(d => d.AddedAt <= endDate.Value.AddDays(1));
                }

                var donations = await query
                    .OrderByDescending(d => d.AddedAt)
                    .Select(d => new DonationListItemViewModel
                    {
                        DonId = d.DonId,
                        FarmerName = d.Far.FarFirstName + " " + d.Far.FarLastName,
                        DonDescription = d.DonDescription,
                        DonStatus = d.DonStatus,
                        DonDate = d.DonDate,
                        AddedAt = d.AddedAt,
                        CropsCount = d.DonationCrops.Count,
                        RequestsCount = d.DonatReqs.Count
                    })
                    .ToListAsync();

                var viewModel = new DonationsIndexViewModel
                {
                    Donations = donations,
                    FilterStatus = status,
                    FilterStartDate = startDate,
                    FilterEndDate = endDate
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading donations list");
                SetErrorMessage("حدث خطأ أثناء تحميل قائمة التبرعات");
                return View(new DonationsIndexViewModel());
            }
        }

        [HttpGet("Donations/Reports")]
        public async Task<IActionResult> DonationReports()
        {
            try
            {
                var viewModel = new DonationReportsViewModel
                {
                    // Donation Statistics
                    DonationStats = new DonationStatsDto
                    {
                        Total = await _context.Donations.CountAsync(),
                        Pending = await _context.Donations.CountAsync(d => d.DonStatus == Constants.DONATION_STATUS_PENDING),
                        Approved = await _context.Donations.CountAsync(d => d.DonStatus == Constants.DONATION_STATUS_APPROVED),
                        Delivered = await _context.Donations.CountAsync(d => d.DonStatus == Constants.DONATION_STATUS_DELIVERED),
                        Rejected = await _context.Donations.CountAsync(d => d.DonStatus == Constants.DONATION_STATUS_REJECTED)
                    },

                    // Top Farmers by Donations
                    TopFarmers = await _context.Farmers
                        .Select(f => new TopFarmerDto
                        {
                            FarId = f.FarId,
                            FarmerName = f.FarFirstName + " " + f.FarLastName,
                            FarLocation = f.FarLocation,
                            DonationsCount = _context.Donations.Count(d => d.FarId == f.FarId && d.DonStatus == Constants.DONATION_STATUS_DELIVERED)
                        })
                        .Where(f => f.DonationsCount > 0)
                        .OrderByDescending(f => f.DonationsCount)
                        .Take(10)
                        .ToListAsync(),

                    // Top Charities by Received Donations
                    TopCharities = await _context.Charities
                        .Select(c => new TopCharityDto
                        {
                            CharId = c.CharId,
                            CharName = c.CharName,
                            CharLocation = c.CharLocation,
                            ReceivedDonationsCount = _context.DonatReqs.Count(dr => dr.CharId == c.CharId && dr.ReqStatus == Constants.REQUEST_STATUS_COMPLETED)
                        })
                        .Where(c => c.ReceivedDonationsCount > 0)
                        .OrderByDescending(c => c.ReceivedDonationsCount)
                        .Take(10)
                        .ToListAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading donation reports");
                SetErrorMessage("حدث خطأ أثناء تحميل تقارير التبرعات");
                return View(new DonationReportsViewModel());
            }
        }

        #endregion

        #region Reports Generation

        [HttpPost("Reports/Generate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateReport(GenerateReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("يرجى التحقق من البيانات المدخلة");
                return RedirectToAction(nameof(Dashboard));
            }

            try
            {
                object reportData = model.ReportType switch
                {
                    "users" => await GenerateUsersReport(model.StartDate, model.EndDate),
                    "donations" => await GenerateDonationsReport(model.StartDate, model.EndDate),
                    "orders" => await GenerateOrdersReport(model.StartDate, model.EndDate),
                    "financial" => await GenerateFinancialReport(model.StartDate, model.EndDate),
                    _ => null
                };

                if (reportData == null)
                {
                    SetErrorMessage("نوع التقرير غير صالح");
                    return RedirectToAction(nameof(Dashboard));
                }

                var viewModel = new ReportResultsViewModel
                {
                    ReportType = model.ReportType,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ReportData = reportData
                };

                return View("ReportResults", viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error generating {model.ReportType} report");
                SetErrorMessage("حدث خطأ أثناء إنشاء التقرير");
                return RedirectToAction(nameof(Dashboard));
            }
        }

        private async Task<UsersReportData> GenerateUsersReport(DateTime startDate, DateTime endDate)
        {
            var users = await _context.Users
                .Where(u => u.AddedAt >= startDate && u.AddedAt <= endDate.AddDays(1))
                .ToListAsync();

            return new UsersReportData
            {
                TotalUsers = users.Count,
                UsersByType = users.GroupBy(u => u.UserType).ToDictionary(g => g.Key, g => g.Count()),
                UsersByStatus = users.GroupBy(u => u.UserStatus).ToDictionary(g => g.Key, g => g.Count()),
                NewUsersPerDay = users.GroupBy(u => u.AddedAt.Date).ToDictionary(g => g.Key, g => g.Count())
            };
        }

        private async Task<DonationsReportData> GenerateDonationsReport(DateTime startDate, DateTime endDate)
        {
            var donations = await _context.Donations
                .Where(d => d.AddedAt >= startDate && d.AddedAt <= endDate.AddDays(1))
                .ToListAsync();

            return new DonationsReportData
            {
                TotalDonations = donations.Count,
                DonationsByStatus = donations.GroupBy(d => d.DonStatus).ToDictionary(g => g.Key, g => g.Count()),
                DonationsPerDay = donations.GroupBy(d => d.AddedAt.Date).ToDictionary(g => g.Key, g => g.Count())
            };
        }

        private async Task<OrdersReportData> GenerateOrdersReport(DateTime startDate, DateTime endDate)
        {
            var orders = await _context.Orders
                .Where(o => o.OrdDate >= DateOnly.FromDateTime(startDate) && o.OrdDate <= DateOnly.FromDateTime(endDate))
                .ToListAsync();

            return new OrdersReportData
            {
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.OrdPrice ?? 0),
                OrdersByStatus = orders.GroupBy(o => o.OrdStatus).ToDictionary(g => g.Key, g => g.Count()),
                RevenuePerDay = orders.GroupBy(o => o.OrdDate).ToDictionary(g => g.Key, g => g.Sum(o => o.OrdPrice ?? 0))
            };
        }

        private async Task<FinancialReportData> GenerateFinancialReport(DateTime startDate, DateTime endDate)
        {
            var orders = await _context.Orders
                .Where(o => o.OrdDate >= DateOnly.FromDateTime(startDate) && o.OrdDate <= DateOnly.FromDateTime(endDate))
                .ToListAsync();

            var payments = await _context.Payments
                .Where(p => p.PayDate >= DateOnly.FromDateTime(startDate) && p.PayDate <= DateOnly.FromDateTime(endDate))
                .ToListAsync();

            return new FinancialReportData
            {
                TotalRevenue = orders.Sum(o => o.OrdPrice ?? 0),
                TotalPayments = payments.Where(p => p.PayStatus == Constants.PAYMENT_STATUS_COMPLETED).Sum(p => p.PayAmount ?? 0),
                PaymentMethods = payments.GroupBy(p => p.PayMethod).ToDictionary(g => g.Key, g => g.Count()),
                RevenueByDay = orders.GroupBy(o => o.OrdDate).ToDictionary(g => g.Key, g => g.Sum(o => o.OrdPrice ?? 0))
            };
        }

        #endregion
    }
}