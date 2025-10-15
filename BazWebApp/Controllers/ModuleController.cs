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
    /// Modül işlemlerinin yapıldığı controller
    /// </summary>
    public class ModuleController : Controller
    {
        private readonly IModulService _modulService;

        /// <summary>
        /// Modül işlemlerinin yapıldığı controller sınıfnın yapıcı metodu
        /// </summary>
        /// <param name="modulService"></param>
        public ModuleController(IModulService modulService)
        {
            _modulService = modulService;
        }

        #region view methods

        /// <summary>
        /// Modl tanımlamaya giden metod
        /// </summary>
        /// <returns></returns>
        [Route("/Module/Definition")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/Module/Definition")]
        public IActionResult ModuleDefinition()
        {
            return View("ModuleDefinition");
        }

        /// <summary>
        /// Modül güncellemeye  giden metod
        /// </summary>
        /// <returns></returns>
        [Route("/Module/Update/{id}")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/Module/Update/{id}")]
        public IActionResult ModuleUpdate()
        {
            return View("ModuleUpdate");
        }

        /// <summary>
        /// Modül listesine  giden metod
        /// </summary>
        /// <returns></returns>
        [Route("/Module/List")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/Module/List")]
        public IActionResult ModuleList()
        {
            return View("ModuleList");
        }

        #endregion view methods

        #region API methods

        /// <summary>
        /// Modül kayıt yada güncelleme
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/Module/AddOrUpdate")]
        public Result<ModulDetayKayitModel> AddOrUpdate([FromBody] ModulDetayKayitModel model)
        {
            var modulModel = new ModulViewModel()
            {
                Name = model.Name,
                TabloID = model.TabloID
            };
            var modulDetayModel = new ModulDetayViewModel()
            {
                SayfaId = model.SayfaId,
                ModulId = model.TabloID
            };

            var result = _modulService.ModulAddOrUpdate(modulModel);
            if (result.Value == null)
                return result.ToResult();

            modulDetayModel.ModulId = result.Value.TabloID;
            var detayResult = _modulService.ModulDetayAddOrUpdate(modulDetayModel);

            if (detayResult.Value)
            {
                model.TabloID = result.Value.TabloID;
                return model.ToResult();
            }
            return result.ToResult();
        }

        /// <summary>
        /// Modül ve datay getirme
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/Module/GetModulDetay/{id}")]
        public Result<ModulDetayKayitModel> GetModulveDetayData(int id)
        {
            var result = _modulService.GetModulveDetayData(id);
            return result;
        }

        /// <summary>
        /// Modül detay listeleme
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/Module/ModulListForView")]
        public Result<List<ModulDetayKayitModel>> ModulListForView(int id)
        {
            var result = _modulService.ModulListForView();
            return result;
        }

        /// <summary>
        /// Modül silme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/Module/Remove/{id}")]
        public Result<bool> Remove(int id)
        {
            var result = _modulService.Remove(id);
            return result;
        }

        #endregion API methods
    }
}