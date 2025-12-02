using Ayna.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Ayna.Controllers
{
    
    public class BaseController : Controller
    {
        protected readonly AynaDbContext _context;
        protected readonly ILogger _logger;

        public BaseController(AynaDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        #region User Info Properties
        protected int? CurrentUserId
        {
            get
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(userIdClaim, out int userId) ? userId : null;
            }
        }
        protected string CurrentUserEmail => User.FindFirst(ClaimTypes.Email)?.Value;
        protected string CurrentUserType => User.FindFirst("UserType")?.Value;
        protected string CurrentUserRole => User.FindFirst(ClaimTypes.Role)?.Value;
        protected bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false;

        #endregion

        #region Success/Error Messages
        protected void SetSuccessMessage(string message)
        {
            TempData["SuccessMessage"] = message;
        }
        protected void SetErrorMessage(string message)
        {
            TempData["ErrorMessage"] = message;
        }
        protected void SetWarningMessage(string message)
        {
            TempData["WarningMessage"] = message;
        }
        protected void SetInfoMessage(string message)
        {
            TempData["InfoMessage"] = message;
        }

        #endregion

        #region Error Logging
        protected void LogError(Exception exception, string customMessage = null)
        {
            _logger.LogError(exception, customMessage ?? "An error occurred");
            _logger.LogError("Exception Message: {Message}", exception.Message);
            _logger.LogError("Stack Trace: {StackTrace}", exception.StackTrace);

            if (exception.InnerException != null)
            {
                _logger.LogError("Inner Exception: {InnerException}", exception.InnerException.Message);
            }
        }
        protected void LogError(string message)
        {
            _logger.LogError(message);
        }

        #endregion

        #region Common Actions
        protected JsonResult JsonSuccess(string message, object data = null)
        {
            return Json(new
            {
                success = true,
                message = message,
                data = data
            });
        }
        protected JsonResult JsonError(string message, object errors = null)
        {
            return Json(new
            {
                success = false,
                message = message,
                errors = errors
            });
        }
        protected IActionResult RedirectToReferrer(string defaultAction = "Index", string defaultController = null)
        {
            var referrer = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(referrer))
            {
                return Redirect(referrer);
            }

            if (!string.IsNullOrEmpty(defaultController))
            {
                return RedirectToAction(defaultAction, defaultController);
            }

            return RedirectToAction(defaultAction);
        }

        #endregion

        #region Override Methods
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Set common ViewBag properties
            ViewBag.CurrentUserId = CurrentUserId;
            ViewBag.CurrentUserEmail = CurrentUserEmail;
            ViewBag.CurrentUserType = CurrentUserType;
            ViewBag.CurrentUserRole = CurrentUserRole;
            ViewBag.IsAuthenticated = IsAuthenticated;

            base.OnActionExecuting(context);
        }

        #endregion

        #region Helper Methods
        protected List<string> GetModelStateErrors()
        {
            return ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
        }
        protected bool ValidateUserAccess(int resourceUserId)
        {
            if (!CurrentUserId.HasValue)
                return false;
            if (CurrentUserRole == "admin")
                return true;
            return CurrentUserId.Value == resourceUserId;
        }
        protected string GetDatabaseErrorMessage(Exception ex)
        {
            if (ex.Message.Contains("FOREIGN KEY"))
                return "لا يمكن حذف هذا العنصر لأنه مرتبط بعناصر أخرى";

            if (ex.Message.Contains("UNIQUE"))
                return "القيمة المدخلة موجودة مسبقاً";

            if (ex.Message.Contains("DELETE"))
                return "حدث خطأ أثناء الحذف";

            if (ex.Message.Contains("UPDATE"))
                return "حدث خطأ أثناء التحديث";

            if (ex.Message.Contains("INSERT"))
                return "حدث خطأ أثناء الإضافة";

            return "حدث خطأ في قاعدة البيانات";
        }

        #endregion
    }
}
