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
    /// Sistem Smtp Değerleri için oluşturulan interface'dir
    /// </summary>
    public interface ISmtpService
    {
        /// <summary>
        /// smtp kaydetme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<SistemSMTPDegerleri> SmtpKaydet(SistemSMTPDegerleri model);

        /// <summary>
        /// smtp listeleme metodu
        /// </summary>
        /// <returns></returns>
        Result<List<SistemSMTPDegerleri>> SmtpList();

        /// <summary>
        /// smtp silme metodu
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        Result<bool> SmtpSil(int tabloID);

        /// <summary>
        /// smtp güncelleme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<SistemSMTPDegerleri> SmtpGuncelle(SistemSMTPDegerleri model);

        /// <summary>
        /// smtp getirme metodu
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        Result<SistemSMTPDegerleri> SmtpGetir(int tabloID);
    }

    /// <summary>
    /// Smtp servis sınıfı
    /// </summary>
    public class SmtpService : ISmtpService
    {
        private readonly IRequestHelper _requestHelper;
        private readonly string url = LocalPortlar.SMTPService;
        private readonly IBazCookieService _bazCookieService;

        /// <summary>
        /// Smtp servis sınıfı konst.
        /// </summary>
        /// <param name="requestHelper"></param>
        /// <param name="bazCookieService"></param>
        public SmtpService(IRequestHelper requestHelper, IBazCookieService bazCookieService)
        {
            _requestHelper = requestHelper;
            _bazCookieService = bazCookieService;
        }

        /// <summary>
        /// Tablo Id'ye göre Sistem Smtp Değerlerini getiren metod
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        public Result<SistemSMTPDegerleri> SmtpGetir(int tabloID)
        {
            var x = _requestHelper.Get<Result<SistemSMTPDegerleri>>(url + "/SMTP/SmtpGetir/" + tabloID);
            return x.Result;
        }

        /// <summary>
        /// Sistem Smtp Değerlerini Güncelleyen metod
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<SistemSMTPDegerleri> SmtpGuncelle(SistemSMTPDegerleri model)
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            model.GuncelleyenKisiID = Convert.ToInt32(cookie.KisiId);
            var x = _requestHelper.Post<Result<SistemSMTPDegerleri>>(url + "/SMTP/SmtpGuncelle", model);

            return x.Result;
        }

        /// <summary>
        /// Sistem Smtp Değerlerini Ekleyen metod
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<SistemSMTPDegerleri> SmtpKaydet(SistemSMTPDegerleri model)
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            model.KurumID = Convert.ToInt32(cookie.KurumId);
            model.IlgiliKurumID = Convert.ToInt32(cookie.KurumId);
            model.KayitEdenID = Convert.ToInt32(cookie.KisiId);
            model.KisiID = Convert.ToInt32(cookie.KisiId);
            var x = _requestHelper.Post<Result<SistemSMTPDegerleri>>(url + "/SMTP/SmtpKaydet", model);
            return x.Result;
        }

        /// <summary>
        /// Sistem Smtp Değerlerini Listeleyen metod
        /// </summary>
        /// <returns></returns>
        public Result<List<SistemSMTPDegerleri>> SmtpList()
        {
            var x = _requestHelper.Get<Result<List<SistemSMTPDegerleri>>>(url + "/SMTP/SmtpList");

            return x.Result;
        }

        /// <summary>
        /// Sistem Smtp Değerlerini silindi yapan metod
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        public Result<bool> SmtpSil(int tabloID)
        {
            var x = _requestHelper.Get<Result<bool>>(url + "/SMTP/SmtpSil/" + tabloID);

            return x.Result;
        }
    }
}