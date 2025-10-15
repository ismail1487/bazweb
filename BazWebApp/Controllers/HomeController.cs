using BazWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Linq;
using System.Threading.Tasks;
using BazWebApp.Handlers;
using Microsoft.AspNetCore.Mvc.Filters;
using Baz.SharedSession;
using BazWebApp.Services;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Baz.ProcessResult;
using Microsoft.Extensions.DependencyInjection;

namespace BazWebApp.Controllers
{
    /// <summary>
    /// Anasayfa kontrol methodlarının yer aldığı sınıftır.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class HomeController : Controller
    {
        private readonly IBazCookieService _bazCookieService;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Anasayfa kontrol methodlarının yer aldığı sınıfının yapıcı metodu
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bazCookieService"></param>
        /// <param name="localizationService"></param>
        /// <param name="menuService"></param>
        /// <param name="serviceProvider"></param>
        public HomeController(ILogger<HomeController> logger, IBazCookieService bazCookieService, ILocalizationService localizationService, IMenuService menuService, IServiceProvider serviceProvider)
        {
            _bazCookieService = bazCookieService;
            _serviceProvider = serviceProvider;
        }

        #region Login-Privacy-Error

        /// <summary>
        /// Login Sayfasına yönlendiren method.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var cookie = await _bazCookieService.GetCookie();
            if (cookie != null)
            {
                return RedirectToAction("Index", "Panel");
            }
            return View("Login");
        }

        /// <summary>
        /// Privacy sayfasına yönlendiren metod
        /// </summary>
        /// <returns></returns>
        [PolicyBasedAuthorize]
        public IActionResult Privacy()
        {
            return View("Privacy");
        }

        /// <summary>
        /// Error sayfasına yönlendiren metod
        /// </summary>
        /// <returns></returns>

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion Login-Privacy-Error

        #region Views

        /// <summary>
        /// Lisans satın alma sayfasına yönledniren metod
        /// </summary>
        /// <returns></returns>
        [Route("/Panel/PurchaseLicense")]
        public IActionResult PurchaseLicense()
        {
            var cook = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            //cook.
            return View("PurchaseLicense");
        }

        /// <summary>
        /// yetkiniz bulunmamaktadır sayfasına yönlendiren metot.
        /// </summary>
        /// <returns></returns>
        [Route("NoAuth")]
        public IActionResult NoAuth()
        {
            return View("NoAuth");
        }

        /// <summary>
        /// lisans süreniz bitmiştir sayfasına yönlendiren metot.
        /// </summary>
        /// <returns></returns>
        [Route("LicenseExpired")]
        public IActionResult LicenseExpired()
        {
            return View("LicenseExpired");
        }

        /// <summary>
        /// yetkiniz bulunmamaktadır sayfasına yönlendiren metot.
        /// </summary>
        /// <returns></returns>
        [Route("PermissionDenied")]
        public IActionResult PermissionDenied()
        {
            return View("PermissionDenied");
        }

        

        #endregion Views

        #region Api Methods

        /// <summary>
        /// Kişi Rol Admin mi Kontrolü.
        /// </summary>
        /// <returns></returns>
        [Route("/home/AdminMi")]
        [HttpGet]
        public Result<bool> AdminMi()
        {
            var _kurumOrganizasyonBirimTanimlariService = _serviceProvider.GetService<IKurumOrganizasyonBirimTanimlariService>();

            var result = _kurumOrganizasyonBirimTanimlariService.AdminMi();
            return result;
        }

        #endregion Api Methods
    }
}