using Baz.Model.Entity;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BazWebApp.Services
{
    /// <summary>
    /// Coğrafya ile  ilgili metotların yer aldığı interface sınıfıdır
    /// </summary>
    public interface ICografyaService
    {
        /// <summary>
        /// Coğrafya tanım ekleme
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<int> CografyaTanimKayit(CografyaListViewModel model);

        /// <summary>
        /// Coğrafya Tanım Listesi getiren method
        /// </summary>
        /// <returns></returns>
        Result<List<CografyaListViewModel>> CografyaTanimListesiGetir();

        /// <summary>
        /// Coğrafta Tanım güncelleme
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<CografyaKutuphanesi> CografyaTanimGuncelle(CografyaListViewModel model);

        /// <summary>
        /// Coğrafya tanım id'sine göre getiren method
        /// </summary>
        /// <param name="cografyaKutuphanesiId"></param>
        /// <returns></returns>
        Result<CografyaListViewModel> CografyaTanimIdyeGoreGetir(int cografyaKutuphanesiId);

        /// <summary>
        /// Coğrafya Tanım Silme methodu
        /// </summary>
        /// <param name="cografyaKutuphanesiId"></param>
        /// <returns></returns>
        Result<bool> CografyaTanimSil(int cografyaKutuphanesiId);
    }

    /// <summary>
    /// Coğrafya ile  ilgili metotların yer aldığı servis sınıfıdır
    /// </summary>
    public class CografyaService : ICografyaService
    {
        private readonly IRequestHelper _requestHelper;
        private readonly IBazCookieService _bazCookieService;

        /// <summary>
        /// Coğrafya ile  ilgili metotların yer aldığı servis sınıfının yapıcı methodu
        /// </summary>
        /// <param name="requestHelper"></param>
        /// <param name="bazCookieService"></param>
        public CografyaService(IRequestHelper requestHelper, IBazCookieService bazCookieService)
        {
            _requestHelper = requestHelper;
            _bazCookieService = bazCookieService;
        }

        #region CografyaKütüphanesi

        /// <summary>
        /// Coğrafya tanım ekleme
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> CografyaTanimKayit(CografyaListViewModel model)
        {
            var response =
                _requestHelper.Post<Result<int>>(LocalPortlar.IYSService + "/api/CografyaTanim/CografyaTanimKayit", model);
            return response.Result;
        }

        /// <summary>
        /// Coğrafya Tanım Listesi getiren method
        /// </summary>
        /// <returns></returns>
        public Result<List<CografyaListViewModel>> CografyaTanimListesiGetir()
        {
            var response = _requestHelper.Get<Result<List<CografyaListViewModel>>>(LocalPortlar.IYSService + "/api/CografyaTanim/CografyaTanimListeleme");
            return response.Result;
        }

        /// <summary>
        /// Coğrafta Tanım güncelleme
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<CografyaKutuphanesi> CografyaTanimGuncelle(CografyaListViewModel model)
        {
            var response = _requestHelper.Post<Result<CografyaKutuphanesi>>(LocalPortlar.IYSService + "/api/CografyaTanim/CografyaTanimGuncelle", model);
            return response.Result;
        }

        /// <summary>
        /// Coğrafya tanım id'sine göre getiren method
        /// </summary>
        /// <param name="cografyaKutuphanesiId"></param>
        /// <returns></returns>
        public Result<CografyaListViewModel> CografyaTanimIdyeGoreGetir(int cografyaKutuphanesiId)
        {
            var response = _requestHelper.Get<Result<CografyaListViewModel>>(LocalPortlar.IYSService + "/api/CografyaTanim/CografyaTanimIdyeGoreGetir/" + cografyaKutuphanesiId);
            return response.Result;
        }

        /// <summary>
        /// Coğrafya Tanım Silme methodu
        /// </summary>
        /// <param name="cografyaKutuphanesiId"></param>
        /// <returns></returns>
        public Result<bool> CografyaTanimSil(int cografyaKutuphanesiId)
        {
            var response = _requestHelper.Get<Result<bool>>(LocalPortlar.IYSService + "/api/CografyaTanim/CografyaTanimSil/" + cografyaKutuphanesiId);
            return response.Result;
        }

        #endregion CografyaKütüphanesi
    }
}