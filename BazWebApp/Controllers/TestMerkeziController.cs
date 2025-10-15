using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baz.Attributes;
using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using BazWebApp.Models;
using BazWebApp.Services;

namespace BazWebApp.Controllers
{
    /// <summary>
    /// Test arayüz sayfalarıyla ilgili metotların yer aldığı sınıftır
    /// </summary>
    public class TestMerkeziController : Controller
    {
        private readonly IBazCookieService _bazCookieService;
        private readonly IParamService _paramService;

        /// <summary>
        /// Test arayüz sayfalarıyla ilgili metotların yer aldığı konst.
        /// </summary>
        /// <param name="testService"></param>
        /// <param name="paramService"></param>
        /// <param name="bazCookieService"></param>
        public TestMerkeziController( IParamService paramService, IBazCookieService bazCookieService)
        {
            _paramService = paramService;
          
            _bazCookieService = bazCookieService;
        }

        #region View Method

        /// <summary>
        /// Test sayfasına yönlendiren metod
        /// </summary>
        /// <returns></returns>
        [Route("/TestCenter/TestView")]
        public IActionResult TestView()
        {
            return View("TestView");
        }

        /// <summary>
        /// Test fatura sayfasına yönlendiren metod
        /// </summary>
        /// <returns></returns>
        [Route("/testcenter/fatura")]
        public IActionResult FaturaView()
        {
            return View("fatura");
        }

        #endregion View Method

        #region Api Method

        

        /// <summary>
        /// Parametreleri listeleyen metot
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("/TestMerkezi/param/list")]
        [HttpPost]
        [ProcessName(Name = "")]
        public Result<List<ParametreResult>> List(ParametreRequest request)
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            request.KurumId = Convert.ToInt32(cookie.KurumId);
            return _paramService.List(request);
        }

        #endregion Api Method
    }
}