using Baz.Model.Entity;
using Baz.ProcessResult;
using BazWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.Attributes;
using BazWebApp.Models;
using BazWebApp.Handlers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Baz.SharedSession;

namespace BazWebApp.Controllers
{
    /// <summary>
    /// Panel kontrol methodlarının olduğu Sınıftır.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class PanelController : Controller
    {
        private readonly ILocalizationService _localizationService;
        private readonly IBazCookieService _bazCookieService;
        private readonly IKisiService _kisiService;
        private readonly IKisiIliskiService _kisiIliskiService;
        private readonly IMedyaKutuphanesiService _medyaKutuphanesiService;
        private readonly IMenuService _menuService;
        private readonly IParamOrganizasyonBirimleriService _paramOrganizasyonBirimleriService;
        private readonly IKurumOrganizasyonBirimTanimlariService _kurumOrganizasyonBirimTanimlariService;
        private readonly IGenelParametrelerService _genelParametrelerService;
        private readonly IKurumService _kurumService;
        private readonly IKurumIliskiService _kurumIliskiService;
        private readonly CookieModel _cookieModel;
        private readonly IDilService _dilService;
        private readonly ILoginRegisterService _loginRegisterService;
        private readonly INotificationService _notificationService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISearchService _searchService;
        private readonly IKurumMesajlasmaModuluIzinIslemleriService _kurumMesajlasmaModuluIzinIslemleriService;
        private readonly IExcelReadWriteHandler _excelReadWriteHandler;
        private readonly IIYSService _iysService;

        /// <summary>
        /// Panel kontrol methodlarının olduğu konst.
        /// </summary>
        /// <param name="localizationService"></param>
        /// <param name="bazCookieService"></param>
        /// <param name="kisiService"></param>
        /// <param name="medyaKutuphanesiService"></param>
        /// <param name="menuService"></param>
        /// <param name="paramOrganizasyonBirimleriService"></param>
        /// <param name="kurumOrganizasyonBirimTanimlariService"></param>
        /// <param name="genelParametrelerService"></param>
        /// <param name="kurumService"></param>
        /// <param name="dilService"></param>
        /// <param name="loginRegisterService"></param>
        /// <param name="kurumIliskiService"></param>
        /// <param name="kisiIliskiService"></param>
        /// <param name="searchService"></param>
        /// <param name="notificationService"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="kurumMesajlasmaModuluIzinIslemleriService"></param>
        /// <param name="excelReadWriteHandler"></param>
        /// <param name="iysService"></param>
        public PanelController(ILocalizationService localizationService, IBazCookieService bazCookieService,
            IKisiService kisiService, IMedyaKutuphanesiService medyaKutuphanesiService, IMenuService menuService,
            IParamOrganizasyonBirimleriService paramOrganizasyonBirimleriService,
            IKurumOrganizasyonBirimTanimlariService kurumOrganizasyonBirimTanimlariService,
            IGenelParametrelerService genelParametrelerService, IKurumService kurumService,
            IDilService dilService, ILoginRegisterService loginRegisterService,
            IKurumIliskiService kurumIliskiService, IKisiIliskiService kisiIliskiService,
            ISearchService searchService,
            INotificationService notificationService, IServiceProvider serviceProvider, IKurumMesajlasmaModuluIzinIslemleriService kurumMesajlasmaModuluIzinIslemleriService, IExcelReadWriteHandler excelReadWriteHandler, IIYSService iysService)
        {
            _localizationService = localizationService;
            _bazCookieService = bazCookieService;
            _kisiService = kisiService;
            _medyaKutuphanesiService = medyaKutuphanesiService;
            _menuService = menuService;
            _paramOrganizasyonBirimleriService = paramOrganizasyonBirimleriService;
            _kurumOrganizasyonBirimTanimlariService = kurumOrganizasyonBirimTanimlariService;
            _cookieModel = _bazCookieService.GetCookie().Result;
            _genelParametrelerService = genelParametrelerService;
            _kurumService = kurumService;
            _dilService = dilService;
            _loginRegisterService = loginRegisterService;
            _notificationService = notificationService;
            _kurumIliskiService = kurumIliskiService;
            _kisiIliskiService = kisiIliskiService;
            _serviceProvider = serviceProvider;
            _searchService = searchService;
            _kurumMesajlasmaModuluIzinIslemleriService = kurumMesajlasmaModuluIzinIslemleriService;
            _excelReadWriteHandler = excelReadWriteHandler;
            _iysService = iysService;
        }

        #region View Methods

        /// <summary>
        /// Controller'a erişildiğinde gösterilecek index sayfasını çağıran metot.
        /// </summary>
        /// <returns></returns>
        [PolicyBasedAuthorize]
        public IActionResult Index()
        {
            var _sessionService = _serviceProvider.GetService<ISharedSession>();
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var session = _sessionService.Get<KullaniciSession>(cookie.SessionId).GetAwaiter().GetResult();

            //ViewBag.Report = planlananGerceklesen;
            if (session.KullaniciYetkiListesi.Any())
            {
                return Redirect("/panel/Profile");
            }
            else if (session.KullaniciYetkiListesi.Count == 0)
            {
                TempData["Message"] = "Bağlı olduğunuz pozisyona erişim yetkisi atanmadığı için sayfalara erişim yetkiniz bulunmamaktadır. Yöneticinizle iletişime geçiniz.";
                return Redirect("/NoAuth");
            }
            
            return Redirect("/panel/Profile");
        }

        /// <summary>
        /// Profil sayfasına yönlendiren method.
        /// </summary>
        /// <returns></returns>
        //[Handlers.PolicyBasedAuthorize]
        public IActionResult Profile()
        {
            return View("Profile");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [PolicyBasedAuthorize]
        public IActionResult Mesajlar()
        {
            return View("Mesajlar");
        }



        /// <summary>
        /// Kişi Tanımlama sayfasına yönlendiren method.
        /// </summary>
        /// <returns></returns>
        [Route("/panel/PersonDefinition")]
        [PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/PersonDefinition")]
        public IActionResult KisiTanimlama()
        {
            return View("KisiTanimlama");
        }

        ///// <summary>
        ///// Dashboard sayfasına yönlendirir.
        ///// </summary>
        ///// <returns></returns>
        //[Route("/Dashboard")]
        //[Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/PersonDefinition")]
        //public IActionResult Dashboard()
        //{
        //    return View("Index");
        //}

        /// <summary>
        /// Kişi Tanımlama sayfasına yönlendiren method.
        /// </summary>
        /// <returns></returns>
        [Route("/panel/Announcements")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/Announcements")]
        public IActionResult Announcements()
        {
            return View("Announcements");
        }

        /// <summary>
        /// Update işlemlerinin yapıldığı sayfaya yönlendiren method.
        /// </summary>
        /// <returns></returns>
        [Route("/panel/profile/update")]
        [HttpGet]
        [Handlers.PolicyBasedAuthorize]
        public IActionResult Update()
        {
            return View("Update");
        }

        /// <summary>
        /// Kişi güncelleme sayfasını getiren metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        [Route("/panel/PersonUpdate/{kisiID}")]
        [HttpGet]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/PersonUpdate/{kisiID}")]
        public IActionResult KisiGuncelle(int kisiID)
        {
            return View("KisiGuncelleme", kisiID);
        }

        /// <summary>
        /// Kişi listesi sayfasını getiren metot
        /// </summary>
        /// <returns></returns>
        [Route("/panel/PersonList")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/PersonList")]
        public IActionResult KisiListesi()
        {
            return View("KisiListesi");
        }

        /// <summary>
        /// Kişi temel profil sayfasını getiren metot
        /// </summary>
        /// <returns></returns>
        [Route("/panel/PersonDetail/{kisiId}")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/PersonDetail/{kisiId}")]
        public IActionResult KisiTemelProfil(int kisiId)
        {
            return View("KisiTemelProfil", kisiId);
        }

        /// <summary>
        /// Kurum temel sayfasını getiren metot
        /// </summary>
        /// <returns></returns>
        [Route("/panel/CompanyDefinition")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/CompanyDefinition")]
        public IActionResult KurumTemelKayit()
        {
            return View("KurumTemelKayit");
        }

        /// <summary>
        /// Kurum güncelleme sayfasını getiren metot
        /// </summary>
        /// <returns></returns>
        [Route("/panel/CompanyUpdate/{companyId}")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/CompanyUpdate/{companyId}")]
        public IActionResult KurumTemelGuncelle()
        {
            return View("KurumTemelGuncelle");
        }

        /// <summary>
        /// Kurum liste sayfasını getiren metot
        /// </summary>
        /// <returns></returns>
        [Route("/panel/CompanyList")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/CompanyList")]
        public IActionResult KurumListesi()
        {
            return View("KurumListesi");
        }

        /// <summary>
        /// Kurum Organizasyon sayfasını getiren method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/panel/CompanyOrganizationDefinition")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/CompanyOrganizationDefinition")]
        public ActionResult KurumOrganizasyon()
        {
            return View("KurumOrganizasyon");
        }

        /// <summary>
        /// Kurum Organizasyon sayfasını getiren method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/panel/CompanyOrganizationDefinitionL")]
        public ActionResult KurumOrganizasyonL()
        {
            return View("KurumOrganizasyonL");
        }

        /// <summary>
        /// Id'ye göre kurum temel profilini getiren metot
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProcessName(Name = "kurum temel verileri getirme")]
        [Route("/panel/CompanyDetail/{id}")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/CompanyDetail/{id}")]
        [HttpGet]
        public ActionResult KurumTemelProfil(int id)
        {
            return View("KurumTemelProfil");
        }

        /// <summary>
        /// Sistem idari profilini getiren metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "sistem idari verileri getirme")]
        [Route("/panel/SystemAdministrativeDetail")]
        [HttpGet]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/SystemAdministrativeDetail")]
        public ActionResult SistemIdariProfil()
        {
            return View("SistemIdariProfil");
        }

        /// <summary>
        /// Kurum idari profilini getiren metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "kurum idari verileri getirme")]
        [Route("/panel/CompanyAdministrativeDetail")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/CompanyAdministrativeDetail")]
        [HttpGet]
        public ActionResult KurumIdariProfil()
        {
            return View("KurumIdariProfil");
        }

        /// <summary>
        /// Kurumlar arası ilişki sayfasını getiren metot
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/panel/RelationshipBetweenCompaniesDefinition")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/RelationshipBetweenCompaniesDefinition")]
        public ActionResult KurumlarArasiIliskiTanimlama()
        {
            return View("KurumlarArasiIliskiTanimlama");
        }

        /// <summary>
        /// Kişiler arası ilişki sayfasını getiren metot
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/panel/RelationshipBetweenPersonsDefinition")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/RelationshipBetweenPersonsDefinition")]
        public ActionResult KisilerArasiIliskiTanimlama()
        {
            return View("KisilerArasiIliskiTanimlama");
        }

        /// <summary>
        /// Mesajlaşma geçmişi arama sayfasını getiren metot
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/panel/MessageRecordsSearch")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/panel/MessageRecordsSearch")]
        public IActionResult MessageRecordsSearch()
        {
            return View("MessageRecordsSearch");
        }

        /// <summary>
        /// Kurum msajlaşma modülü ayar sayfasını getiren metot
        /// </summary>
        /// <returns></returns>
        public IActionResult CompanyMessageModulePermissions()
        {
            return View("CompanyMessageModulePermissions");
        }

        /// <summary>
        /// Dashboard sayfasına yönledniren metod
        /// </summary>
        /// <returns></returns>
        [Route("/Dashboard")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/Dashboard")]
        public IActionResult Dashboard()
        {
            return View("Index");
        }

        /// <summary>
        /// Ayarlar planeline yönlendiren metod
        /// </summary>
        /// <returns></returns>
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/Panel/SettingsPanel")]
        [Route("/Panel/SettingsPanel")]
        public IActionResult SettingsPanel()
        {
            return View("SettingsPanel");
        }

        /// <summary>
        /// Sistemdeki kişileri listeleyen sayfaya yönlendiren metodx
        /// </summary>
        /// <returns></returns>
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/Panel/MemberList")]
        [Route("/Panel/MemberList")]
        public IActionResult MemberList()
        {
            return View("MemberList");
        }

        /// <summary>
        /// Malzeme Talep Et sayfasına yönlendiren metod
        /// </summary>
        /// <returns></returns>
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/MalzemeTalepEt")]
        [Route("/MalzemeTalepEt")]
        public IActionResult MalzemeTalepEt()
        {
            return View("MalzemeTalepEt");
        }

        /// <summary>
        /// Proje Genel Bilgileri listesini getiren metod
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/panel/ProjeGenelBilgilerList")]
        public Result<List<ProjeGenelBilgilerVM>> ProjeGenelBilgilerList()
        {
            var result = _iysService.ProjeGenelBilgilerList();
            return result;
        }

        /// <summary>
        /// Talep Süreç Statüleri listesini getiren metod
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/panel/TalepSurecStatuleriList")]
        public Result<List<TalepSurecStatuleriVM>> TalepSurecStatuleriList()
        {
            var result = _iysService.TalepSurecStatuleriList();
            return result;
        }

        /// <summary>
        /// Malzeme taleplerini getiren metod
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/panel/MalzemeTalepleriniGetir")]
        public Result<List<MalzemeTalepVM>> MalzemeTalepleriniGetir([FromBody] MalzemeTalepFilterModel model)
        {
            var result = _iysService.MalzemeTalepleriniGetir(model);
            return result;
        }

        /// <summary>
        /// Toplu SAT Bilgisi Güncelleme
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/panel/TopluSATBilgisiGuncelle")]
        public Result<string> TopluSATBilgisiGuncelle([FromBody] TopluSATBilgisiGuncelleModel model)
        {
            var result = _iysService.TopluSATBilgisiGuncelle(model);
            return result;
        }

        /// <summary>
        /// Süreç Statüleri Bildirim Tiplerini getiren metod
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/panel/SurecStatuleriBildirimTipleriList/{tabloID}")]
        public Result<List<SurecStatuleriBildirimTipleriVM>> SurecStatuleriBildirimTipleriList(int tabloID)
        {
            var result = _iysService.SurecStatuleriBildirimTipleriList(tabloID);
            return result;
        }

        /// <summary>
        /// Malzeme talep et metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/panel/MalzemeTalepEt")]
        public Result<string> MalzemeTalepEt([FromBody] MalzemeTalepEtModel model)
        {
            var result = _iysService.MalzemeTalepEt(model);
            return result;
        }

        /// <summary>
        /// Malzemeleri hazırla metodu
        /// </summary>
        /// <param name="malzemeTalepSurecTakipID">Malzeme talep süreç takip ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("/panel/MalzemeleriHazirla/{malzemeTalepSurecTakipID}")]
        public Result<bool> MalzemeleriHazirla(int malzemeTalepSurecTakipID)
        {
            var result = _iysService.MalzemeleriHazirla(malzemeTalepSurecTakipID);
            return result;
        }

        /// <summary>
        /// Malzeme iade et metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/MalzemeTalepGenelBilgiler/MalzemeIadeEt")]
        public Result<bool> MalzemeIadeEt([FromBody] MalzemeIadeEtModel model)
        {
            var result = _iysService.MalzemeIadeEt(model);
            return result;
        }

        /// <summary>
        /// Malzeme mal kabul et API metodu
        /// </summary>
        /// <param name="malzemeTalepSurecTakipID">Malzeme talep süreç takip ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/MalzemeTalepGenelBilgiler/MalKabulEt/{malzemeTalepSurecTakipID}")]
        public Result<bool> MalKabulEt(int malzemeTalepSurecTakipID)
        {
            var result = _iysService.MalKabulEt(malzemeTalepSurecTakipID);
            return result;
        }

        /// <summary>
        /// Malzeme hasarlı olarak işaretle API metodu
        /// </summary>
        /// <param name="model">Hasarlı işaretleme modeli</param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/MalzemeTalepGenelBilgiler/HasarliOlarakIsaretle")]
        public Result<bool> HasarliOlarakIsaretle([FromBody] MalzemeIadeEtModel model)
        {
            var result = _iysService.HasarliOlarakIsaretle(model);
            return result;
        }

        /// <summary>
        /// Süreç statüleri bildirim tipleri API metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/SurecStatusleriBildirimTipleri")]
        public Result<List<SurecStatuleriBildirimTipleriVM>> SurecStatuleriBildirimTipleriAPI([FromBody] dynamic model)
        {
            int tabloId = model.tabloId;
            var result = _iysService.SurecStatuleriBildirimTipleriList(tabloId);
            return result;
        }

        #endregion View Methods

        #region Api Methods

        //--------------------KURUM--------------------

        #region Kurum

        /// <summary>
        /// Kurumların listesini getiren metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Kurum Listesi")]
        [HttpGet]
        [Route("/panel/kurumList")]
        public Result<List<KurumTemelBilgiler>> KurumList()
        {
            var result = _kurumService.List();
            return result;
        }

        /// <summary>
        /// Kurumları listeleyen metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Kurum Listesi")]
        [HttpGet]
        public Result<List<KurumTemelBilgiler>> KurumListForKurum()
        {
            var cook = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var result = _kurumService.List(Convert.ToInt32(cook.KurumId));
            return result;
        }

        /// <summary>
        /// Kurumları listeleyen metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Kurum Listesi")]
        [HttpGet]
        [Route("/panel/ListKendisiIle")]
        public Result<List<KurumTemelBilgiler>> ListKendisiIle()
        {
            var cook = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var result = _kurumService.ListKendisiIle(Convert.ToInt32(cook.KurumId));
            return result;
        }

        /// <summary>
        /// Kurumilişki türlerini listeleyen metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Kurum ilişki türlerini listeleme")]
        [HttpGet]
        public Result<List<ParamIliskiTurleri>> KurumIliskiTuruGetir()
        {
            var result = _genelParametrelerService.KurumIliskiTuruGetir();
            return result;
        }

        /// <summary>
        /// Kurum Müşteri Temsilcilerini listeleme
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Kurum Müşteri Temsilcilerini listeleme")]
        [HttpGet]
        [Route("/panel/MusteriTemsilcisiGetir")]
        public Result<List<GenelViewModel>> KurumMusteriTemsilcisiGetir()
        {
            var result = _kurumService.KurumMusteriTemsilcisiGetir();
            return result;
        }

        /// <summary>
        /// Kurum temel verileri kaydetme metotu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ProcessName(Name = "kurum temel verileri kaydetme")]
        [Route("/panel/KurumTemelVerileriKaydet/")]
        [HttpPost]
        public Result<KurumTemelKayitModel> KurumTemelVerileriKaydet(KurumTemelKayitModel model)
        {
            var result = _kurumService.KurumTemelVerileriKaydet(model);
            return result;
        }

        /// <summary>
        /// Id'ye göre kurum temel bilgilerini getirme metotu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProcessName(Name = "kurum temel verileri getirme")]
        [Route("/panel/KurumTemelVerileriGetir/{id}")]
        [HttpGet]
        public Result<KurumTemelKayitModel> KurumTemelVerileriGetir(int id)
        {
            var result = _kurumService.KurumTemelVerileriGetir(id);
            return result;
        }

        /// <summary>
        /// Id'ye göre kurum idari bilgilerini getirme metotu
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "kurum idari verileri getirme")]
        [Route("/panel/KurumIdariVerileriGetir/{kurumID}")]
        [HttpGet]
        public Result<KurumIdariProfilModel> KurumIdariVerileriGetir(int kurumID)
        {
            var result = _kurumService.KurumIdariVerileriGetir(kurumID);
            return result;
        }

        /// <summary>
        /// Cookie'deki kurumID getiren metod
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Cookie kurumID getiren metod")]
        [Route("/panel/cookieKurumID")]
        [HttpGet]
        public Result<int> CookieKurumID()
        {
            var cook = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            return Convert.ToInt32(cook.KurumId).ToResult();
        }

        /// <summary>
        /// Kurum ek parametre getirme metotu
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "kurum ek parametreleri getirme")]
        [Route("/panel/EkParametreListesi")]
        [HttpGet]
        public Result<List<KurumEkParametreViewModel>> EkParametreListesi()
        {
            var result = _kurumService.EkParametreListesi(null);
            return result;
        }

        /// <summary>
        /// Id'ye göre kurum ek parametre getirme metotu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProcessName(Name = "kurum ek parametreleri getirme")]
        [Route("/panel/EkParametreListesi/{id?}")]
        [HttpGet]
        public Result<List<KurumEkParametreViewModel>> EkParametreListesi(int? id)
        {
            var result = _kurumService.EkParametreListesi(id);
            return result;
        }

        /// <summary>
        /// Kurum temel verilerini güncelleme metotu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ProcessName(Name = "kurum temel verileri güncelleme")]
        [Route("/panel/KurumTemelVerileriGuncelle")]
        [HttpPost]
        public Result<KurumTemelKayitModel> KurumTemelVerileriGuncelle(KurumTemelKayitModel model)
        {
            var result = _kurumService.KurumTemelVerileriGuncelle(model);
            return result;
        }

        /// <summary>
        /// Kurum Id'ye göre temel kurum bilgilerini silindi yapan metot
        /// </summary>
        /// <param name="kurumID"></param>
        /// <returns></returns>
        [ProcessName(Name = "Kurum silme")]
        [Route("/panel/TemelKurumSilindiYap/{kurumID}")]
        [HttpGet]
        public Result<bool> TemelKurumSilindiYap(int kurumID)
        {
            var result = _kurumService.TemelKurumSilindiYap(kurumID);
            return result;
        }

        /// <summary>
        /// Kurum ek parametre kaydetme metodu
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "kurum ek parametreleri kaydetme")]
        [Route("/panel/EkParametreKayit")]
        [HttpPost]
        public Result<bool> EkParametreKayit(KurumEkParametreKayitViewModel model)
        {
            var result = _kurumService.EkParametreKayit(model);
            return result;
        }

        /// <summary>
        /// Kurumlar arası ilişkiyi kaydeden metot
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ProcessName(Name = "kurumlar arası ilişki kaydetme")]
        [Route("/panel/KurumIliskiKaydet/")]
        [HttpPost]
        public Result<Iliskiler> KurumIliskiKaydet(KurumIliskiKayitModel model)
        {
            var result = _kurumIliskiService.KurumIliskiKaydet(model);
            return result;
        }

        /// <summary>
        /// Kurumlar arası ilişkileri listeleyen metot
        /// </summary>
        /// <returns></returns>
        [Route("/panel/KurumIliskiList")]
        [HttpGet]
        [ProcessName(Name = "Kurumlar arası ilişki listeleme")]
        public Result<List<Iliskiler>> KurumIliskiList()
        {
            var cook = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var result = _kurumIliskiService.KurumIliskiList(Convert.ToInt32(cook.KurumId));

            return result;
        }

        /// <summary>
        /// Kurumlar arası ilişkiyi silindi yapan metot
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        [ProcessName(Name = "Kurum ilişki silme")]
        [Route("/panel/KurumIliskiSil/{tabloID}")]
        [HttpGet]
        public Result<bool> KurumIliskiSil(int tabloID)
        {
            var result = _kurumIliskiService.KurumIliskiSil(tabloID);
            return result;
        }

        /// <summary>
        /// Kurumlar arası ilişkiyi güncelleyen metot
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ProcessName(Name = "kurumlar arası ilişki güncelleme")]
        [Route("/panel/KurumIliskiGuncelle/")]
        [HttpPost]
        public Result<Iliskiler> KurumIliskiGuncelle(KurumIliskiKayitModel model)
        {
            var result = _kurumIliskiService.KurumIliskiGuncelle(model);
            return result;
        }

        /// <summary>
        /// Amir temsilciye göre kurum listesi getiren metod
        /// </summary>
        /// <returns></returns>
        [Route("/panel/AmirTemsilciyeGoreKurumListesi")]
        [HttpGet]
        public Result<List<KurumTemelBilgiler>> AmirTemsilciyeGoreKurumListesi()
        {
            var cook = _cookieModel;
            var result = _kurumService.AmirTemsilciyeGoreKurumListesi(cook);
            return result;
        }

        /// <summary>
        /// Kurum organizasyon birimi ekleyen method
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ProcessName(Name = "AddKurumOrganizasyonBirimi")]
        public Result<int> AddKurumOrganizasyonBirimi(KurumOrganizasyonBirimView data)
        {
            data.KurumId = Convert.ToInt32(_cookieModel.KurumId);
            return _kurumOrganizasyonBirimTanimlariService.Add(data);
        }

        /// <summary>
        /// Kurum organizasyon birimi ekleyen method
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ProcessName(Name = "AddKurumOrganizasyonBirimi2")]
        public Result<int> AddKurumOrganizasyonBirimi2(KurumOrganizasyonBirimView data)
        {
            return _kurumOrganizasyonBirimTanimlariService.AddPoz(data);
        }

        /// <summary>
        /// Kurum organizasyon birimi ekleme metodu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ProcessName(Name = "AddKurumOrganizasyonBirimiJson")]
        public Result<int> AddKurumOrganizasyonBirimiJson([FromBody] KurumOrganizasyonBirimView data)
        {
            data.KurumId = Convert.ToInt32(_cookieModel.KurumId);
            return _kurumOrganizasyonBirimTanimlariService.Add(data);
        }

        /// <summary>
        /// Kurum organizasyon birimini güncelleyen method
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [ProcessName(Name = "UpdateForKurum")]
        public Result<bool> UpdateForKurum(KurumOrganizasyonBirimView item)
        {
            item.KurumId = Convert.ToInt32(_cookieModel.KurumId);
            return _kurumOrganizasyonBirimTanimlariService.UpdateForKurum(item);
        }

        /// <summary>
        /// Kurum organizasyon birimini güncelleyen method(pozisyon)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [ProcessName(Name = "UpdateForKurum2")]
        public Result<bool> UpdateForKurum2(KurumOrganizasyonBirimView item)
        {
            return _kurumOrganizasyonBirimTanimlariService.UpdateForKurum2(item);
        }

        /// <summary>
        /// Kurum organizasyon birimini güncelleyen method
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        [ProcessName(Name = "UpdateName")]
        public Result<bool> UpdateName([FromBody] KurumOrganizasyonBirimView item)
        {
            item.KurumId = Convert.ToInt32(_cookieModel.KurumId);
            return _kurumOrganizasyonBirimTanimlariService.UpdateName(item);
        }

        ///// <summary>
        ///// Id'ye göre kurum birimlerini getiren method
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[ProcessName(Name = "GetKurumBirim")]
        //public Result<List<KurumOrganizasyonBirimView>> GetKurumBirim(int id)
        //{
        //    var request = new KurumOrganizasyonBirimRequest()
        //    { KurumId = Convert.ToInt32(_cookieModel.KurumId), UstId = id };
        //    return _kurumOrganizasyonBirimTanimlariService.GetTree(request);
        //}

        /// <summary>
        /// Ilgili Kurum Id'ye göre kurum pozisyon birimlerini getiren method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProcessName(Name = "GetKurumBirim2/{id}/{tipId}")]
        [Route("panel/GetKurumBirim2/{id}/{tipId}")]
        public Result<List<KurumOrganizasyonBirimView>> GetKurumBirim2(int id, int tipId)
        {
            var request = new KurumOrganizasyonBirimRequest()
            { UstId = tipId, IlgiliKurumID = id };
            return _kurumOrganizasyonBirimTanimlariService.GetTree2(request);
        }

        /// <summary>
        /// Id'ye göre kurum birimlerini listeleyen method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProcessName(Name = "GetKurumBirimList")]
        public Result<List<KurumOrganizasyonBirimView>> GetKurumBirimList(int id)
        {
            var request = new KurumOrganizasyonBirimRequest()
            { KurumId = Convert.ToInt32(_cookieModel.KurumId), UstId = id };
            return _kurumOrganizasyonBirimTanimlariService.List(request);
        }

        /// <summary>
        /// Id'ye göre kurum birimlerini listeleyen method(Pozisyon)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProcessName(Name = "GetKurumBirimList2")]
        [Route("/panel/GetKurumBirimList2/{id}/{tipId}")]
        public Result<List<KurumOrganizasyonBirimView>> GetKurumBirimList2(int id, int tipId)
        {
            var request = new KurumOrganizasyonBirimRequest()
            { KurumId = Convert.ToInt32(_cookieModel.KurumId), UstId = tipId, IlgiliKurumID = id };
            return _kurumOrganizasyonBirimTanimlariService.List2(request);
        }

        /// <summary>
        /// Id'ye göre kurum birim hiyararşisine uygun şekilde kurumları getiren method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProcessName(Name = "GetKurumBirimListForLevel")]
        public Result<List<KurumOrganizasyonBirimView>> GetKurumBirimListForLevel(int id)
        {
            var request = new KurumOrganizasyonBirimRequest()
            { KurumId = Convert.ToInt32(_cookieModel.KurumId), UstId = id };
            return _kurumOrganizasyonBirimTanimlariService.ListForLevel(request);
        }

        /// <summary>
        /// Id'ye göre kurum birim hiyararşisine uygun şekilde kurumları getiren method(Pozisyon)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="IlgiliKurumId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/panel/GetKurumBirimListForLevel2/{id}/{IlgiliKurumId}")]
        [ProcessName(Name = "GetKurumBirimListForLevel2")]
        public Result<List<KurumOrganizasyonBirimView>> GetKurumBirimListForLevel2(int id, int IlgiliKurumId)
        {
            var request = new KurumOrganizasyonBirimRequest()
            { KurumId = Convert.ToInt32(_cookieModel.KurumId), UstId = id, IlgiliKurumID = IlgiliKurumId };
            return _kurumOrganizasyonBirimTanimlariService.ListForLevel2(request);
        }

        /// <summary>
        /// Kurum organizasyon birimini silen metot
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProcessName(Name = "DeleteKurumBirim")]
        public Result<bool> DeleteKurumBirim(int id)
        {
            return _kurumOrganizasyonBirimTanimlariService.Delete(id);
        }

        /// <summary>
        /// Kurum bünyesinde mesajlaşma modülü kullanmasına izin verilmeyen pozisyonların kayıt işleminin gerçekleştiren metot
        /// </summary>
        /// <param name="model">model</param>
        /// <returns>sonucu döndürür.</returns>
        [HttpPost]
        [Route("/panel/IzinVerilmeyenPozisyonKaydi")]
        public Result<bool> IzinVerilmeyenPozisyonKaydi([FromBody] KurumMesajlasmaModuluIzinIslemleriViewModel model)
        {
            //var result = _requestHelper.Post<Result<bool>>("KurumService/ModuleIzinVerilmeyenPozisyonKaydi", model)
            model.KurumID = Convert.ToInt32(_cookieModel.KurumId);
            model.KisiID = Convert.ToInt32(_cookieModel.KisiId);
            var result = _kurumMesajlasmaModuluIzinIslemleriService.IzinVerilmeyenPozisyonKaydi(model);
            return result;
        }

        /// <summary>
        /// Mesajlaşma modülü kulllanımına izin verilmeyen pozisyon kayıtlarını silen metot
        /// </summary>
        /// <param name="id"> silinecek kayıt Id değeri</param>
        /// <returns>sonucu döndürür.</returns>
        [HttpGet]
        [Route("/panel/DeleteRecord/{id}")]
        public Result<bool> DeleteRecord(int id)
        {
            var result = _kurumMesajlasmaModuluIzinIslemleriService.DeleteRecord(id);
            return result;
        }

        /// <summary>
        /// Meesajlaşma modülü kullanımına izin veirlmeyen pozisyonları listeleyen metot.
        /// </summary>
        /// <returns>sonuçlar listesini döndürür.</returns>
        [HttpGet]
        [Route("/panel/messagePermissionListForView")]
        public Result<List<KurumMesajlasmaModuluIzinIslemleriViewModel>> ListForView()
        {
            var result = _kurumMesajlasmaModuluIzinIslemleriService.ListForView();
            return result;
        }

        #endregion Kurum

        //-------------------KİSİ----------------------

        #region Kisi

        /// <summary>
        /// Temel kişi kaydı yapan metot
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ProcessName(Name = "Temel kişi kaydı işlemi")]
        [HttpPost]
        public Result<KisiTemelKayitModel> KisiTemelKaydet(KisiTemelKayitModel model)
        {
            var result = _kisiService.KisiTemelKaydet(model);
            return result;
        }

        /// <summary>
        /// Kişi Id'ye göre Kişi temel kayıt verilerini getiren metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        [ProcessName(Name = "Temel kişi verilerinin getirilmesi işlemi")]
        [Route("/panel/KisiTemelKayitVerileriGetir/{kisiID}")]
        [HttpGet]
        public Result<KisiTemelKayitModel> KisiTemelKayitVerileriGetir(int kisiID)
        {
            var result = _kisiService.KisiTemelKayitVerileriGetir(kisiID);
            return result;
        }

        /// <summary>
        /// Kişi temel kayıt verilerini güncelleyen metot
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ProcessName(Name = "Temel kişi verilerinin güncellenmesi işlemi")]
        [HttpPost]
        public Result<KisiTemelKayitModel> KisiTemelKayitVerileriGuncelle(KisiTemelKayitModel model)
        {
            var result = _kisiService.KisiTemelKayitVerileriGuncelle(model);
            return result;
        }

        /// <summary>
        /// Temel Kişi Listesi getiren metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Temel kişi verilerinin listelenmesi işlemi")]
        [HttpGet]
        public Result<List<KisiListeModel>> TemelKisiListesiGetir()
        {
            var result = _kisiService.TemelKisiListesiGetir();
            return result;
        }

        /// <summary>
        /// Kişi Id'ye göre Kişiyi aktif yapan metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        [ProcessName(Name = "Kisi aktifleştirme")]
        [Route("/panel/TemelKisiAktifYap/{kisiID}")]
        [HttpGet]
        public Result<bool> TemelKisiAktifYap(int kisiID)
        {
            var result = _kisiService.TemelKisiAktifYap(kisiID);
            return result;
        }

        /// <summary>
        /// Kişi Id'ye göre Kişiyi silindi yapan metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        [ProcessName(Name = "Kisi silme")]
        [Route("/panel/TemelKisiSilindiYap/{kisiID}")]
        [HttpGet]
        public Result<bool> TemelKisiSilindiYap(int kisiID)
        {
            var result = _kisiService.TemelKisiSilindiYap(kisiID);
            return result;
        }

        /// <summary>
        /// Kişi Id'ye göre kişi temel kayıt verilerini getiren metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        [ProcessName(Name = "Kisi verilerini view model ile getirme")]
        [Route("/panel/KisiTemelKayitVerileriGetirView/{kisiID}")]
        [HttpGet]
        public Result<KisiTemelViewModel> KisiTemelKayitVerileriGetirView(int kisiID)
        {
            var result = _kisiService.KisiTemelKayitVerileriGetirView(kisiID);
            return result;
        }

        /// <summary>
        /// Pasif Kişi Listesi getiren metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Pasif kişi verilerinin listelenmesi işlemi")]
        [HttpGet]
        public Result<List<KisiTemelBilgiler>> PasifKisiListesiGetir()
        {
            var result = _kisiService.PasifKisiListesiGetir();
            return result;
        }

        /// <summary>
        /// Kişiler arası ilişkileri listeleyen metot
        /// </summary>
        /// <returns></returns>
        [Route("/panel/KisiIliskiList")]
        [HttpGet]
        [ProcessName(Name = "Kişiler arası ilişki listeleme")]
        public Result<List<Iliskiler>> KisiIliskiList()
        {
            var cook = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var result1 = new List<Iliskiler>();
            var result = _kisiIliskiService.KisiIliskiList(Convert.ToInt32(cook.KurumId));
            for (int i = 0; i < result.Value.Count; i++)
            {
                if (result.Value[i].AktifMi == 1)
                {
                    result1.Add(result.Value[i]);
                }
            }
            return result1.ToResult();
        }

        /// <summary>
        /// Kişi ilişki türlerini listeleyen metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Kişi ilişki türlerini listeleme")]
        [HttpGet]
        public Result<List<ParamIliskiTurleri>> KisiIliskiTuruGetir()
        {
            var result = _genelParametrelerService.KisiIliskiTuruGetir();
            return result;
        }

        /// <summary>
        /// Kişiler arası ilişkiyi kaydeden metot
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ProcessName(Name = "kişiler arası ilişki kaydetme")]
        [Route("/panel/KisiIliskiKaydet/")]
        [HttpPost]
        public Result<Iliskiler> KisiIliskiKaydet(KisiIliskiKayitModel model)
        {
            var result = _kisiIliskiService.KisiIliskiKaydet(model);
            return result;
        }

        /// <summary>
        /// Kişiler arası ilişkiyi güncelleyen metot
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ProcessName(Name = "kişiler arası ilişki güncelleme")]
        [Route("/panel/KisiIliskiGuncelle/")]
        [HttpPost]
        public Result<Iliskiler> KisiIliskiGuncelle(KisiIliskiKayitModel model)
        {
            var result = _kisiIliskiService.KisiIliskiGuncelle(model);
            return result;
        }

        /// <summary>
        /// Kişiler arası ilişkiyi silindi yapan metot
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        [ProcessName(Name = "Kişi ilişki silme")]
        [Route("/panel/KisiIliskiSil/{tabloID}")]
        [HttpGet]
        public Result<bool> KisiIliskiSil(int tabloID)
        {
            var result = _kisiIliskiService.KisiIliskiSil(tabloID);
            return result;
        }

        /// <summary>
        /// Kurum organizasyon tablosundan Id'lerle kişi listeleme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("/panel/KisiIdileKisiListele")]
        [HttpPost]
        public Result<List<KisiTemelBilgiler>> KisiIdileKisiListele([FromBody] List<int> model)
        {
            var response = _kisiService.IdlereGoreKisileriGetir(model);

            return response;
        }

        /// <summary>
        /// Kişi kayıt sayfasını excele aktaran metod
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [Route("/panel/ExcelKisiKayit")]
        [HttpPost]
        public Result<bool> ExcelIleKisiKayit(IFormFile file)
        {
            var result = _excelReadWriteHandler.KisiListExcelImport(file);
            return result;
        }

        /// <summary>
        /// Amir temsilciye göre kişi listesini getiren metod
        /// </summary>
        /// <returns></returns>
        [Route("/panel/AmirTemsilciyeGoreKisiListesi")]
        [HttpGet]
        public Result<List<KisiListeModel>> AmirTemsilciyeGoreKisiListesi()
        {
            var cook = _cookieModel;
            var result = _kisiService.AmirTemsilciyeGoreKisiListesi(cook);
            return result;
        }

        /// <summary>Kişi müşteri temsilcisi mi kontrolü sağlayan metot.
        /// </summary>
        /// <param name="kisiId">kişi Id</param>
        /// <returns>sonucu true veya false olarak döndürür.</returns>
        [Route("/panel/KisiMusteriTemsilcisiMi/{kisiId}")]
        [HttpGet]
        public Result<bool> KisiMusteriTemsilcisiMi(int kisiId)
        {
            var result = _kurumService.KisiMusteriTemsilcisiMi(kisiId);
            return result;
        }

        /// <summary>
        /// Kişi ek parametre getirme metodu
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "kisi ek parametreleri getirme")]
        [Route("/panel/KisiEkParametreListesi")]
        [HttpGet]
        public Result<List<KisiEkParametreViewModel>> KisiEkParametreListesi()
        {
            var result = _kisiService.EkParametreListesi(null);
            return result;
        }

        /// <summary>
        /// Id'ye göre kisi ek parametre getirme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProcessName(Name = "kisi ek parametreleri getirme")]
        [Route("/panel/KisiEkParametreListesi/{id?}")]
        [HttpGet]
        public Result<List<KisiEkParametreViewModel>> KisiEkParametreListesi(int? id)
        {
            var result = _kisiService.EkParametreListesi(id);
            return result;
        }

        /// <summary>
        /// Kişi ek parametre kaydetme metodu
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "kisi ek parametreleri kaydetme")]
        [Route("/panel/KisiEkParametreKayit")]
        [HttpPost]
        public Result<bool> KisiEkParametreKayit(KisiEkParametreKayitViewModel model)
        {
            var result = _kisiService.EkParametreKayit(model);
            return result;
        }

        /// <summary>
        /// Cookie oluşturulanv ve Kisi Temel Bilgilerinin getirildiği method.
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "kişi verilerinin profile getirilmesi")]
        public KisiTemelBilgiler GetUserProfile()
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            if (cookie != null)
            {
                var kisiResult = _kisiService.SingleOrDefault(Convert.ToInt32(cookie.KisiId));
                if (kisiResult.IsSuccess)
                {
                    return kisiResult.Value;
                }
            }

            return null;
        }

        [ProcessName(Name = "kişi organizasyon bilgileriningetirilmesi")]
        public IActionResult GetUserOrganizasyon()
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            if (cookie != null)
            {
                var paramOrganizasyonBirimleri = _paramOrganizasyonBirimleriService.List();
                var kisiResult = _kisiService.KisiOrganizasyonListGetir(Convert.ToInt32(cookie.KisiId));
                if (kisiResult.IsSuccess)
                {
                    var sonuc = (from paramBirim in paramOrganizasyonBirimleri.Value
                                 join kisi in kisiResult.Value on paramBirim.TabloId equals kisi.OrganizasyonBirimTipiId
                                 select new { paramBirim.TabloId, paramBirim.Tanim,kisi.BirimTanim }
                              ).ToList();

                    return Ok(sonuc);
                }
            }

            return null;
        }



        /// <summary>
        /// Kişi Temel bilgilerinin güüncellendiği method
        /// </summary>
        /// <param name="kisiItem">The kisi item.</param>
        /// <returns></returns>
        [ProcessName(Name = "Kişi profil güncellenmesi")]
        [HttpPost]
        public bool UpdateProfile(KisiTemelBilgiler kisiItem)
        {
            return _kisiService.Update(kisiItem).Value;
        }

        #endregion Kisi

        //-------------------Iys-----------------------

        #region Iys




        /// <summary>
        /// Cinsiyet listesini getiren metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Cinsiyet listesinin getirilmesi işlemi")]
        [HttpGet]
        public Result<List<ParamCinsiyet>> CinsiyetList()
        {
            var result = _genelParametrelerService.CinsiyetList();
            return result;
        }

        /// <summary>
        /// Ülke listesini getiren metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Ülke listesinin getirilmesi işlemi")]
        [HttpGet]
        public Result<List<ParamUlkeler>> UlkeList()
        {
            var result = _genelParametrelerService.UlkeList();
            return result;
        }

        /// <summary>
        /// Şehir listesini getiren metot
        /// </summary>
        /// <param name="ulkeID"></param>
        /// <returns></returns>
        [ProcessName(Name = "Şehir listesinin getirilmesi işlemi")]
        [Route("/panel/SehirList/{ulkeID}")]
        [HttpGet]
        public Result<List<ParamUlkeler>> SehirList(int ulkeID)
        {
            var result = _genelParametrelerService.SehirList(ulkeID);
            return result;
        }

        /// <summary>
        /// Banka listesini getiren metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Ülke listesinin getirilmesi işlemi")]
        [HttpGet]
        public Result<List<ParamBankalar>> BankaList()
        {
            var result = _genelParametrelerService.BankaList();
            return result;
        }

        /// <summary>
        /// Şehir listesini getiren metot
        /// </summary>
        /// <param name="bankaID"></param>
        /// <returns></returns>
        [ProcessName(Name = "Şehir listesinin getirilmesi işlemi")]
        [Route("/panel/SubeList/{bankaID}")]
        [HttpGet]
        public Result<List<ParamBankalar>> SubeList(int bankaID)
        {
            var result = _genelParametrelerService.SubeList(bankaID);
            return result;
        }

        /// <summary>
        /// Medeni hal listesini getiren metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Medeni hal listesinin getirilmesi işlemi")]
        [HttpGet]
        public Result<List<ParamMedeniHal>> MedeniHalList()
        {
            var result = _genelParametrelerService.MedeniHalList();
            return result;
        }

        /// <summary>
        /// Adres tipi listesini getiren metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Adres tipi listesinin getirilmesi işlemi")]
        [HttpGet]
        public Result<List<ParamAdresTipi>> AdresTipiList()
        {
            var result = _genelParametrelerService.AdresTipiList();
            return result;
        }

        /// <summary>
        /// Okul tipi listesini getiren metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Okul tipi listesinin getirilmesi işlemi")]
        [HttpGet]
        public Result<List<ParamOkulTipi>> OkulTipiList()
        {
            var result = _genelParametrelerService.OkulTipiList();
            return result;
        }

        /// <summary>
        /// Telefon tipi listesini getiren metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Telefon tipi listesinin getirilmesi işlemi")]
        [HttpGet]
        public Result<List<ParamTelefonTipi>> TelefonTipiList()
        {
            var result = _genelParametrelerService.TelefonTipiList();
            return result;
        }

        /// <summary>
        /// Yabancı dil seviyesi listesini getiren metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Yabancı dil seviyesi listesinin getirilmesi işlemi")]
        [HttpGet]
        public Result<List<ParamDilSeviyesi>> YabanciDilSeviyesiList()
        {
            var result = _genelParametrelerService.YabanciDilSeviyesiList();
            return result;
        }

        /// <summary>
        /// Organizasyon birim tipine göre listeyi getiren metot
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProcessName(Name = "Organizasyon birim tipine göre listenin getirilmesi işlemi")]
        [Route("/panel/ListTip")]
        [HttpPost]
        public Result<List<KurumOrganizasyonBirimView>> ListTip(KurumOrganizasyonBirimRequest request)
        {
            var result = _paramOrganizasyonBirimleriService.ListTip(request);
            return result;
        }

        /// <summary>
        /// Kişi Id'ye göre Sistem dillerini getiren metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        [ProcessName(Name = "KSistem Dili listesi getirme")]
        [Route("/panel/SistemDilleriGetir/")]
        [HttpGet]
        public Result<List<ParamDiller>> SistemDilleriGetir(int kisiID)
        {
            var result = _dilService.List();
            return result;
        }
        /// <summary>
        /// yabancıDilleri getiren metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "kişinin bildiği yaabancı diller seçimi için gerekli yabancıdilleri getiren metod")]
        [Route("/panel/YabanciDilList/")]
        [HttpGet]
        public Result<List<ParamYabanciDiller>> YabanciDilList()
        {
            var result = _dilService.YabanciDilList();
            return result;
        }

        /// <summary>
        /// Sistem dillerini listeleyen metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Sistem Dili listesi")]
        [Route("/panel/SistemDilList")]
        [HttpGet]
        public Result<List<ParamDiller>> SistemDilList()
        {
            var result = _genelParametrelerService.SistemDilList();
            return result;
        }


        /// <summary>
        /// Organizasyon birimlerini listeleyen method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProcessName(Name = "GetOrganizasyonBirim")]
        public Result<List<ParamBirimTanimView>> GetOrganizasyonBirim()
        {
            return _paramOrganizasyonBirimleriService.List();
        }

        /// <summary>
        /// Çalışan sayılarını listeleyen metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Çalışan sayıları Listesi")]
        [HttpGet]
        public Result<List<ParamCalisanSayilari>> CalisanSayilariList()
        {
            var result = _genelParametrelerService.CalisanSayilariList();
            return result;
        }

        /// <summary>
        /// Kurum lokasyon tipi listeleyen metot
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Kurum lokasyon tipleri listesi getirme")]
        [Route("/panel/KurumLokasyonTipiList/")]
        [HttpGet]
        public Result<List<ParamKurumLokasyonTipi>> KurumLokasyonTipiList()
        {
            var result = _genelParametrelerService.KurumLokasyonTipiList();
            return result;
        }

        #endregion Iys

        //------------------Diger----------------------

        #region Diger

        /// <summary>
        /// Yetki merkezi için sayfa verilerini getiren method
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/panel/YetkiIcinSayfaGetir")]
        public Result<List<SistemSayfalari>> YetkiIcinSayfaGetir()
        {
            var result = _menuService.YetkiIcinSayfaGetir();
            return result;
        }

        /// <summary>
        /// Bildirim getirme metodu
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/panel/GetNotification")]
        [ProcessName(Name = "GetNotification")]
        public Result<NotificationList> GetNotification()
        {
            return _notificationService.List();
        }

        /// <summary>
        /// Bildirim  görüldü yapan metod
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/panel/SetSeen")]
        [ProcessName(Name = "SetSeen")]
        public Result<bool> SetSeen()
        {
            return _notificationService.SetSeen();
        }

        /// <summary>
        /// Bildirime dair aksiyon alındığında (misal bildirime tıklandığında) bu işlemin loglanmasını sağlayan metot.
        /// </summary>
        /// <param name="model">loglanacak bildirim aksiyonu modeli.</param>
        /// <returns> işlem sonucuna göre true döndürür.</returns>
        [Route("/panel/BildirimAksiyonLog")]
        [HttpPost]
        public Result<bool> LogNotificationAction(BildirimAksiyonLogModel model)
        {
            var cook = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var kisi = _kisiService.SingleOrDefault(Convert.ToInt32(cook.KisiId)).Value;
            model.KurumId = cook.KurumId;
            model.UserId = cook.KisiId;
            model.Mail = kisi.KisiEposta;
            model.Name = kisi.KisiAdi + " " + kisi.KisiSoyadi;
            model.Islem = "Bildirim Aksiyonu";
            model.Date = DateTime.Now;
            var _logger = _serviceProvider.GetService<ILogger<BildirimAksiyonLogModel>>();

            _logger.LogInformation(JsonConvert.SerializeObject(model));
            return true.ToResult();
        }

        /// <summary>
        /// Son giriş tarihini getirme metodu
        /// </summary>
        /// <returns></returns>
        public Result<string> SonGiris()
        {
            return _loginRegisterService.SonGiris();
        }

        /// <summary>
        /// Kisiye sifre atama islemi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("/panel/SifreAtama")]
        [HttpPost]
        public Result<bool> SifreAtama(SifreAtamaModel model)
        {
            var result = _kisiService.SifreAtama(model);
            return result;
        }

        /// <summary>
        /// mesajlar içeriğinde arama yapma metodu
        /// </summary>
        /// <param name="query">aranacak keliöme</param>
        /// <returns>İlgili sonuçları döndürür.</returns>
        [HttpGet]
        [Route("/panel/MesajlarIcindeArama/{query}")]
        public Result<List<MesajAramaSonucModel>> MesajlarIcindeArama(string query)
        {
            var result = _searchService.MesajlarIcindeArama(query);
            return result;
        }

        /// <summary>
        /// Parola güncelleme metotu
        /// </summary>
        /// <param name="sifreModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ProcessName(Name = "Şifre güncelleme işlemi")]
        public IActionResult UpdatePassword(SifreModel sifreModel)
        {
            var result = _kisiService.SifreDeğistir(sifreModel);
            return Json(result);
        }

        /// <summary>
        /// Kültürün ayarlandığı methodtur.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        [HttpPost]
        public bool SetCulture(string culture)
        {
            _localizationService.SetCulture(culture);
            return true;
        }

        /// <summary>
        /// Resmin yüklendiği method.
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "sisteme resim yüklenmesi")]
        [HttpPost]
        public IActionResult ResimYukle(IFormFile file)
        {
            var result = _medyaKutuphanesiService.ResimYukle(file);
            return Json(result);
        }  
        [ProcessName(Name = "sisteme resim yüklenmesi")]
        [HttpPost]
        public IActionResult ResimYukleIcerik(IFormFile file)
        {
            var result = _medyaKutuphanesiService.ResimYukleIcerik(file);
            return Json(result);
        }

        /// <summary>
        /// Çoklu dosyaların yüklendiği method.
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "sisteme çoklu dosya yüklenmesi")]
        [HttpPost]
        public IActionResult UploadMultiple(List<IFormFile> files)
        {
            var result = _medyaKutuphanesiService.UploadMultiple(files);
            return Json(result);
        }

        /// <summary>
        /// Çoklu dosyaların yüklendiği method.
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "sisteme çoklu dosya yüklenmesi")]
        [HttpPost]
        public IActionResult UploadChunk(ChunkModel model)
        {
            var result = _medyaKutuphanesiService.UploadChunk(model);
            return Json(result);
        }




        /// <summary>
        /// Online kişi sayısını getiren method.
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Online kişi sayısının getirilmesi")]
        [HttpGet]
        public IActionResult GetOnlineUserCount()
        {
            var result = _menuService.GetOnlineUserCount();
            return Json(result);
        }

       

        /// <summary>
        /// Sistemdeki üyeleri getirir
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Sistemdeki üyeleri getirir")]
        [Route("UyeleriGetir/List")]
        [HttpGet]
        public Result<List<UyeListViewModel>> UyeleriGetir()
        {
            var result = _loginRegisterService.UyeleriGetir();
            return result;
        }

        /// <summary>
        /// Kurum organizasyon tablosundan Id'lerle kişi listeleme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("/Panel/ListKurumOrganizasyonIdileKisiListele")]
        [HttpPost]
        public Result<List<KisiTemelBilgiler>> ListKurumOrganizasyonIdileKisiListele([FromBody] List<string> model)
        {
            var data = new List<int>();
            model.ForEach(x => data.Add(int.Parse(x)));

            var result = _paramOrganizasyonBirimleriService.ListKurumOrganizasyonIdileKisiListele(data);
            var response = _kisiService.IdlereGoreKisileriGetir(result.Value);

            return response;
        }

        #endregion Diger

        #endregion Api Methods
    }
}