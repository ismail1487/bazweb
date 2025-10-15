using Baz.Model.Entity;
using Baz.ProcessResult;
using BazWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baz.Attributes;
using Baz.Model.Entity.ViewModel;

namespace BazWebApp.Controllers
{
    /// <summary>
    /// Kişi işlemlerinin yapıldığı controller
    /// </summary>
    public class KisiController : Controller
    {
        private readonly IBazCookieService _bazCookieService;
        private readonly IKisiService _kisiService;

        /// <summary>
        ///  Kişi işlemlerinin yapıldığı controller sınıfının yapıcı metodu
        /// </summary>
        /// <param name="bazCookieService"></param>
        /// <param name="kisiService"></param>
        public KisiController(IBazCookieService bazCookieService, IKisiService kisiService)
        {
            _bazCookieService = bazCookieService;
            _kisiService = kisiService;
        }

        #region API Methods

        /// <summary>
        /// paramtreye göre kuruma bağlı kişileri getiren metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("/kisi/KurumaBagliKisiListGetirById/{id}")]
        [HttpGet]
        public Result<List<KisiTemelBilgiler>> KurumaBagliKisiListGetirById(int id)
        {
            var result = _kisiService.KurumaBagliKisiListGetirById(id);
            return result;
        }

        /// <summary>
        ///  AktifPasifKisiList getiren metod
        /// </summary>
        /// <param name="eposta"></param>
        /// <returns></returns>
        [Route("/kisi/AktifPasifKisiList/{eposta}")]
        [HttpGet]
        public Result<bool> AktifPasifKisiList(string eposta)
        {
            var rs = _kisiService.AktifPasifKisiList();
            var sr = rs.Value.Any(x => x.KisiEposta == eposta);
            if (sr)
            {
                return false.ToResult();
            }
            return true.ToResult();
        }

        /// <summary>
        ///  AktifPasifKisiList getiren metod update
        /// </summary>
        /// <param name="eposta"></param>
        /// <param name="tabloid"></param>
        /// <returns></returns>
        [Route("/kisi/AktifPasifKisiList1/{eposta}/{tabloid}")]
        [HttpGet]
        public Result<bool> AktifPasifKisiList1(string eposta, int tabloid)
        {
            var rs = _kisiService.AktifPasifKisiList();
            var sr = rs.Value.Any(x => x.TabloID != tabloid && x.KisiEposta == eposta);
            if (sr)
            {
                return false.ToResult();
            }
            return true.ToResult();
        }

        /// <summary>
        /// Kuruma bağlı kişilerin listesini getiren metot.
        /// </summary>
        /// <returns>kişiler listesini döndürür.</returns>
        [Route("/Kisi/KisiListGetir")]
        [HttpGet]
        public Result<List<KisiTemelBilgiler>> KisiListGetir()
        {
            return _kisiService.KurumaBagliKisiListGetir();
        }

        #endregion API Methods
    }
}