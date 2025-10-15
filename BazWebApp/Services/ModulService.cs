using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager;
using Baz.RequestManager.Abstracts;
using BazWebApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Services
{
    /// <summary>
    /// Modül servis sınıfı interface i
    /// </summary>
    public interface IModulService
    {
        /// <summary>
        /// Modül ekleme güncelleme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<ModulViewModel> ModulAddOrUpdate(ModulViewModel model);

        /// <summary>
        /// Id'ye göre modül detay getirme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<ModulDetayKayitModel> GetModulveDetayData(int id);

        /// <summary>
        /// Modül listeleme metodu
        /// </summary>
        /// <returns></returns>
        public Result<List<ModulDetayKayitModel>> ModulListForView();

        /// <summary>
        /// Modül detay ekleme güncelleme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> ModulDetayAddOrUpdate(ModulDetayViewModel model);

        /// <summary>
        /// Modül silme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<bool> Remove(int id);
    }

    /// <summary>
    /// Modül servis sınıfı
    /// </summary>
    public class ModulService : IModulService
    {
        private readonly IRequestHelper _requestHelper;

        /// <summary>
        /// Modul servis sınıfı konst.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ModulService(IServiceProvider serviceProvider)
        {
            _requestHelper = new RequestHelper(LocalPortlar.LisansServis + "/api/", new RequestManagerHeaderHelperForCookie(serviceProvider).SetDefaultHeader());
        }

        /// <summary>
        /// Modül ekleme güncelleme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<ModulViewModel> ModulAddOrUpdate(ModulViewModel model)
        {
            var result = _requestHelper.Post<Result<ModulViewModel>>("Modul/AddOrUpdate", model);
            return result.Result;
        }

        /// <summary>
        /// Id'ye göre modül detay getiren metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<ModulDetayKayitModel> GetModulveDetayData(int id)
        {
            var result = _requestHelper.Get<Result<ModulDetayKayitModel>>("Modul/GetModulDetay/" + id);
            return result.Result;
        }

        /// <summary>
        /// Modul detayları listeleme metodu
        /// </summary>
        /// <returns></returns>
        public Result<List<ModulDetayKayitModel>> ModulListForView()
        {
            var result = _requestHelper.Get<Result<List<ModulDetayKayitModel>>>("Modul/List");
            return result.Result;
        }

        /// <summary>
        /// Modül detay ekleme güncelleme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> ModulDetayAddOrUpdate(ModulDetayViewModel model)
        {
            var result = _requestHelper.Post<Result<bool>>("Modul/Detay/AddOrUpdate", model);
            return result.Result;
        }

        /// <summary>
        /// Modul silme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<bool> Remove(int id)
        {
            var result = _requestHelper.Get<Result<bool>>("Modul/Remove/" + id);
            return result.Result;
        }
    }
}