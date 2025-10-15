using Baz.ProcessResult;
using BazWebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Handlers
{
    /// <summary>
    /// Yetki kimlik adına göre Authorize işleminin yapıldığı sınıftır.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.TypeFilterAttribute" />
    public class PolicyBasedAuthorizeAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Yetki kimlik adına göre Authorize işleminin yapıldığı konst.
        /// </summary>
        /// <param name="yetkiKimlikAdi"></param>
        public PolicyBasedAuthorizeAttribute(string yetkiKimlikAdi = null) : base(typeof(PermissionHandler))
        {
            if (yetkiKimlikAdi != null)
                Arguments = new object[] { yetkiKimlikAdi };
        }
    };

    /// <summary>
    /// Action bazında kontrol yapan Filtre nesnesinin olduğu sınıftır.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.IAsyncAuthorizationFilter" />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class PermissionHandler : IAsyncAuthorizationFilter
    {
        private readonly string _yetkiKimlikAdi;

        /// <summary>
        /// Action bazında kontrol yapan Filtre nesnesinin olduğu konst.
        /// </summary>
        /// <param name="yetkiKimlikAdi"></param>
        public PermissionHandler(string yetkiKimlikAdi = null)
        {
            _yetkiKimlikAdi = yetkiKimlikAdi;
        }

        /// <summary>
        /// Öncelikle kullanıcının cookie bilgisi kontrol edilir.
        /// cookie bilgisi bulunmayan kullanıcı ilgili sayfaya yönlendirilmez.
        /// cookie kimlik bilgisi bulunan kullanıcı yetkiKimlikAdi değeri verilmiş ise bu sefer Yetki kontrolüne tabi tutulur.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext" />.</param>

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var cookieService = context.HttpContext.RequestServices.GetService<IBazCookieService>();
            var cookieModel = await cookieService.GetCookie();
            if (cookieModel == null)
            {
                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    var socketService = context.HttpContext.RequestServices.GetService<SocketService>();
                    socketService.Logout();
                }
                else
                {
                    await context.HttpContext.SignOutAsync();
                    context.Result = new RedirectResult("/");
                }
            } //admin mi kontrolüne göre adminse satın almaya değilse lisansınız bittiye yönlenecek.
            else if (cookieModel.AdminMi)
            {
                //context.Result = new RedirectResult("/Panel/profile");
                return;
            }
            else if (!string.IsNullOrEmpty(_yetkiKimlikAdi))
            {
                var authService = context.HttpContext.RequestServices.GetService<IBazAuthorizationService>();
                if (!authService.KullaniciYetkiKontrolu(_yetkiKimlikAdi).Result)
                {
                    if (IsAjaxRequest(context.HttpContext.Request))
                    {
                        context.Result = new JsonResult(Results.Fail("Yetkiniz bulunmuyor"));
                    }
                    else
                    {
                        context.Result = new RedirectResult("/PermissionDenied");
                    }
                }
            }
        }

        private static bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}