using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Baz.Model.Entity.Constants;

namespace BazWebApp.Services
{
    /// <summary>
    /// Organizasyon birimlerine ait listeleme metotlarının yer aldığı servis interface
    /// </summary>
    public interface IParamOrganizasyonBirimleriService
    {
        /// <summary>
        /// Organizasyon birimleri listeleme metodu
        /// </summary>
        /// <returns></returns>
        public Result<List<Baz.Model.Entity.ViewModel.ParamBirimTanimView>> List();

        /// <summary>
        /// Kurum organizasyon birim tipini listeleyen metod
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<KurumOrganizasyonBirimView>> ListTip(KurumOrganizasyonBirimRequest request);

        /// <summary>
        /// Kurum organizasyon Id ile kişi listeleme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<List<int>> ListKurumOrganizasyonIdileKisiListele(List<int> model);
    }

    /// <summary>
    /// Param organizasyon birimleri servis sınıfı
    /// </summary>
    public class ParamOrganizasyonBirimleriService : IParamOrganizasyonBirimleriService
    {
        private readonly IRequestHelper _requestHelper;
        private readonly IBazCookieService _bazCookie;

        /// <summary>
        /// Param organizasyon birimleri servis sınıfı konst.
        /// </summary>
        /// <param name="requestHelper"></param>
        /// <param name="bazCookie"></param>
        public ParamOrganizasyonBirimleriService(IRequestHelper requestHelper, IBazCookieService bazCookie)
        {
            _bazCookie = bazCookie;
            _requestHelper = requestHelper;
        }

        /// <summary>
        /// Organizasyon birimleri listeleme metotu
        /// </summary>
        /// <returns></returns>
        public Result<List<Baz.Model.Entity.ViewModel.ParamBirimTanimView>> List()
        {
            var url = LocalPortlar.IYSService + "/api/ParamOrganizasyonBirimTanim/ListForView/";
            var x = _requestHelper.Get<Result<List<Baz.Model.Entity.ViewModel.ParamBirimTanimView>>>(url);
            return x.Result;
        }

        /// <summary>
        /// Organizasyon birimleri listeleme metotu
        /// </summary>
        /// <returns></returns>
        public Result<List<int>> ListKurumOrganizasyonIdileKisiListele(List<int> model)
        {
            var url = Baz.Model.Entity.Constants.LocalPortlar.KurumService + "/api/KurumService/ListKurumOrganizasyonIdileKisiListele/";
            var x = _requestHelper.Post<Result<List<int>>>(url, model);

            return x.Result;
        }

        /// <summary>
        /// Organizasyon birimlerini tipe göre listeleme metotu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<KurumOrganizasyonBirimView>> ListTip(KurumOrganizasyonBirimRequest request)
        {
            if (request.KurumId == 0)
            {
                var cookie = _bazCookie.GetCookie().GetAwaiter().GetResult();
                request.KurumId = Convert.ToInt32(cookie.KurumId);
            }
            var url = LocalPortlar.UserLoginregisterService + "/api/LoginRegister/ListTip";
            var x = _requestHelper.Post<Result<List<KurumOrganizasyonBirimView>>>(url, request);

            return x.Result;
        }
    }
}