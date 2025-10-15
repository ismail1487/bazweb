using Baz.RequestManager.Abstracts;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Services
{
    /// <summary>
    /// Soket servis sınıfı interface
    /// </summary>
    public interface ISocketService 
    {

        /// <summary>
        /// Çıkış yapıldığında çalışan metod
        /// </summary>
        public void Logout();
    }
    /// <summary>
    /// Soket servis sınıfı
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SocketService : ISocketService
    {
        private readonly IRequestHelper _requestHelper;
        private readonly IBazCookieService _bazCookieService;
        /// <summary>
        /// Soket servis sınıfı konst.
        /// </summary>
        /// <param name="requestHelper"></param>
        /// <param name="bazCookieService"></param>
        public SocketService(IRequestHelper requestHelper, IBazCookieService bazCookieService) 
        {
            _requestHelper = requestHelper;
            _bazCookieService = bazCookieService;
        }
        /// <summary>
        /// Çıkış yapıldığında çalışan metod
        /// </summary>
        public void Logout()
        {
            var cookie = _bazCookieService.GetCookie(false).GetAwaiter().GetResult();
            if (cookie == null || string.IsNullOrEmpty(cookie.KisiId))
                return;
            var url = Baz.Model.Entity.Constants.LocalPortlar.SocketServer + "api/user/UserLogout/"+cookie.KisiId;
            _requestHelper.Get<bool>(url);
        }
    }
}
