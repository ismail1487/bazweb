using Baz.Model.Entity;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using BazWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Controllers
{
    /// <summary>
    /// Yetki merkezi kontrol metodlarını içeren sınıf
    /// </summary>
    public class YetkiMerkeziController : Controller
    {
        private readonly IYetkiMerkeziService _yetkiMerkeziService;
        private readonly IBazCookieService _bazCookieService;

        /// <summary>
        /// Yetki merkezi kontrol metodlarını içeren konst....
        /// </summary>
        /// <param name="yetkiMerkeziService"></param>
        /// <param name="bazCookieService"></param>
        public YetkiMerkeziController(IYetkiMerkeziService yetkiMerkeziService, IBazCookieService bazCookieService)
        {
            _yetkiMerkeziService = yetkiMerkeziService;
            _bazCookieService = bazCookieService;
        }

        #region View Methods

        /// <summary>
        /// Yetkilendirme merkezi sayfasına yönlendiren metod
        /// </summary>
        /// <returns></returns>
        [Route("/AuthCenter")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/AuthCenter")]
        public IActionResult YetkilendirmeMerkezi()
        {
            return View("YetkilendirmeMerkezi");
        }

        /// <summary>
        /// Erişim yetki sayfasına yönlendiren metod
        /// </summary>
        /// <returns></returns>
        [Route("/AuthCenter/AccessAuthList")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/AuthCenter/AccessAuthList")]
        public IActionResult ErisimYetkiList()
        {
            return View("ErisimYetkiList");
        }

        #endregion View Methods

        #region Api Methods

        /// <summary>
        /// Erişim yetkilendirme tanımlarını kaydeden method.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/YetkiMerkezi/ErisimYetkilendirmeTanimlariKaydet")]
        public Result<List<ErisimYetkilendirmeTanimlari>> ErisimYetkilendirmeTanimlariKaydet(ErisimYetkileriKayitModel data)
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var list = new List<ErisimYetkilendirmeTanimlari>();

            foreach (var birim in data.IlgiliKurumOrganizasyonBirimIdList)
            {
                foreach (var sayfa in data.ErisimYetkisiVerilenSayfaIdList)
                {
                    var yetkiTanimi = new ErisimYetkilendirmeTanimlari()
                    {
                        IlgiliKurumOrganizasyonBirimTanimiId = birim,
                        ErisimYetkisiVerilenSayfaId = sayfa,
                        KayitTarihi = DateTime.Now,
                        ErisimYetkisiVerilmeTarihi = DateTime.Now,
                        GuncellenmeTarihi = DateTime.Now,
                        AktifMi = 1,
                        SilindiMi = 0,
                        KayitEdenID = Convert.ToInt32(cookie.KisiId),
                        KisiID = Convert.ToInt32(cookie.KisiId),
                        ErisimYetkisiVerenKisiId = Convert.ToInt32(cookie.KisiId),
                        IlgiliKurumId = Convert.ToInt32(cookie.KurumId),
                        KurumID = Convert.ToInt32(cookie.KurumId),
                    };
                    list.Add(yetkiTanimi);
                }
            }
            var result = _yetkiMerkeziService.ErisimYetkilendirmeTanimlariKaydet(list);
            return result;
        }

        /// <summary>
        ///
        /// KişiID değeri ile ilgili kişinin yetkilendirildiği sayfa tanımlarını getiren method.
        /// </summary>
        /// <param name="kisiID">ilgili kişiID değeri</param>
        /// <returns>yetkilendirilen sayfa tanımları listesiin döndürür.</returns>
        [HttpGet]
        [Route("/YetkiMerkezi/KisiYetkilerListGetir/{kisiID}")]
        public Result<List<string>> KisiYetkilerListGetir(int kisiID)
        {
            var result = _yetkiMerkeziService.KisiYetkilerListGetir(kisiID);
            return result;
        }

        /// <summary>
        /// Erişim yetkilendirme tanimlari listesinin tanımlarını getiren method.
        /// </summary>
        /// <returns>erişim yetki tanımları view model listesi döndürür.</returns>
        [HttpGet]
        [Route("/YetkiMerkezi/ErisimYetkiTanimListGetir")]
        public Result<List<ErisimYetkilendirmeTanimlariListView>> ErisimYetkiTanimListGetir()
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var kurumId = Convert.ToInt32(cookie.KurumId);
            var result = _yetkiMerkeziService.ErisimYetkiTanimListGetir().Value;
            var listForKurum = new List<ErisimYetkilendirmeTanimlariListView>();
            foreach (var item in result)
            {
                if (item.KurumID == kurumId)
                {
                    listForKurum.Add(item);
                }
            }
            return listForKurum.ToResult();
        }

        /// <summary>
        /// Erişim yetki tanımı kaydını silen method.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>sonuca göre true veya false döndürür.</returns>
        [HttpGet]
        [Route("/YetkiMerkezi/ErisimYetkiTanimiSil/{id}")]
        public Result<bool> ErisimYetkiTanimiSil(int id)
        {
            var result = _yetkiMerkeziService.ErisimYetkiTanimiSil(id);
            return result;
        }

        #endregion Api Methods
    }
}