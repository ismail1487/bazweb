using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using BazWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Controllers
{
    /// <summary>
    /// Coğrafya işlemlerini yöneten controller class'ı.
    /// </summary>
    public class GeographyController : Controller
    {
        private readonly ICografyaService _cografyaService;

        /// <summary>
        /// Coğrafya işlemlerini yöneten controller sınıfının yapıcı metodu
        /// </summary>
        /// <param name="cografyaService"></param>
        public GeographyController(ICografyaService cografyaService)
        {
            _cografyaService = cografyaService;
        }

        //Coğrafya sayfa methodları yer alır

        #region View Methods

        /// <summary>
        /// Coğrafya listesi sayfasına yönlendiren metod
        /// </summary>
        /// <returns></returns>
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/Geography/List")]
        [Route("/Geography/List")]
        public IActionResult CografyaAyrintiListesi()
        {
            return View("CografyaAyrintiListesi");
        }

        /// <summary>
        /// Coğrafya tanımlama sayfasına yönlendiren metod
        /// </summary>
        /// <returns></returns>
        [Route("/Geography/Definition")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/Geography/Definition")]
        public IActionResult CografyaAyrintiKayit()
        {
            return View("CografyaAyrintiKayit");
        }

        /// <summary>
        /// Coğrafya güncelleme sayfasına yönledirien metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("/Geography/Update/{id}")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/Geography/Update/{id}")]
        public IActionResult CografyaAyrintiGuncelle(int id)
        {
            return View("CografyaAyrintiGuncelle", id);
        }

        #endregion View Methods

        //Coğrafya api methodları yer alır

        #region WEB API Methods

      

        #region Coğrafya Ayrıntı WEB API Methodları

        
        /// <summary>
        /// Coğrafya Listeleme
        /// </summary>
        /// <returns></returns>
        [Route("Geography/ListForView")]
        [HttpGet]
        public Result<List<CografyaListViewModel>> CografyaTanimListeleme()
        {
            var result = _cografyaService.CografyaTanimListesiGetir();
            return result;
        }

     
        #endregion Coğrafya Ayrıntı WEB API Methodları

        #region CoğrafyaKütüphanesi WEB API methodları

        /// <summary>
        /// Coğrafya Tanım kayıt
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Geography/Save")]
        [HttpPost]
        public Result<int> CografyaTanimKayit([FromBody] CografyaListViewModel model)
        {
            var result = _cografyaService.CografyaTanimKayit(model);
            return result;
        }

        /// <summary>
        /// Coğrafya Tanım Silme
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("Geography/Delete/{id}")]
        [HttpGet]
        public Result<bool> CografyaTanimSil(int id)
        {
            var result = _cografyaService.CografyaTanimSil(id);
            return result;
        }

        /// <summary>
        /// Coğrafya Tanım id'sine göre getirme
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("Geography/Get/{id}")]
        [HttpGet]
        public Result<CografyaListViewModel> CografyaTanimIdsineGoreGetir(int id)
        {
            var result = _cografyaService.CografyaTanimIdyeGoreGetir(id);
            return result;
        }

        /// <summary>
        /// Coğrafya Tanım Güncelleme
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Geography/UpdateForView")]
        [HttpPost]
        public Result<CografyaKutuphanesi> CografyaTanimGuncelle([FromBody] CografyaListViewModel model)
        {
            var result = _cografyaService.CografyaTanimGuncelle(model);
            return result;
        }

        #endregion CoğrafyaKütüphanesi WEB API methodları

        #endregion WEB API Methods
    }
}