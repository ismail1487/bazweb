using Baz.Attributes;
using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using BazWebApp.Models;
using BazWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace BazWebApp.Controllers
{
    /// <summary>
    /// Parametre kontrol metodlarının yer aldığı sınıftır.
    /// </summary>
    public class ParamController : Controller
    {
        /// <summary>
        /// Parametre control metotlarının yer aldığı sınıftır.
        /// </summary>
        private readonly IParamService _paramService;

        private readonly CookieModel cookieModel;

        /// <summary>
        /// Parametre control metotlarının yer aldığı konst.
        /// </summary>
        /// <param name="paramService"></param>
        /// <param name="bazCookieService"></param>
        public ParamController(IParamService paramService, IBazCookieService bazCookieService)
        {
            cookieModel = bazCookieService.GetCookie().GetAwaiter().GetResult();
            _paramService = paramService;
        }

        #region View Methods

        /// <summary>
        /// Index sayfasına yönlendiren metot
        /// </summary>
        /// <returns></returns>
        [Route("/panel/param/")]
        [HttpGet]
        [ProcessName(Name = "")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/param/")]
        public ActionResult Index()
        {
            return View("Index");
        }

        #endregion View Methods

        #region Api Methods

        /// <summary>
        /// Parametreleri listeleyen metot
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("/panel/param/list")]
        [HttpPost]
        [ProcessName(Name = "")]
        public Result<List<ParametreResult>> List(ParametreRequest request)
        {
            request.KurumId = Convert.ToInt32(cookieModel.KurumId);
            return _paramService.List(request);
        }

        /// <summary>
        /// Parametreleri listeleyen metot
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("/panel/param/listparam")]
        [HttpPost]
        [ProcessName(Name = "")]
        public Result<List<ParametreResult>> ListParam(ParametreRequest request)
        {
            request.KurumId = cookieModel == null ? 0 : Convert.ToInt32(cookieModel.KurumId);
            return _paramService.ListParam(request);
        }

        /// <summary>
        /// Parametreleri listeleyen metot
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("/panel/param/listparamDiller")]
        [HttpPost]
        [ProcessName(Name = "")]
        public Result<List<ParametreDilTanimlar>> ListParamDiller(ParametreRequest request)
        {
            request.KurumId = cookieModel == null ? 0 : Convert.ToInt32(cookieModel.KurumId);
            return _paramService.ListParamDiller(request);
        }


        /// <summary>
        /// Parametreleri listeleyen metot
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("/panel/param/saveparamDiller")]
        [HttpPost]
        [ProcessName(Name = "")]
        public Result<bool> saveparamDiller(ParametreDiller request) 
        { 
            request.KurumId = cookieModel == null ? 0 : Convert.ToInt32(cookieModel.KurumId);
            return _paramService.SaveParamDiller(request);
        }


        /// <summary>
        /// Parametreleri ekleyen metot
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [Route("/panel/param/add")]
        [HttpPost]
        [ProcessName(Name = "")]
        public Result<int> Add(ParametreRequest item)
        {
            item.KurumId = Convert.ToInt32(cookieModel.KurumId);
            return _paramService.Add(item);
        }

        /// <summary>
        /// Parametreleri güncelleyen metot
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [Route("/panel/param/update")]
        [HttpPost]
        [ProcessName(Name = "")]
        public Result<int> Update(ParametreRequest item)
        {
            item.KurumId = Convert.ToInt32(cookieModel.KurumId);
            var upd = _paramService.Update(item);
            return upd;
        }

        /// <summary>
        /// Parametreleri silen metot
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [Route("/panel/param/delete")]
        [HttpPost]
        [ProcessName(Name = "")]
        public Result<int> Delete(ParametreRequest item)
        {
            item.KurumId = Convert.ToInt32(cookieModel.KurumId);
            return _paramService.Delete(item);
        }

        
        #endregion Api Methods
    }
}