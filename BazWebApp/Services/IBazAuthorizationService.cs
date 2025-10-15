using Baz.Model.Entity.ViewModel;
using Baz.SharedSession;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Services
{
    /// <summary>
    /// Kullanıcının Yetkilerinin kontrolülünü sağlayan servistir.
    /// İlgili kontrol işlemi Ortak Session Server üzerinde barınan Yetki listesi üzerinde yapılır. 
    /// </summary>
    public interface IBazAuthorizationService
    {
        /// <summary>
        /// Verilen Yatki Kimlik bilgisinin kontrolünü yapar.
        /// </summary>
        /// <param name="yetkiKimlikAdi">The yetki kimlik adi.</param>
        /// <returns></returns>
        Task<bool> KullaniciYetkiKontrolu(string yetkiKimlikAdi);   
    }
    /// <summary>
    /// Kullanıcının Yetkilerinin kontrolülünü sağlayan servistir.
    /// İlgili kontrol işlemi Ortak Session Server üzerinde barınan Yetki listesi üzerinde yapılır. 
    /// </summary>
    public class BazAuthorizationService : IBazAuthorizationService
    {
        private readonly ISharedSession _sharedSession;
        private readonly IBazCookieService _bazCookieService;

        /// <summary>
        /// Kullanıcının Yetkilerinin kontrolülünü sağlayan servis sınıfının yapıcı metodu
        /// </summary>
        /// <param name="sharedSession"></param>
        /// <param name="bazCookieService"></param>
        public BazAuthorizationService(ISharedSession sharedSession,IBazCookieService bazCookieService)
        {
            _sharedSession = sharedSession;
            _bazCookieService = bazCookieService;
        }
        /// <summary>
        /// Verilen Yatki Kimlik bilgisinin kontrolünü yapar.
        /// </summary>
        /// <param name="yetkiKimlikAdi"> yetki kimlik adi.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public async Task<bool> KullaniciYetkiKontrolu(string yetkiKimlikAdi)
        {
          
            //Öncelikle kullanıcının login olup olmadığı cookie bilgisine bakılarak belirlenir. 
            //Login olmyan kullanıcı için false değeri dönülür.
            var cookieModel = await _bazCookieService.GetCookie();
            if (cookieModel != null) 
            {
                //Login olmuş kullanıcı için yetki kontrolü yapılmak için session serverdan kullanıcının session nesnesi okunur.
                //Kullanıcının session nesnesi içerisinde bulunan YetkiListesi içerisinde ilgili yetkiKimlikAdi aranır ve buna göre yetki bilgisi dönülür.
                 
                var userSession= await _sharedSession.Get<Baz.Model.Entity.ViewModel.KullaniciSession>(cookieModel.SessionId);
                if (userSession == null)
                    return false;
                if (string.IsNullOrEmpty(yetkiKimlikAdi))
                    return true;
                if(userSession.KullaniciYetkiListesi != null)
                    if (userSession.KullaniciYetkiListesi.Any(p => p == yetkiKimlikAdi))
                        return true;
            }
            return false;
        }
    }
}
