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
    /// Kişi İlişkileriyle ilgili metotların yer aldığı interface
    /// </summary>
    public interface IKisiIliskiService
    {
        /// <summary>
        /// Kişi ilişkilerini kaydeden servis metotu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<Iliskiler> KisiIliskiKaydet(KisiIliskiKayitModel model);

        /// <summary>
        /// Kişi ilişkilerini listeleyen servis metotu
        /// </summary>
        /// <param name="kurumID"></param>
        /// <returns></returns>
        Result<List<Iliskiler>> KisiIliskiList(int kurumID);

        /// <summary>
        /// Kisi ilişkilerini silindi yapan servis metotu
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        Result<bool> KisiIliskiSil(int tabloID);

        /// <summary>
        /// Kisi ilişkilerini güncelleyen servis metotu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<Iliskiler> KisiIliskiGuncelle(KisiIliskiKayitModel model);
    }

    /// <summary>
    ///  Kişi İlişkileriyle ilgili metotların yer aldığı servis sınıfı
    /// </summary>
    public class KisiIliskiService : IKisiIliskiService
    {
        private readonly IRequestHelper _requestHelper;
        private readonly string url = LocalPortlar.KurumService;
        private readonly IBazCookieService _bazCookieService;

        /// <summary>
        ///  Kişi İlişkileriyle ilgili metotların yer aldığı servis sınıfının yapıcı metodu
        /// </summary>
        /// <param name="requestHelper"></param>
        /// <param name="bazCookieService"></param>
        public KisiIliskiService(IRequestHelper requestHelper, IBazCookieService bazCookieService)
        {
            _requestHelper = requestHelper;
            _bazCookieService = bazCookieService;
        }

        /// <summary>
        /// Kişi ilişkilerini listeleyen servis metotu
        /// </summary>
        /// <param name="kurumID"></param>
        /// <returns></returns>
        public Result<List<Iliskiler>> KisiIliskiList(int kurumID)
        {
            var x = _requestHelper.Get<Result<List<Iliskiler>>>(LocalPortlar.KisiServis + "/api/KisiService/KisiIliskiList/" + kurumID);

            return x.Result;
        }

        /// <summary>
        /// Kişi ilişkilerini kaydeden servis metotu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<Iliskiler> KisiIliskiKaydet(KisiIliskiKayitModel model)
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            model.KurumID = Convert.ToInt32(cookie.KurumId);
            model.KayıtEdenID = Convert.ToInt32(cookie.KisiId);
            model.GuncelleyenKisiID = Convert.ToInt32(cookie.KisiId);
            var x = _requestHelper.Post<Result<Iliskiler>>(LocalPortlar.KisiServis + "/api/KisiService/KisiIliskiKaydet", model);

            return x.Result;
        }

        /// <summary>
        /// Kisi ilişkilerini silindi yapan servis metotu
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        public Result<bool> KisiIliskiSil(int tabloID)
        {
            var x = _requestHelper.Get<Result<bool>>(LocalPortlar.KisiServis + "/api/KisiService/KisiIliskiSil/" + tabloID);
            

            return x.Result;
        }

        /// <summary>
        /// Kisi ilişkilerini güncelleyen servis metotu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<Iliskiler> KisiIliskiGuncelle(KisiIliskiKayitModel model)
        {
            var x = _requestHelper.Post<Result<Iliskiler>>(LocalPortlar.KisiServis + "/api/KisiService/KisiIliskiGuncelle", model);

            return x.Result;
        }
    }
}