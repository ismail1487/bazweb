using Baz.Attributes;
using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using BazWebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BazWebApp.Controllers
{
    public class IcerikGirisController : Controller
    {
        private readonly IBazCookieService _bazCookieService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IKurumService _kurumService;
        private readonly IMedyaKutuphanesiService _medyaKutuphanesiService;
        public IcerikGirisController(ILogger<HomeController> logger, IMedyaKutuphanesiService medyaKutuphanesiService, IKurumService kurumService, IBazCookieService bazCookieService, ILocalizationService localizationService, IMenuService menuService, IServiceProvider serviceProvider)
        {
            _bazCookieService = bazCookieService;
            _serviceProvider = serviceProvider;
            _kurumService = kurumService;
            _medyaKutuphanesiService = medyaKutuphanesiService;
        }
        #region Views

        /// <summary>
        /// İçerik Girşlerinin Yapıldığı Arayüz
        /// </summary>
        /// <returns></returns>

        public IActionResult IcerikGirisEkrani()
        {
            return View("IcerikGirisEkrani");
        }
        /// <summary>
        /// İçerik Listelendiği Arayüz
        /// </summary>
        /// <returns></returns>

        public IActionResult IcerikListelemeEkrani()

        {
            return View("IcerikListelemeEkrani");
        }
        /// <summary>
        /// İçerik Listelendiği Arayüz
        /// </summary>
        /// <returns></returns>

        public IActionResult IcerikGuncelleme()

        {
            return View("IcerikGuncelleme");
        }
        #endregion Views

        #region API Methods

        /// <summary>
        /// İçeriklerin Kategorilerini Listeleyen Metod
        /// </summary>

        /// <returns></returns>
        [Route("/IcerikGiris/IcerikKategorileriGetir")]
        [HttpGet]
        public Result<List<ParamIcerikKategoriler>> IcerikKategorileriGetir()
        {
            var result = _kurumService.IcerikKategorileriGetir();
            return result;
        }

        /// <summary>
        /// İçerikleri oluşturduktan sonra kaydet butonuna tıklayınca kaydetme işini yapan metod
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/IcerikGiris/IcerikKaydet")]
        public Result<IcerikKutuphanesiMedyalarVM> IcerikKaydet([FromBody] IcerikKutuphanesiMedyalarVM model)
        {
            var result = _kurumService.IcerikKaydet(model);
            return result;

        }
        /// <summary>
        /// Kayıtlı İçerik Grup Başlıklarını Listeleyen Metod
        /// </summary>
        /// <returns></returns>
        [Route("/IcerikGiris/IcerikListesi")]
        [HttpGet]
        public Result<List<IcerikKutuphanesiMedyalarVM>> IcerikListesi()
        {
            var result = _kurumService.IcerikListesi();
            return result;
        }

        /// <summary>
        /// İçeriklerin Güncelleme Sayfasına Aktarılması
        /// </summary>
        ///// <returns></returns>
        [Route("/IcerikGiris/IcerikVeriGetir/{urlId}")]
        [HttpGet]
        public Result<List<IcerikKutuphanesiMedyalarVM>> IcerikVeriGetir(int urlId)
        {
            try
            {
                var result = _kurumService.IcerikVeriGetir(urlId);
                if(result.IsSuccess)
                {
                    return result;
                }
                return null;
            }
            catch
            {
                return null;
            }

        }
        /// <summary>
        /// İçeriklerin Güncellenmesi
        /// </summary>
        ///// <returns></returns>
        [Route("/IcerikGiris/IcerikGuncelle")]
        [HttpPost]
        public Result<IcerikKutuphanesiMedyalarVM> IcerikGuncelle([FromBody] IcerikKutuphanesiMedyalarVM model)
        {
            try
            {
                var result = _kurumService.IcerikGuncelle(model);
                if (result.IsSuccess)
                {
                    return result;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// İçerikleri Silme
        /// </summary>
        ///// <returns></returns>
        [Route("/IcerikGiris/IcerikSil/{TabloId}")]
        [HttpPost]
        public Result<bool> IcerikSil(int TabloId)
        {
            try
            {
                var result = _kurumService.IcerikSil(TabloId);
                if (result.IsSuccess)
                {
                    return true.ToResult();
                }
                return false.ToResult();
            }
            catch
            {
                return false.ToResult();
            }

        }

        #endregion API Methods
    }
}