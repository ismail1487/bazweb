using Baz.Attributes;
using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.Model.Pattern;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using Baz.SharedSession;
using BazWebApp.Models;
using BazWebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Controllers
{
    /// <summary>
    /// Takvim işlemlerini yöneten controller class'ı.
    /// </summary>
    public class TakvimController : Controller
    {
        private readonly CookieModel _cookieModel;
        private readonly IParamOrganizasyonBirimleriService _paramOrganizasyonBirimleriService;

        /// <summary>
        /// Takvim işlemlerini yöneten controller Konst.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bazCookieService"></param>
        /// <param name="requestHelper"></param>
        /// <param name="paramOrganizasyonBirimleriService"></param>
        /// <param name="sharedSession"></param>
        public TakvimController(ILogger<TakvimController> logger, IBazCookieService bazCookieService, IRequestHelper requestHelper, IParamOrganizasyonBirimleriService paramOrganizasyonBirimleriService, ISharedSession sharedSession)
        {
            _cookieModel = bazCookieService.GetCookie().Result;
            _paramOrganizasyonBirimleriService = paramOrganizasyonBirimleriService;
        }

        #region view method

        /// <summary>
        /// Takvim index sayfasına erişim sağlayan method.
        /// </summary>
        /// <returns>ilgili sayfayı döndürür.</returns>
        [Route("/Calendar")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/Calendar")]
        public IActionResult Index()
        {
            return View("Index");
        }

        /// <summary>
        /// Takvim index sayfasına erişim sağlayan method.
        /// </summary>
        /// <returns>ilgili sayfayı döndürür.</returns>
        [Route("/FastMeet")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/FastMeet")]
        public IActionResult HizliToplanti()
        {
            return View("HizliToplanti");
        }

        #endregion view method

        #region Web API Methods

        /// <summary>
        /// Organizasyon tipine göre tip getiren method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProcessName(Name = "Organizasyon birim tipine göre listenin getirilmesi işlemi")]
        [HttpPost]
        public Result<List<KurumOrganizasyonBirimView>> ListTip(KurumOrganizasyonBirimRequest request)
        {
            request.KurumId = int.Parse(_cookieModel.KurumId);
            var result = _paramOrganizasyonBirimleriService.ListTip(request);
            return result;
        }

        #endregion Web API Methods
    }
}