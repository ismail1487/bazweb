using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Controllers
{
    /// <summary>
    /// Hata sayfaları işlemlerinin yönetildiği controller.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ErrorController : Controller
    {
        /// <summary>
        /// 404 Sayfa bulunamadı türünd hataların işlendiği methot.
        /// </summary>
        /// <returns></returns>
        [Route("Error/403")]
        public IActionResult Page403()
        {
            var statusCodeData = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            if (statusCodeData != null)
            {
                ViewBag.RouteOfException = statusCodeData.OriginalPath;
            }
            ViewBag.ErrorMessage = "Sayfa bulunamadı";
            return View("403");
        }   
        [Route("Error/404")]
        public IActionResult Page404()
        {
            var statusCodeData = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            if (statusCodeData != null)
            {
                ViewBag.RouteOfException = statusCodeData.OriginalPath;
            }
            ViewBag.ErrorMessage = "Sayfa bulunamadı";
            return View("404");
        }
        /// <summary>
        /// 500 sistem hatası türünde hataların işlendiği metot.
        /// </summary>
        /// <returns></returns>
        [Route("Error/500")]
        [AllowAnonymous]
        public IActionResult Page500()
        {
            var statusCodeData = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (statusCodeData != null)
            {
                ViewBag.RouteOfException = statusCodeData.OriginalPath;
            }
            ViewBag.ErrorMessage = "Sistemde bir aksaklık oluştu.";
            return View("500");
        }
    }
}
