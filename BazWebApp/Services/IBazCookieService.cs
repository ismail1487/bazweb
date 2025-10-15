using BazWebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BazWebApp.Services
{
    /// <summary>
    /// Kullanıcı için cookienin oluşturulması ve silinmesini yöneten servistir.
    /// </summary>
    public interface IBazCookieService
    {
        /// <summary>
        /// Login işlemi gerçekleşmiş cookie bilgisini döner yada kullanıcı login olmamış ise null değeri döner.
        /// </summary>
        /// <returns>CookieModel</returns>
        Task<CookieModel> GetCookie(bool sessionControl = true);

        /// <summary>
        /// Sisteme giriş yapılması sırasında kullanıcı için bir cookie oluşturur.
        /// </summary>
        /// <param name="cookieModel">The cookie model.</param>
        /// <returns></returns>
        Task SetCookie(CookieModel cookieModel);
    }

    /// <summary>
    /// Kullanıcı için cookienin oluşturulması ve silinmesini yöneten servistir.
    /// </summary>
    /// <seealso cref="BazWebApp.Services.IBazCookieService" />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class BazCookieService : IBazCookieService
    {
        /// <summary>
        /// HttpContext Nesnesini barındırır.
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly Baz.SharedSession.ISharedSession _sharedSession;

        /// <summary>
        /// Kullanıcı için cookienin oluşturulması ve silinmesini yöneten servist sınıfının yapıcı metodu
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedSession"></param>
        public BazCookieService(IHttpContextAccessor httpContextAccessor, Baz.SharedSession.ISharedSession sharedSession)
        {
            _sharedSession = sharedSession;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Login işlemi gerçekleşmiş cookie bilgisini döner yada kullanıcı login olmamış ise null değeri döner.
        /// </summary>
        /// <returns>
        /// CookieModel
        /// </returns>
        public async Task<CookieModel> GetCookie(bool sessionControl = true)
        {
            try
            {
                var auth = await _httpContextAccessor.HttpContext.AuthenticateAsync();
                if (auth != null && auth.Succeeded && auth.Principal != null)
                {
                    var cook = new CookieModel()
                    {
                        KisiId = auth.Principal.FindFirstValue(ClaimTypes.NameIdentifier),
                        KurumId = auth.Principal.FindFirstValue(ClaimTypes.GroupSid),
                        Name = auth.Principal.FindFirstValue(ClaimTypes.Name),
                        Mail = auth.Principal.FindFirstValue(ClaimTypes.Email),
                        SessionId = auth.Principal.FindFirstValue(ClaimTypes.Sid),
                        KurumName = auth.Principal.FindFirstValue(ClaimTypes.Actor),
                        //AdminMi = auth.Principal.FindFirstValue(ClaimTypes.Actor) == "true",
                    };
                    if (!sessionControl)
                        return cook;
                    var sess = _sharedSession.Get<Baz.Model.Entity.ViewModel.KullaniciSession>(cook.SessionId).GetAwaiter().GetResult();
                    if (sess == null)
                    {
                        return null;
                    }
                    cook.MusteriTemsilcisiIdList = sess.MusteriTemsilcisiIdListesi;
                    
                    cook.AdminMi = sess.KurumAdminMi;
                    return cook;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Sisteme giriş yapılması sırasında kullanıcı için bir cookie oluşturur.
        /// </summary>
        /// <param name="cookieModel">The cookie model.</param>
        public async Task SetCookie(CookieModel cookieModel)
        {
            var anyUser = await GetCookie();
            if (anyUser != null)
                await _httpContextAccessor.HttpContext.SignOutAsync();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,cookieModel.KisiId),
                new Claim(ClaimTypes.GroupSid,cookieModel.KurumId),
                new Claim(ClaimTypes.Name,cookieModel.Name),
                new Claim(ClaimTypes.Email,cookieModel.Mail),
                new Claim(ClaimTypes.Sid,cookieModel.SessionId),
                new Claim(ClaimTypes.Actor, cookieModel.KurumName),
                //new Claim(ClaimTypes.Role, cookieModel.AdminMi.ToString()),

                //new Claim(ClaimTypes.Role,JsonConvert.SerializeObject(cookieModel.MusteriTemsilcisiIdList)),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProp = new AuthenticationProperties();

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProp);
        }
    }
}