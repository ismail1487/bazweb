using BazWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using Baz.RequestManager;
using Baz.Model.Entity.ViewModel;
using Microsoft.AspNetCore.Http;
using System.Net;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Baz.Attributes;
using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.SharedSession;
using BazWebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Google;

namespace BazWebApp.Controllers
{
    /// <summary>
    /// Login ve Kayıt olma işlemlerini yöneten MVC controller classı
    /// </summary>

    public class LoginRegisterController : Controller
    {
        private readonly IBazCookieService _bazCookieService;
        private readonly ILoginRegisterService _loginRegisterService;
        private readonly IKurumService _kurumService;
        private readonly IKisiService _kisiService;

        /// <summary>
        /// Login ve Kayıt olma işlemlerini yöneten MVC controller classının yapıcı metodu
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="bazCookieService"></param>
        /// <param name="kurumService"></param>
        /// <param name="loginRegisterService"></param>
        /// <param name="sharedSession"></param>
        /// <param name="kisiService"></param>
        public LoginRegisterController(ILogger<LoginRegisterController> logger, IBazCookieService bazCookieService, IKurumService kurumService, ILoginRegisterService loginRegisterService, ISharedSession sharedSession, IKisiService kisiService)
        {
            _bazCookieService = bazCookieService;
            _kurumService = kurumService;
            _loginRegisterService = loginRegisterService;
            _kisiService = kisiService;
        }

        #region View Methods

        /// <summary>
        /// Gizlilik Sözleşmesi sayfasına yönlediren metod
        /// </summary>
        /// <returns></returns>
        [Route("/LoginRegister/confidentialityagreement")]
        public ActionResult Confidentialityagreement()
        {
            return View("confidentialityagreement");
        }

        /// <summary>
        /// Üyelik Koşulları sayfasına yönlediren metod
        /// </summary>
        /// <returns></returns>
        [Route("/LoginRegister/membershipconditions")]
        public ActionResult Membershipconditions()
        {
            return View("membershipconditions");
        }

        /// <summary>
        /// forgotPassword.cshtml sayfasına erişim sağlayan method.
        /// </summary>
        /// <returns>ilgili sayfayı döndürür.</returns>
        [Route("/LoginRegister/ForgotPassword")]
        [ProcessName(Name = "şifremi unuttum sayfasına erişimin yönetilmesi")]
        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        /// <summary>
        /// RecoverPassword.cshtml sayfasına erişim sağlayan method.
        /// </summary>
        /// <returns>ilgili sayfayı döndürür.</returns>
        [Route("/LoginRegister/RecoverPassword")]
        [ProcessName(Name = "şifremi unuttum sayfasına erişimin yönetilmesi")]
        public IActionResult RecoverPassword()
        {
            return View("RecoverPassword");
        }

        /// <summary>
        /// Activation.cshtml sayfasına erişim sağlayan method.
        /// </summary>
        /// <returns>ilgili sayfayı döndürür.</returns>
        [Route("/LoginRegister/Activation")]
        [ProcessName(Name = "şifremi unuttum sayfasına erişimin yönetilmesi")]
        public IActionResult Activation()
        {
            return View("Activation");
        }

        #endregion View Methods

        #region Web API Methods

        ///// <summary>
        ///// Facebook işle login
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("facebook-login")]
        //[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        //public IActionResult FacebookLogin()
        //{
        //    var properties = new AuthenticationProperties { RedirectUri = Url.Action("FacebookResponse") };
        //    return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        //}

