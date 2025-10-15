using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Decor;
using Baz.AOP.Logger.ExceptionLog;

namespace BazWebApp.Services
{
    /// <summary>
    /// Bildirim servis sınıfı interface i
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Bildirim listeleme metodu
        /// </summary>
        /// <returns></returns>
        Result<NotificationList> List();

        /// <summary>
        /// bildirim göründü metodu
        /// </summary>
        /// <returns></returns>
        public Result<bool> SetSeen();
    }

    /// <summary>
    /// Bildirim servis sınıfı
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly IBazCookieService _bazCookieService;
        private readonly IRequestHelper _requestHelper;

        /// <summary>
        /// Bildirim servis sınıfı konst.
        /// </summary>
        /// <param name="bazCookieService"></param>
        /// <param name="requestHelper"></param>
        /// <param name="postaciIslemlerService"></param>
        public NotificationService(IBazCookieService bazCookieService, IRequestHelper requestHelper, IPostaciIslemlerService postaciIslemlerService)
        {
            _bazCookieService = bazCookieService;
            _requestHelper = requestHelper;
        }

        /// <summary>
        /// Bildirim listeleme metodu
        /// </summary>
        /// <returns></returns>
        public Result<NotificationList> List()
        {
            var user = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            //var url = LocalPortlar.PostaciService + "/api/PostaciService/List/" + user?.KisiId;
            var url = LocalPortlar.KisiServis + "/api/KisiMesaj/List/" + user?.KisiId;
            var result = _requestHelper.Get<Result<NotificationList>>(url);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return result.Result;
            }
            return result.Result;
        }

        /// <summary>
        /// Bildirim göründü metodu
        /// </summary>
        /// <returns></returns>
        public Result<bool> SetSeen()
        {
            var user = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            //var url = LocalPortlar.PostaciService + "/api/PostaciService/SetSeen/" + user.KisiId;
            var url = LocalPortlar.KisiServis + "/api/KisiMesaj/SetSeen/" + user.KisiId;
            var result = _requestHelper.Get<Result<bool>>(url).Result;
            return result;
        }
    }
}