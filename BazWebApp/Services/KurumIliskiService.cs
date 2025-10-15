using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Baz.Model.Entity;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;

namespace BazWebApp.Services
{
    /// <summary>
    /// Kurumlar arası ilişkiye ait metodların bulunduğu interface
    /// </summary>
    public interface IKurumIliskiService
    {
        /// <summary>
        /// Kurum ilişkilerini kaydeden servis metotu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<Iliskiler> KurumIliskiKaydet(KurumIliskiKayitModel model);

        /// <summary>
        /// Kurum ilişkilerini listeleyen servis metotu
        /// </summary>
        /// <param name="kurumID"></param>
        /// <returns></returns>
        Result<List<Iliskiler>> KurumIliskiList(int kurumID);

        /// <summary>
        /// Kurum ilişkilerini silindi yapan servis metotu
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        Result<bool> KurumIliskiSil(int tabloID);

        /// <summary>
        /// Kurum ilişkilerini güncelleyen servis metotu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<Iliskiler> KurumIliskiGuncelle(KurumIliskiKayitModel model);
    }

    /// <summary>
    /// Kurumlar arası ilişkiye ait metodların bulunduğu servis sınıfı
    /// </summary>
    public class KurumIliskiService : IKurumIliskiService
    {
        private readonly IRequestHelper _requestHelper;
        private readonly string url = LocalPortlar.KurumService;
        private readonly IBazCookieService _bazCookieService;

        /// <summary>
        /// Kurumlar arası ilişkiye ait metodların bulunduğu servis sınıfının yapıcı metodu
        /// </summary>
        /// <param name="requestHelper"></param>
        /// <param name="bazCookieService"></param>
        public KurumIliskiService(IRequestHelper requestHelper, IBazCookieService bazCookieService)
        {
            _requestHelper = requestHelper;
            _bazCookieService = bazCookieService;
        }

        /// <summary>
        /// Kurum ilişkilerini kaydeden servis metotu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<Iliskiler> KurumIliskiKaydet(KurumIliskiKayitModel model)
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            model.KurumID = Convert.ToInt32(cookie.KurumId);
            model.KayitEdenID = Convert.ToInt32(cookie.KisiId);
            model.GuncelleyenKisiID = Convert.ToInt32(cookie.KisiId);
            var x = _requestHelper.Post<Result<Iliskiler>>(url + "/api/KurumService/KurumIliskiKaydet", model);
            return x.Result;
        }

        /// <summary>
        /// Kurum ilişkilerini listeleyen servis metotu
        /// </summary>
        /// <param name="kurumID"></param>
        /// <returns></returns>
        public Result<List<Iliskiler>> KurumIliskiList(int kurumID)
        {
            var x = _requestHelper.Get<Result<List<Iliskiler>>>(LocalPortlar.KurumService + "/api/KurumService/KurumIliskiList/" + kurumID);
            return x.Result;
        }

        /// <summary>
        /// Kurum ilişkilerini silindi yapan servis metotu
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        public Result<bool> KurumIliskiSil(int tabloID)
        {
            var x = _requestHelper.Get<Result<bool>>(LocalPortlar.KurumService + "/api/KurumService/KurumIliskiSil/" + tabloID);
            return x.Result;
        }

        /// <summary>
        /// Kurum ilişkilerini güncelleyen servis metotu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<Iliskiler> KurumIliskiGuncelle(KurumIliskiKayitModel model)
        {
            var x = _requestHelper.Post<Result<Iliskiler>>(url + "/api/KurumService/KurumIliskiGuncelle", model);
            return x.Result;
        }
    }
}