        ///// <summary>
        ///// Facebook ile register olma
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("facebook-response")]
        //[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        //public async Task<IActionResult> FacebookResponse()
        //{
        //    var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //    var claims = result.Principal.Identities
        //        .FirstOrDefault().Claims.Select(claim => new
        //        {
        //            claim.Issuer,
        //            claim.OriginalIssuer,
        //            claim.Type,
        //            claim.Value
        //        });
        //    var id = claims.FirstOrDefault(p => p.Type.Contains("nameidentifier")).Value;
        //    var mail = claims.FirstOrDefault(p => p.Type.Contains("emailaddress")).Value;
        //    if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(mail))
        //    {
        //        var loginResult = _loginRegisterService.Login(new LoginModel() { EmailOrUserName = mail, ExternalId = id, Facebook = true, Google = false, Password = Guid.NewGuid().ToString("N").Substring(0, 10) });
        //        if (loginResult.Value == null)
        //        {
        //            return Redirect("/LoginRegister/Register?m=" + mail);
        //        }
        //        return Redirect("/panel");
        //    }

        //    return Redirect("/");
        //}

        ///// <summary>
        ///// Google ile login işlemi
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("googlelogin")]
        //[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        //public IActionResult GoogleLogin()
        //{
        //    var properties = new AuthenticationProperties { RedirectUri = Url.Action("ExternalResponse") };
        //    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        //}

        ///// <summary>
        ///// Google ile register olma
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("externalresponse")]
        //[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        //public async Task<IActionResult> ExternalResponse()
        //{
        //    var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //    var claims = result.Principal.Identities
        //        .FirstOrDefault().Claims.Select(claim => new
        //        {
        //            claim.Issuer,
        //            claim.OriginalIssuer,
        //            claim.Type,
        //            claim.Value
        //        });
        //    var id = claims.FirstOrDefault(p => p.Type.Contains("nameidentifier")).Value;
        //    var mail = claims.FirstOrDefault(p => p.Type.Contains("emailaddress")).Value;
        //    if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(mail))
        //    {
        //        Result<string> loginResult = _loginRegisterService.Login(new LoginModel() { EmailOrUserName = mail, ExternalId = id, Facebook = false, Google = true, Password = Guid.NewGuid().ToString("N").Substring(0, 10) });
        //        if (loginResult.Value == null)
        //        {
        //            return Redirect("/LoginRegister/Register?m=" + mail);
        //        }

        //        return Redirect("/panel");
        //    }

        //    return Redirect("/");
        //}

        /// <summary>
        /// Login işlemini Web API'ye yönlendiren method.
        /// </summary>
        /// <param name="model"> Login bilgilerini içeren LoginModel parametresi.</param>
        /// <returns></returns>
        [ProcessName(Name = "Login işleminin LoginregisterService API'ına ileten ve dönen sonuca göre login işlemini sağlanması")]
        [HttpPost]
        [Route("/LoginRegister/Login")]
        public Result<string> Login([FromBody] LoginModel model)
        {
            var result = _loginRegisterService.Login(model);
            return result;
        }

        /// <summary>
        /// Session ve cookie değerlerinin silinmesi ile çıkış işlemi ve logout işlemine dair kaydın eklenmesi işlemini gerçekleştiren method.
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Çıkış işlemi")]
        [Route("/LoginRegister/Logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            if (cookie != null)
            {
                var sessionId = cookie.SessionId;
                _loginRegisterService.LogoutKaydiAt(sessionId);
            }
            await HttpContext.SignOutAsync();
            return Json(true);
        }

        /// <summary>
        /// Bireysel Register işlemini Web API'ye yönlendiren method.
        /// </summary>
        /// <param name="model"> Kayıt bilgilerini içeren KurumKisiPostModel parametresi.</param>
        /// <returns></returns>
        [ProcessName(Name = "Bireysel Kayıt verilerinin LoginRegisterService API'ına gönderilmesi ile kayıt işleminin gerçekleşmesi")]
        [HttpPost]
        [Route("/LoginRegister/BireyselRegister")]
        public Result<SistemLoginSifreYenilemeAktivasyonHareketleri> BireyselRegister(KurumKisiPostModel model)
        {
            var result = _loginRegisterService.BireyselRegister(model);
            return result;
        }

        /// <summary>
        /// Register işlemini Web API'ye yönlendiren method.
        /// </summary>
        /// <param name="model"> Kayıt bilgilerini içeren KurumKisiPostModel parametresi.</param>
        /// <returns></returns>
        [ProcessName(Name = "Kayıt verilerinin LoginRegisterService API'ına gönderilmesi ile kayıt işleminin gerçekleşmesi")]
        [HttpPost]
        public Result<SistemLoginSifreYenilemeAktivasyonHareketleri> Register(KurumKisiPostModel model)
        {
            var result = _loginRegisterService.Register(model);
            return result;
        }

        /// <summary>
        /// Register.cshtml sayfasına erişim sağlayan method.
        /// </summary>
        /// <returns>ilgili sayfayı döndürür.</returns>
        //[HttpGet]
        [Route("/LoginRegister/Register")]
        [ProcessName(Name = "kayıt olma sayfasına erişimin yönetilmesi")]
        [HttpGet]
        public IActionResult Register()
        {
            return View("Register");
        }

        /// <summary>
        /// Hesap aktifleştirme işlemini Web API'ye yönlendiren method.
        /// </summary>
        /// <param name="guid"> etkinleştirilecek hesabı belirten GUID parametresi.</param>
        /// <returns></returns>
        [ProcessName(Name = "GUID değeri ile hesabın aktifleştirilmesi için LoginRegisterService API'ına request gönderilmesi")]
        [HttpPost]
        public Result<KisiTemelBilgiler> HesapAktiflestirme(string guid)
        {
            var result = _loginRegisterService.HesapAktiflestirme(guid);
            return result;
        }

        /// <summary>
        /// Hesap aktifleştirme isteği halen katif mi kontrolü işlemini Web API'a yönlendiren method.
        /// </summary>
        /// <param name="guid">kontrol edilecek hesap aktifleştirme isteği için tanımlı GUID parametresi</param>
        /// <returns>kotrol sonucuna göre true veya false döner.</returns>
        [ProcessName(Name = "GUID değeri ile hesabın aktifleştirilmesi için LoginRegisterService API'ına request gönderilmesi")]
        [HttpPost]
        public Result<bool> HesapAktivasyonLinkiGecerliMi(string guid)
        {
            var result = _loginRegisterService.HesapAktivasyonLinkiGecerliMi(guid);
            return result;
        }

        /// <summary>
        /// Şifre yenileme talebini Web API'ye yönlendiren method.
        /// </summary>
        /// <param name="mail"> şifre yenilenme talebinde bulunulan hesabın mail adresi.</param>
        /// <returns></returns>
        [ProcessName(Name = "mail adresi ile sifre yenileme talebi oluşturulması için KisiService API'ına request gönderilmesi")]
        [HttpPost]
        public Result<SistemLoginSifreYenilemeAktivasyonHareketleri> SifreYenilemeIstegi(string mail)
        {
            var result = _kisiService.SifreYenilemeIstegi(mail);
            return result;
        }

        /// <summary>
        /// Şifre yenileme işlemine dair requesti Web API'ye yönlendiren method.
        /// </summary>
        /// <param name="model"> şifre yenilenmesi için gereken verileri içeren SifreModel parametresi.</param>
        /// <returns></returns>
        [ProcessName(Name = "Şifre yenilenme işlemi için KisiService API'ına request gönderilmesi")]
        [HttpPost]
        public Result<SifreModel> SifreYenile(SifreModel model)
        {
            var result = _kisiService.SifreYenile(model);
            return result;
        }

        /// <summary>
        /// SifreYenile.cshtml sayfasına erişim sağlayan method.
        /// </summary>
        /// <returns>ilgili sayfayı döndürür.</returns>
        [Route("/LoginRegister/ResetPassword")]
        [ProcessName(Name = "Zorunlu şifre yenileme sayfasına erişimin yönetilmesi")]
        public IActionResult SifreYenile()
        {
            return View("SifreYenile");
        }

        /// <summary>
        /// Şifre yenileme işlemine dair requesti Web API'ye yönlendiren method.
        /// </summary>
        /// <param name="guid"> şifre yenilenmesi için gereken verileri içeren SifreModel parametresi.</param>
        /// <returns></returns>
        [ProcessName(Name = "Şifre yenilenme işlemi için KisiService API'ına request gönderilmesi")]
        [HttpPost]
        public Result<bool> SifreYenilemeGecerliMi([FromBody] string guid)
        {
            var result = _kisiService.SifreYenilemeGecerliMi(guid);
            return result;
        }

        /// <summary>
        ///  Kişi kullanıcı adı ve mail adresinin bulunup bulunmadığının kontrol edilmesi için Web API'a gönderen method.
        /// </summary>
        /// <param name="mailOrUsername"> kontrol edilecek mail adresi/ kullanıcı adı</param>
        /// <returns>kontrol edilen değer yoksa true, varsa false döner.</returns>
        [ProcessName(Name = "Kullanıcı adı / mail adresi sistemde kayıtlı mı kontrolü")]
        [HttpPost]
        public Result<bool> KullaniciAdiKontrolu(string mailOrUsername)
        {
            var result = _loginRegisterService.KullaniciAdiKontrolu(mailOrUsername);
            return result;
        }

        /// <summary>
        /// Kurum dinamik listesi işlemi için gerekli listeyi web API'den getiren method.
        /// </summary>
        /// <param name="kurumAdi"></param>
        /// <returns>kaydedilen verileri JSON formatında döndürür.</returns>
        [ProcessName(Name = "Kurum ismine göre yapılan aramadan listenin dönülmesi")]
        [HttpPost]
        public Result<List<KurumTemelBilgiler>> KurumList(string kurumAdi)
        {
            var result = _kurumService.KurumList(kurumAdi);
            return result;
        }

        /// <summary>
        /// Kullanıcı Id bilgisini getiren metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Kullanıcı Id bilgisini döner")]
        [HttpGet]
        public Result<int> GetUserId()
        {
            int id = 0;
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            if (cookie != null)
                return Convert.ToInt32(cookie.KisiId).ToResult();
            return id.ToResult();
        }

        #endregion Web API Methods
    }
}