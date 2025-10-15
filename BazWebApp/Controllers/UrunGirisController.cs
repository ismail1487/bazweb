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

    public class UrunGirisController : Controller
    {
        #region Sevisler
        private readonly IBazCookieService _bazCookieService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IKurumService _kurumService;
        private readonly IMedyaKutuphanesiService _medyaKutuphanesiService;
        public UrunGirisController(ILogger<HomeController> logger, IMedyaKutuphanesiService medyaKutuphanesiService, IKurumService kurumService, IBazCookieService bazCookieService, ILocalizationService localizationService, IMenuService menuService, IServiceProvider serviceProvider)
        {
            _bazCookieService = bazCookieService;
            _serviceProvider = serviceProvider;
            _kurumService = kurumService;
            _medyaKutuphanesiService = medyaKutuphanesiService;
        }
        #endregion

        #region Görünümler
        /// <summary>
        /// Urun Girşlerinin Yapıldığı Arayüz
        /// </summary>
        /// <returns></returns>

        public IActionResult UrunGirisEkrani()
        {
            return View("UrunGirisEkrani");
        }

        public IActionResult UrunGirisListesi()
        {
            return View("UrunGirisListesi");
        }

        public IActionResult UrunGuncellemeEkrani()
        {
            return View("UrunGuncellemeEkrani");
        }

        #endregion

        #region Servis Yönlendiriciler

        /// <summary>
        /// Ürün Güncelleme Sayfasına Aktarılması
        /// </summary>
        ///// <returns></returns>
        [Route("/UrunGiris/UrunVeriGetir/{urlId}")]
        [HttpGet]
        public Result<List<UrunKutuphanesiMedyalarVM>> UrunVeriGetir(int urlId)
        {
            try
            {
                var result = _kurumService.UrunVeriGetir(urlId);
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
        /// Urun Kayıteden Metod
        /// </summary>

        /// <returns></returns>
        [HttpPost]
        /*      [ValidateAntiForgeryToken] */ /// Normal Yapıdan Farklı Olarak Eklenmiştir. Eklenecek yeni yapıda kullanılmadan önce 'application/xxx' Content tipinin CSRF Objesi olmadığı zamanda kullanmayın.
        [Route("/UrunGiris/UrunKaydet")]
        public Result<UrunKutuphanesiMedyalarVM> UrunKaydet([FromBody] UrunKutuphanesiMedyalarVM model)
        {
            try
            {
                var result = _kurumService.UrunKaydet(model);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Ürün Kategorilerini Listeleyen Metod
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("/UrunGiris/UrunKategorileriGetir")]
        [HttpGet]
        public Result<List<ParamUrunKategoriler>> UrunKategorileriGetir()
        {
            var result = _kurumService.ParamUrunKategorileriGetir();
            return result;
        }

        /// <summary>
        /// ParaBirimi Kategorilerini Listeleyen Metod
        /// </summary>

        /// <returns></returns>
        [Route("/UrunGiris/UrunParaBirimiGetir")]
        [HttpGet]
        public Result<List<ParamParaBirimleri>> UrunParaBirimiGetir()
        {
            var result = _kurumService.UrunParaBirimiGetir();
            return result;
        }
        /// <summary>
        /// Ürünleri Listeleyen Metod
        /// </summary>

        /// <returns></returns>

        [Route("/UrunGiris/UrunListesi")]
        [HttpGet]
        public Result<List<UrunKutuphanesiMedyalarVM>> UrunListesi()
        {
            var result = _kurumService.UrunListesi();
            return result;
        }

        /// <summary>
        /// Ürünleri Silme
        /// </summary>
        ///// <returns></returns>
        [Route("/UrunGiris/UrunSil/{TabloId}")]
        [HttpPost]
        public Result<bool> UrunSil(int TabloId)
        {
            try
            {
                var result = _kurumService.UrunSil(TabloId);
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

        /// <summary>
        /// Urun Guncelleyen Metod
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        /*      [ValidateAntiForgeryToken] */ /// Normal Yapıdan Farklı Olarak Eklenmiştir. Eklenecek yeni yapıda kullanılmadan önce 'application/xxx' Content tipinin CSRF Objesi olmadığı zamanda kullanmayın.
        [Route("/UrunGiris/UrunGuncelle")]
        public Result<UrunKutuphanesiMedyalarVM> UrunGuncelle([FromBody] UrunKutuphanesiMedyalarVM model)
        {
            try
            {
                var result = _kurumService.UrunGuncelle(model);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Ürün verilerini Listeleyen Metod
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("/UrunGiris/UrunParametreDegerGetir")]
        [HttpGet]
        public Result<List<ParamOlcumBirimleri>> UrunParametreDegerGetir()
        {
            var result = _kurumService.UrunParametreDegerGetir();
            return result;
        }


        /// <summary>
        /// Ürün verilerini Listeleyen Metod
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("/UrunGiris/ParamUrunMarkalarınıGetir")]
        [HttpGet]
        public Result<List<ParamUrunMarkalar>> ParamUrunMarkalarınıGetir()
        {
            var result = _kurumService.ParamUrunMarkalarınıGetir();
            return result;
        }

        /// <summary>
        /// Ürün verilerini Listeleyen Metod
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("/UrunGiris/UrunOlcumBirimiData")]
        [HttpGet]
        public Result<List<ParamOlcumBirimleri>> UrunOlcumBirimiData()
        {
            var result = _kurumService.UrunOlcumBirimiData();
            return result;
        }

        /// <summary>
        /// Ürün verilerini Listeleyen Metod
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("/UrunGiris/IcerikBlokKategorilerGetir")]
        [HttpGet]
        public Result<List<ParamIcerikBlokKategorileri>> IcerikBlokKategorilerGetir()
        {
            var result = _kurumService.IcerikBlokKategorilerGetir();
            return result;
        }

        /// <summary>
        /// IçerikKutuphanesi verilerini Listeleyen Metod
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("/UrunGiris/IcerikKutuphanesiGetir")]
        [HttpGet]
        public Result<List<IcerikKutuphanesi>> IcerikKutuphanesiGetir()
        {
            var result = _kurumService.IcerikKutuphanesiGetir();
            return result;
        }
        #endregion

    }
}
