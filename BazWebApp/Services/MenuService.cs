using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity;
using Baz.SharedSession;

namespace BazWebApp.Services
{
    /// <summary>
    /// Menu servisi için oluşturulmuş Interfacedir.
    /// </summary>
    public interface IMenuService
    {
        /// <summary>
        /// Menüleri getiren metod
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        Result<List<Baz.Model.Entity.ViewModel.MenuListResponse>> GetMenu(string menu);

        /// <summary>
        /// Online kullanıcı sayısını getiren metod
        /// </summary>
        /// <returns></returns>
        Result<int> GetOnlineUserCount();

        /// <summary>
        /// yetki için sayfa getirme metodu
        /// </summary>
        /// <returns></returns>
        Result<List<SistemSayfalari>> YetkiIcinSayfaGetir();
    }

    /// <summary>
    /// Menü servisinin methodlarının bulunduğu servis sınıfıdır.
    /// </summary>
    /// <seealso cref="BazWebApp.Services.IMenuService" />
    public class MenuService : IMenuService
    {
        private readonly ILocalizationService _localizationService;
        private readonly IRequestHelper _requestHelper;
        private readonly IBazCookieService _bazCookieService;
        private readonly ISharedSession _sharedSession;
        private readonly IBazAuthorizationService _bazAuthorizationService;

        /// <summary>
        /// Menü servisinin methodlarının bulunduğu servis sınıfı konst.
        /// </summary>
        /// <param name="localizationService"></param>
        /// <param name="requestHelper"></param>
        /// <param name="bazCookieService"></param>
        /// <param name="sharedSession"></param>
        /// <param name="bazAuthorizationService"></param>
        public MenuService(ILocalizationService localizationService, IRequestHelper requestHelper, IBazCookieService bazCookieService, ISharedSession sharedSession, IBazAuthorizationService bazAuthorizationService)
        {
            _localizationService = localizationService;
            _requestHelper = requestHelper;
            _bazCookieService = bazCookieService;
            _sharedSession = sharedSession;
            _bazAuthorizationService = bazAuthorizationService;
        }

        /// <summary>
        /// Menüyü parametre olarak aldığı isme göre menüyü getiren methodtur.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <returns></returns>
        public Result<List<MenuListResponse>> GetMenu(string menu)
        {
            var results = new List<MenuListResponse>();
            var menuRequest = new Baz.Model.Entity.ViewModel.MenuListRequest();
            menuRequest.DilKodu = _localizationService.GetCulture();
            menuRequest.Name = menu;
            var result = _requestHelper.Post<Result<List<Baz.Model.Entity.ViewModel.MenuListResponse>>>(LocalPortlar.IYSService + "/api/menu/list", menuRequest);
            //Dile görre menüyü getirir
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = result.Result.Value;
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        if (item.Identity == string.Empty || _bazAuthorizationService.KullaniciYetkiKontrolu(item.Identity).GetAwaiter().GetResult())
                        {
                            results.Add(item);
                        }
                    }
                }
                return results.ToResult();
            }
            return null;
        }

        /// <summary>
        /// SocketServer web API'ına bağlanarak online kullanıcı sayısını getiren method
        /// </summary>
        /// <returns>online kullanıcı sayısı</returns>
        public Result<int> GetOnlineUserCount()
        {
        //    var url = LocalPortlar.SocketServer + "/api/user/GetOnlineUserCount";
        //    var result = _requestHelper.Get<int>(url);
        //    if (result.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        return result.Result.ToResult();
        //    }
            return null;
        }

        /// <summary>
        /// Yetki merkezi için sayfa verilerini getiren method
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public Result<List<SistemSayfalari>> YetkiIcinSayfaGetir()
        {
            var dilKodu = _localizationService.GetCulture();
            var result = _requestHelper.Get<Result<List<SistemSayfalari>>>(LocalPortlar.IYSService + "/api/menu/YetkiIcinSayfaGetir/" + dilKodu);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return result.Result;
            }
            return null;
        }
    }
}