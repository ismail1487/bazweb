using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using BazWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BazWebApp.Controllers
{
    public class SliderController : Controller
    {

        #region Sevisler
        private readonly IBazCookieService _bazCookieService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IKurumService _kurumService;
        private readonly IMedyaKutuphanesiService _medyaKutuphanesiService;
        public SliderController(ILogger<HomeController> logger, IMedyaKutuphanesiService medyaKutuphanesiService, IKurumService kurumService, IBazCookieService bazCookieService, ILocalizationService localizationService, IMenuService menuService, IServiceProvider serviceProvider)
        {
            _bazCookieService = bazCookieService;
            _serviceProvider = serviceProvider;
            _kurumService = kurumService;
            _medyaKutuphanesiService = medyaKutuphanesiService;
        }
        #endregion
        #region Görünümler
        public IActionResult SliderGiris()
        {
            return View("SliderGiris");
        }
        public IActionResult SliderGuncelleme()
        {
            return View("SliderGuncelleme");
        }

        public IActionResult SliderListeleme()
        {
            return View("SliderListeleme");
        }
        #endregion

        #region Servis Yönlendiriciler

        /// <summary>
        /// Ürünleri Listeleyen Metod
        /// </summary>

        /// <returns></returns>

        [Route("/Slider/SliderListesi")]
        [HttpGet]
        public Result<List<SliderTemelBilgilerMedyalarVM>> SliderListesi()
        {
            var result = _kurumService.SliderListesi();
            return result;
        }

        /// <summary>
        /// Ürünleri Silme
        /// </summary>
        ///// <returns></returns>
        [Route("/Slider/SliderSil/{TabloId}")]
        [HttpPost]
        public Result<bool> SliderSil(int TabloId)
        {
            try
            {
                var result = _kurumService.SliderSil(TabloId);
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
        /// Urun Kayıteden Metod
        /// </summary>

        /// <returns></returns>
        [HttpPost]
        /*      [ValidateAntiForgeryToken] */ /// Normal Yapıdan Farklı Olarak Eklenmiştir. Eklenecek yeni yapıda kullanılmadan önce 'application/xxx' Content tipinin CSRF Objesi olmadığı zamanda kullanmayın.
        [Route("/Slider/SliderKaydet")]
        public Result<SliderTemelBilgilerMedyalarVM> SliderKaydet([FromBody] SliderTemelBilgilerMedyalarVM model)
        {
            try
            {
                var result = _kurumService.SliderKaydet(model);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Urun Guncelleyen Metod
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        /*      [ValidateAntiForgeryToken] */ /// Normal Yapıdan Farklı Olarak Eklenmiştir. Eklenecek yeni yapıda kullanılmadan önce 'application/xxx' Content tipinin CSRF Objesi olmadığı zamanda kullanmayın.
        [Route("/Slider/SliderGuncelle")]
        public Result<SliderTemelBilgilerMedyalarVM> SliderGuncelle([FromBody] SliderTemelBilgilerMedyalarVM model)
        {
            try
            {
                var result = _kurumService.SliderGuncelle(model);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Ürün Güncelleme Sayfasına Aktarılması
        /// </summary>
        ///// <returns></returns>
        [Route("/Slider/SliderVeriGetir/{urlId}")]
        [HttpGet]
        public Result<List<SliderTemelBilgilerMedyalarVM>> SliderVeriGetir(int urlId)
        {
            try
            {
                var result = _kurumService.SliderVeriGetir(urlId);
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
        #endregion
    }
}
