using Baz.RequestManager;
using BazWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Helpers
{
    /// <summary>
    /// Cookie'deki session Id'yi request helperın headerına ekleyen servis sınıfı
    /// </summary>
    public class RequestManagerHeaderHelperForCookie
    {
        readonly IBazCookieService _bazCookieService;
        /// <summary>
        /// Cookie'deki session Id'yi request helperın headerına ekleyen servis konst.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public RequestManagerHeaderHelperForCookie(IServiceProvider serviceProvider)
        {
            _bazCookieService = (IBazCookieService)serviceProvider.GetService(typeof(IBazCookieService));
        }
        /// <summary>
        /// Cookie'deki session Id'yi request helperın headerına ekler
        /// </summary>
        /// <returns></returns>
        public RequestHelperHeader SetDefaultHeader()
        {
            var headers = new RequestHelperHeader();
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            if (cookie != null)
            {
                headers.Add("sessionId", cookie.SessionId);
            }

            return headers;
        }
    }
}
