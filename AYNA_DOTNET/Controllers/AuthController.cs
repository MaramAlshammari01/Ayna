using Ayna.Data;
using Ayna.Models;
using Ayna.Support;
using Ayna.ViewModels.AuthVMs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ayna.Controllers
{
    public class AuthController : BaseController
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100000;

        public AuthController(AynaDbContext context, ILogger<AuthController> logger)
            : base(context, logger)
        {
        }

        #region Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Redirect if already authenticated
            if (IsAuthenticated)
            {
                return RedirectToDashboard(CurrentUserType);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    SetErrorMessage("يرجى تصحيح الأخطاء في النموذج");
                    return View(model);
                }

                // Find user by email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserEmail == model.UserEmail);

                if (user == null)
                {
                    _logger.LogWarning("Login attempt with non-existent email: {Email}", model.UserEmail);
                    SetErrorMessage("البريد الإلكتروني أو كلمة المرور غير صحيحة");
                    return View(model);
                }

                // Verify password
                if (!VerifyPassword(model.UserPassword, user.UserPassword))
                {
                    _logger.LogWarning("Failed login attempt for user: {Email}", model.UserEmail);
                    SetErrorMessage("البريد الإلكتروني أو كلمة المرور غير صحيحة");
                    return View(model);
                }

                // Check if user is active
                if (user.UserStatus != Constants.USER_STATUS_ACTIVE)
                {
                    _logger.LogWarning("Inactive user attempted login: {Email}", model.UserEmail);
                    SetErrorMessage("حسابك غير نشط. يرجى التواصل مع الدعم");
                    return View(model);
                }

                // Create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.UserEmail),
                    new Claim("UserType", user.UserType),
                    new Claim(ClaimTypes.Name, user.UserEmail)
                };

                // Create identity and principal
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = model.RememberMe
                            ? DateTimeOffset.UtcNow.AddDays(30)
                            : DateTimeOffset.UtcNow.AddHours(2)
                    });

                _logger.LogInformation("User logged in: {Email}, Type: {UserType}",
                    user.UserEmail, user.UserType);

                SetSuccessMessage("تم تسجيل الدخول بنجاح!");

                // Redirect to appropriate dashboard or return URL
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToDashboard(user.UserType);
            }
            catch (Exception ex)
            {
                LogError(ex, "Error during login");
                SetErrorMessage("حدث خطأ أثناء تسجيل الدخول. يرجى المحاولة مرة أخرى");
                return View(model);
            }
        }

        #endregion

        #region Register

        [HttpGet]
        public IActionResult Register()
        {
            // Redirect if already authenticated
            if (IsAuthenticated)
            {
                return RedirectToDashboard(CurrentUserType);
            }

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    SetErrorMessage("يرجى تصحيح الأخطاء في النموذج");
                    return View(model);
                }

                // Check if email already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserEmail == model.UserEmail);

                if (existingUser != null)
                {
                    ModelState.AddModelError("UserEmail", "البريد الإلكتروني مستخدم بالفعل");
                    SetErrorMessage("البريد الإلكتروني مستخدم بالفعل");
                    return View(model);
                }

                // Additional validation for Charity
                if (model.UserType == Constants.USER_TYPE_CHARITY)
                {
                    if (string.IsNullOrWhiteSpace(model.CharName))
                    {
                        ModelState.AddModelError("CharName", "اسم الجمعية مطلوب");
                        SetErrorMessage("يرجى إدخال اسم الجمعية");
                        return View(model);
                    }

                    if (string.IsNullOrWhiteSpace(model.CharCR))
                    {
                        ModelState.AddModelError("CharCR", "السجل التجاري مطلوب");
                        SetErrorMessage("يرجى إدخال السجل التجاري");
                        return View(model);
                    }
                }

                // Start transaction
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Create User
                    var user = new User
                    {
                        UserEmail = model.UserEmail,
                        UserPassword = HashPassword(model.UserPassword),
                        UserPhone = model.UserPhone,
                        UserType = model.UserType,
                        UserStatus = Constants.USER_STATUS_ACTIVE,
                        AddedAt = DateTime.Now
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    // Create role-specific entity
                    switch (model.UserType)
                    {
                        case Constants.USER_TYPE_FARMER:
                            var farmer = new Ayna.Models.Farmer
                            {
                                FarFirstName = model.FirstName,
                                FarLastName = model.LastName,
                                FarLocation = model.Location,
                                UserId = user.UserId,
                                AddedAt = DateTime.Now
                            };
                            _context.Farmers.Add(farmer);
                            break;

                        case Constants.USER_TYPE_DONOR:
                            var donor = new Ayna.Models.Donor
                            {
                                DonorFirstName = model.FirstName,
                                DonorLastName = model.LastName,
                                UserId = user.UserId,
                                AddedAt = DateTime.Now
                            };
                            _context.Donors.Add(donor);
                            break;

                        case Constants.USER_TYPE_CHARITY:
                            var charity = new Ayna.Models.Charity
                            {
                                CharName = model.CharName,
                                CharCr = model.CharCR,
                                CharLocation = model.Location,
                                CharContact = model.UserPhone,
                                UserId = user.UserId,
                                AddedAt = DateTime.Now
                            };
                            _context.Charities.Add(charity);
                            break;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("New user registered: {Email}, Type: {UserType}",
                        user.UserEmail, user.UserType);

                    // Auto login after registration
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Email, user.UserEmail),
                        new Claim("UserType", user.UserType),
                        new Claim(ClaimTypes.Name, user.UserEmail)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties
                        {
                            IsPersistent = false,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                        });

                    SetSuccessMessage("تم إنشاء الحساب بنجاح!");
                    return RedirectToDashboard(user.UserType);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Error during registration");
                SetErrorMessage("حدث خطأ أثناء إنشاء الحساب. يرجى المحاولة مرة أخرى");
                return View(model);
            }
        }

        #endregion

        #region Logout

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userEmail = CurrentUserEmail;

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();

                _logger.LogInformation("User logged out: {Email}", userEmail ?? "Unknown");

                SetSuccessMessage("تم تسجيل الخروج بنجاح!");
                return RedirectToAction("Home", "Front");
            }
            catch (Exception ex)
            {
                LogError(ex, "Error during logout");
                SetErrorMessage("حدث خطأ أثناء تسجيل الخروج");
                return RedirectToAction("Home", "Front");
            }
        }

        #endregion

        #region Access Denied

        [HttpGet]
        public IActionResult AccessDenied()
        {
            _logger.LogWarning("Access denied for user: {Email}", CurrentUserEmail ?? "Anonymous");
            SetErrorMessage("غير مصرح لك بالوصول إلى هذه الصفحة");
            return View();
        }

        #endregion

        #region Password Helper Methods

        /// <summary>
        /// Hashes password using secure PBKDF2 algorithm
        /// </summary>
        private string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256);

            var hash = pbkdf2.GetBytes(KeySize);

            var hashBytes = new byte[SaltSize + KeySize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);

            return $"{Iterations}.{Convert.ToBase64String(hashBytes)}";
        }

        /// <summary>
        /// Verifies password against hash
        /// </summary>
        private bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            try
            {
                var parts = hashedPassword.Split('.');
                if (parts.Length != 2)
                {
                    return VerifyLegacyHash(password, hashedPassword);
                }

                var iterations = int.Parse(parts[0]);
                var hashBytes = Convert.FromBase64String(parts[1]);

                var salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                using var pbkdf2 = new Rfc2898DeriveBytes(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256);

                var hash = pbkdf2.GetBytes(KeySize);

                for (int i = 0; i < KeySize; i++)
                {
                    if (hashBytes[i + SaltSize] != hash[i])
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifies legacy SHA256 hashed passwords
        /// </summary>
        private bool VerifyLegacyHash(string password, string hashedPassword)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var legacyHash = Convert.ToBase64String(hashedBytes);
            return legacyHash == hashedPassword;
        }

        /// <summary>
        /// Checks if a hashed password needs to be rehashed with current settings
        /// </summary>
        private bool NeedsRehash(string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
                return true;

            var parts = hashedPassword.Split('.');
            if (parts.Length != 2)
                return true;

            if (int.TryParse(parts[0], out int iterations))
            {
                return iterations != Iterations;
            }

            return true;
        }

        /// <summary>
        /// Generates a random secure password
        /// </summary>
        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            var random = new Random();
            var chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(chars);
        }

        /// <summary>
        /// Validates password strength and returns feedback
        /// </summary>
        private (bool IsValid, string Message) ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "كلمة المرور مطلوبة");

            if (password.Length < 8)
                return (false, "كلمة المرور يجب أن تكون 8 أحرف على الأقل");

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            if (!hasUpper)
                return (false, "كلمة المرور يجب أن تحتوي على حرف كبير واحد على الأقل");

            if (!hasLower)
                return (false, "كلمة المرور يجب أن تحتوي على حرف صغير واحد على الأقل");

            if (!hasDigit)
                return (false, "كلمة المرور يجب أن تحتوي على رقم واحد على الأقل");

            if (!hasSpecial)
                return (true, "كلمة المرور قوية (يُنصح بإضافة رمز خاص)");

            return (true, "كلمة المرور قوية");
        }

        #endregion

        #region Helper Methods      
        private IActionResult RedirectToDashboard(string userType)
        {
            return userType switch
            {
                Constants.USER_TYPE_FARMER => RedirectToAction("Index", "Farmer"),
                Constants.USER_TYPE_DONOR => RedirectToAction("Index", "Donor"),
                Constants.USER_TYPE_CHARITY => RedirectToAction("Index", "Charity"),
                _ => RedirectToAction("Home", "Front")
            };
        }

        #endregion
    }
}