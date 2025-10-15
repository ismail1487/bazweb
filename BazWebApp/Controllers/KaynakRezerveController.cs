using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using BazWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BazWebApp.Controllers
{
    public class KaynakRezerveController : Controller
    {
        #region Sevisler
        private readonly IBazCookieService _bazCookieService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IKurumService _kurumService;
        private readonly IMedyaKutuphanesiService _medyaKutuphanesiService;
        private readonly IIYSService _iysService;
        public KaynakRezerveController(ILogger<HomeController> logger, IMedyaKutuphanesiService medyaKutuphanesiService, IKurumService kurumService, IBazCookieService bazCookieService, ILocalizationService localizationService, IMenuService menuService, IServiceProvider serviceProvider, IIYSService iysService)
        {
            _bazCookieService = bazCookieService;
            _serviceProvider = serviceProvider;
            _kurumService = kurumService;
            _medyaKutuphanesiService = medyaKutuphanesiService;
            _iysService = iysService;
        }
        #endregion

        #region Görünümler
        /// <summary>
        /// Urun Girşlerinin Yapıldığı Arayüz
        /// </summary>
        /// <returns></returns>

        public IActionResult KaynakRezerveGiris()
        {
            return View("KaynakRezerveGiris");
        }

        public IActionResult KaynakRezerveListesi()
        {
            return View("KaynakRezerveListesi");
        }

        public IActionResult KaynakRezerveGuncelleme()
        {
            return View("KaynakRezerveGuncelleme");
        }

        public IActionResult KaynakRezerveTakvim()
        {
            return View("KaynakRezerveTakvim");
        }
        #endregion

        #region Servis Yönlendiriciler
        [Route("/KaynakRezerve/KaynakTipiGetir")]
        [HttpGet]
        public Result<List<ParamKaynakTipleri>> KaynakTipiGetir()
        {
            var result = _kurumService.KaynakTipiGetir();
            return result;
        }

        [Route("/KaynakRezerve/KaynakRezerveVeriGetir/{urlId}")]
        [HttpGet]
        public Result<List<KaynakTanimlariRezerveVM>> KaynakRezerveVeriGetir(int urlId)
        {
            var result = _kurumService.KaynakRezerveVeriGetir(urlId);
            return result;
        }
        [Route("/KaynakRezerve/KaynakRezerveListele")]
        [HttpGet]
        public Result<List<KaynakTanimlariRezerveVM>> KaynakRezerveListele()
        {
            var result = _kurumService.KaynakRezerveListele();
            return result; 
        }
        [Route("/KaynakRezerve/RezerveKaynakGuncelle")]
        [HttpPost]
        public Result<KaynakTanimlariRezerveVM> RezerveKaynakGuncelle([FromBody] KaynakTanimlariRezerveVM model)
        {
            var result = _kurumService.RezerveKaynakGuncelle(model);
            return result;
        }
        [Route("/KaynakRezerve/KaynakRezerveKayit")]
        [HttpPost]
        public Result<KaynakTanimlariRezerveVM> KaynakRezerveKayit([FromBody] KaynakTanimlariRezerveVM model)
        {
            var result = _kurumService.KaynakRezerveKayit(model);
            return result;
        }

        [Route("/KaynakRezerve/SilKaynak/{id}")]
        [HttpPost]
        public Result<bool> SilKaynak(int id)
        {
            var result = _kurumService.SilKaynak(id);
            return result;
        }

        #region Takvim
        [Route("/KaynakRezerve/KaynakRezerveTakvimVeriGetir")]
        [HttpGet]
        public Result<List<KaynakTanimlariRezerveVM>> KaynakRezerveTakvimVeriGetir()
        {
            var result = _iysService.KaynakRezerveTakvimVeriGetir();
            return result;
        }

        [Route("/KaynakRezerve/KaynakSelectVeriGetir")]
        [HttpPost]
        public IActionResult KaynakSelectVeriGetir([FromBody] List<int> id)
        {
            var result = _iysService.KaynakSelectVeriGetir(id);
            return Ok(result);
        }

        [Route("/KaynakRezerve/EventKaydet")]
        [HttpPost]
        public Result<KaynakRezervasyonCariDegerlerVM> EventKaydet([FromBody] KaynakRezervasyonCariDegerlerVM model)
        {
           var result = _iysService.EventKaydet(model);
           return result;
        }

        /// <summary>
        /// EVent Guncelleme
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("/KaynakRezerve/EventGuncelle")]
        [HttpPost]
        public Result<KaynakRezervasyonCariDegerlerVM> EventGuncelle([FromBody] KaynakRezervasyonCariDegerlerVM model)
        {
            var result = _iysService.EventGuncelle(model);
            return result;
        }
        /// <summary>
        /// Event Listeleme
        /// </summary>
        /// <returns></returns>
        [Route("/KaynakRezerve/EventListele")]
        [HttpGet]
        public Result<List<KaynakRezervasyonCariDegerlerVM>> EventListele()
        {
            var result = _iysService.EventListele();
            return result;
        }
        /// <summary>
        /// Event Veri DOldurma
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("/KaynakRezerve/EventVeriGetir/{id}")]
        [HttpGet]
        public Result<List<KaynakRezervasyonCariDegerlerVM>> EventVeriGetir(int id)
        {
            var result = _iysService.EventVeriGetir(id);
            return result;
        }

        [Route("/KaynakRezerve/EventSil/{id}")]
        [HttpPost]
        public Result<bool> EventSil(int id)
        {
            var result = _iysService.EventSil(id);
            return result;
        }
        #endregion
        #endregion
    }
}
