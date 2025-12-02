using Microsoft.AspNetCore.Mvc;
using Ayna.Data;
using Microsoft.Extensions.Logging;

namespace Ayna.Controllers
{

    public class FrontController : BaseController
    {
        public FrontController(AynaDbContext context, ILogger<FrontController> logger)
            : base(context, logger)
        {
        }

        [HttpGet]
        public IActionResult Home()
        {
            try
            {
                _logger.LogInformation("Home page accessed by {UserType}",
                    CurrentUserType ?? "Anonymous");

                return View();
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading home page");
                SetErrorMessage("حدث خطأ أثناء تحميل الصفحة الرئيسية");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult HowItWorks()
        {
            try
            {
                _logger.LogInformation("How It Works page accessed");

                return View();
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading how it works page");
                SetErrorMessage("حدث خطأ أثناء تحميل الصفحة");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult About()
        {
            try
            {
                _logger.LogInformation("About page accessed");

                return View();
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading about page");
                SetErrorMessage("حدث خطأ أثناء تحميل الصفحة");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Contact()
        {
            try
            {
                _logger.LogInformation("Contact page accessed");

                return View();
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading contact page");
                SetErrorMessage("حدث خطأ أثناء تحميل الصفحة");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            try
            {
                _logger.LogInformation("Privacy page accessed");

                return View();
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading privacy page");
                SetErrorMessage("حدث خطأ أثناء تحميل الصفحة");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Terms()
        {
            try
            {
                _logger.LogInformation("Terms page accessed");

                return View();
            }
            catch (Exception ex)
            {
                LogError(ex, "Error loading terms page");
                SetErrorMessage("حدث خطأ أثناء تحميل الصفحة");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                ViewBag.StatusCode = statusCode.Value;
                ViewBag.ErrorMessage = statusCode.Value switch
                {
                    404 => "الصفحة غير موجودة",
                    403 => "غير مصرح بالوصول",
                    500 => "حدث خطأ في الخادم",
                    _ => "حدث خطأ غير متوقع"
                };
            }

            return View();
        }
    }
}