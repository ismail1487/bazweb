using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baz.Model.Entity;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.RequestManager.Abstracts;

namespace BazWebApp.Services
{
    /// <summary>
    /// Lokasyon bilgisi Interface'dir.
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Kültür verisini getiren metod
        /// </summary>
        /// <returns></returns>
        string GetCulture();

        /// <summary>
        /// Kültürü set eden metod
        /// </summary>
        /// <param name="culture"></param>
        void SetCulture(string culture);

        /// <summary>
        /// GetCultureId prop
        /// </summary>
        /// <returns></returns>
        int GetCultureId();
    }

    /// <summary>
    /// Lokasyon bilgisi için oluşturulmuş methodları barındıran sınıftır.
    /// </summary>
    /// <seealso cref="BazWebApp.Services.ILocalizationService" />
    public class LocalizationService : ILocalizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBazCookieService _bazCookieService;
        private readonly IRequestHelper _helper;

        /// <summary>
        /// Lokasyon bilgisi için oluşturulmuş methodları barındıran sınıf konst.
        /// </summary>
        /// <param name="requestHelper"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="bazCookieService"></param>
        public LocalizationService(IRequestHelper requestHelper, IHttpContextAccessor httpContextAccessor, IBazCookieService bazCookieService)
        {
            _httpContextAccessor = httpContextAccessor;
            _bazCookieService = bazCookieService;
            _helper = requestHelper;
        }

        /// <summary>
        /// Kültür bilgisini getiren methodtur.
        /// </summary>
        /// <returns></returns>
        public string GetCulture()
        {
            var rqf = _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>();
            return rqf.RequestCulture.Culture.Name;
        }

        /// <summary>
        /// Kültür verisini getiren metod
        /// </summary>
        /// <returns></returns>
        public int GetCultureId()
        {
            var kod = this.GetCulture();
            var dilId = _helper.Get<int>(LocalPortlar.IYSService + "/api/Dil/Get/" + kod).Result;
            return dilId;
        }

        /// <summary>
        /// Kültür bilgisi ve bunun için gerekli Cookie oluşturan methodtur.
        /// </summary>
        /// <param name="culture">The culture.</param>
        public void SetCulture(string culture)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), HttpOnly = true, Secure = true }
            );
        }
    }
}