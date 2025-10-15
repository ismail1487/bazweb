using Baz.Model.Entity.Constants;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using System.Collections.Generic;

namespace BazWebApp.Services
{
    /// <summary>
    /// ParamDis Platform Interface Alanı
    /// </summary>
    public interface IParamDisPlatformlarService
    {
        /// <summary>
        /// ParamDisPlatform Listeleme Methodu Tanımı
        /// </summary>
        /// <returns></returns>
        public Result<List<Baz.Model.Entity.ViewModel.ParamBirimTanimView>> List();
    }
    /// <summary>
    /// ParamDis Platform Controller Alanı
    /// </summary>
    public class ParamDisPlatformlarService : IParamDisPlatformlarService
    {
        private readonly IRequestHelper _requestHelper;
        private readonly IBazCookieService _bazCookie;

        /// <summary>
        /// Param DisPlatform birimleri servis sınıfı konst.
        /// </summary>
        /// <param name="requestHelper"></param>
        /// <param name="bazCookie"></param>
        public ParamDisPlatformlarService(IRequestHelper requestHelper, IBazCookieService bazCookie)
        {
            _bazCookie = bazCookie;
            _requestHelper = requestHelper;
        }

        /// <summary>
        /// ParamDisPlatform Listeleme Methodu Iceigi
        /// </summary>
        /// <returns></returns>
        public Result<List<Baz.Model.Entity.ViewModel.ParamBirimTanimView>> List()
        {
            var url = LocalPortlar.IYSService + "/api/ParamDisPlatformlar/ListForView";
            var x = _requestHelper.Get<Result<List<Baz.Model.Entity.ViewModel.ParamBirimTanimView>>>(url);
            return x.Result;
        }

    }
}