using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Baz.ElasticSearch.Abstract;
using Baz.ElasticSearch.Model;
using Baz.Model.Entity.ViewModel;
using Baz.RequestManager;
using BazWebApp.Handlers;
using BazWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WebAppUnitTests.Helper;

namespace WebAppUnitTests
{
    [TestClass()]
    public class PanelControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly PanelController _panelController;

        public PanelControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            //new RequestHelper {_headers = _helper._headers}
            var localizationService = _testServer.Services.GetService<ILocalizationService>();
            var bazCookieService = _testServer.Services.GetService<IBazCookieService>();
            var kisiService = _testServer.Services.GetService<IKisiService>();
            var medyaKutuphanesiService = _testServer.Services.GetService<IMedyaKutuphanesiService>();
            var menuService = _testServer.Services.GetService<IMenuService>();
            var paramOrganizasyonBirimleriService = _testServer.Services.GetService<IParamOrganizasyonBirimleriService>();
            var kurumOrganizasyonBirimTanimlariService = _testServer.Services.GetService<IKurumOrganizasyonBirimTanimlariService>();
            var genelParametrelerService = _testServer.Services.GetService<IGenelParametrelerService>();
            var kurumService = _testServer.Services.GetService<IKurumService>();
            var dilService = _testServer.Services.GetService<IDilService>();
            var loginRegisterService = _testServer.Services.GetService<ILoginRegisterService>();
            var kurumIliskiService = _testServer.Services.GetService<IKurumIliskiService>();
            var kisiIliskiService = _testServer.Services.GetService<IKisiIliskiService>();
            var searchService = _testServer.Services.GetService<ISearchService>();
            var notificationService = _testServer.Services.GetService<INotificationService>();
            _testServer.Services.GetService<IElasticService<KurumKisiSearchModel>>();
            var serviceProvider = _testServer.Services.GetService<IServiceProvider>();
            var excelReadWriteHandler = _testServer.Services.GetService<IExcelReadWriteHandler>();
            var kurumLisansService = _testServer.Services.GetService<IKurumLisansService>();

            var kurumMesajlasmaModuluIzinIslemleriService = _testServer.Services.GetService<IKurumMesajlasmaModuluIzinIslemleriService>();

            _panelController = new PanelController(localizationService, bazCookieService, kisiService,
                medyaKutuphanesiService, menuService, paramOrganizasyonBirimleriService,
                kurumOrganizasyonBirimTanimlariService, genelParametrelerService, kurumService, dilService,
                loginRegisterService, kurumIliskiService, kisiIliskiService, searchService, notificationService, kurumLisansService, serviceProvider, kurumMesajlasmaModuluIzinIslemleriService, excelReadWriteHandler);
        }

        [TestMethod()]
        public void ViewPanelController()
        {
            _helper.Get<ViewResult>("/panel/Index");

            //Act - 2 KisiGuncelle
            var actual1 = _panelController.KisiGuncelle(1) as ViewResult;
            _helper.Get<ViewResult>("/panel/PersonUpdate/" + 129);
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual1.ViewName) || actual1.ViewName == "KisiGuncelleme");

            //Act - 3 KisiListesi
            var actual2 = _panelController.KisiListesi() as ViewResult;
            _helper.Get<ViewResult>("/panel/PersonList");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual2.ViewName) || actual2.ViewName == "KisiListesi");

            //Act - 4 KisiTanimlama
            var actual3 = _panelController.KisiTanimlama() as ViewResult;
            _helper.Get<ViewResult>("/panel/PersonDefinition");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual3.ViewName) || actual3.ViewName == "KisiTanimlama");

            //Act - 5 KisiTemelProfil
            var actual4 = _panelController.KisiTemelProfil(1) as ViewResult;
            _helper.Get<ViewResult>("/panel/PersonDetail/" + 129);
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual4.ViewName) || actual4.ViewName == "KisiTemelProfil");

            //Act - 6 KurumListesi
            var actual5 = _panelController.KurumListesi() as ViewResult;
            _helper.Get<ViewResult>("/panel/CompanyOrganizationDefinition");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual5.ViewName) || actual5.ViewName == "KurumListesi");

            //Act - 7 KurumTemelGuncelle
            var actual6 = _panelController.KurumTemelGuncelle() as ViewResult;
            _helper.Get<ViewResult>("/panel/CompanyUpdate/" + 82);
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual6.ViewName) || actual6.ViewName == "KurumTemelGuncelle");

            //Act - 8 KurumTemelKayit
            var actual7 = _panelController.KurumTemelKayit() as ViewResult;
            _helper.Get<ViewResult>("/panel/CompanyDefinition");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual7.ViewName) || actual7.ViewName == "KurumTemelKayit");

            //Act - 9 KurumTemelProfil
            var actual8 = _panelController.KurumTemelProfil(1) as ViewResult;
            _helper.Get<ViewResult>("/panel/CompanyDetail/" + 82);
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual8.ViewName) || actual8.ViewName == "KurumTemelProfil");

            //Act - 10 KurumlarArasiIliskiTanimlama
            var actual9 = _panelController.KurumlarArasiIliskiTanimlama() as ViewResult;
            _helper.Get<ViewResult>("/panel/RelationshipBetweenCompaniesDefinition");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual9.ViewName) || actual9.ViewName == "KurumlarArasiIliskiTanimlama");

            //Act - 11 Update
            var actual10 = _panelController.Update() as ViewResult;
            _helper.Get<ViewResult>("/panel/profile/update");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual10.ViewName) || actual10.ViewName == "Update");

            //Act - 12 Profile
            var actual11 = _panelController.Profile() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual11.ViewName) || actual11.ViewName == "Profile");

            //Act - 13 KurumOrganizasyon
            var actual12 = _panelController.KurumOrganizasyon() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual12.ViewName) || actual12.ViewName == "KurumOrganizasyon");

            //Act - 14 KurumOrganizasyonL
            var actual13 = _panelController.KurumOrganizasyonL() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual13.ViewName) || actual13.ViewName == "KurumOrganizasyonL");

            //Act - 15 SistemIdariProfil
            var actual14 = _panelController.SistemIdariProfil() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual14.ViewName) || actual14.ViewName == "SistemIdariProfil");

            //Act - 16 KurumIdariProfil
            var actual15 = _panelController.KurumIdariProfil() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual15.ViewName) || actual15.ViewName == "KurumIdariProfil");

            //Act - 17 KisilerArasiIliskiTanimlama
            var actual16 = _panelController.KisilerArasiIliskiTanimlama() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual16.ViewName) || actual16.ViewName == "KisilerArasiIliskiTanimlama");

            //Act - 18 MessageRecordsSearch
            var actual17 = _panelController.MessageRecordsSearch() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual17.ViewName) || actual17.ViewName == "MessageRecordsSearch");

            //Act - 19 CompanyMessageModulePermissions
            var actual18 = _panelController.CompanyMessageModulePermissions() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual18.ViewName) || actual18.ViewName == "CompanyMessageModulePermissions");

            //Act - 5 SettingsPanel
            var actual19 = _panelController.SettingsPanel() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual19.ViewName) || actual19.ViewName == "SettingsPanel");
        }

        [TestMethod()]
        public void PanelIysController()
        {
            //Assert DilList
            var DilList = _panelController.DilList();
            Assert.IsTrue(DilList.IsSuccess);
            Assert.IsNotNull(DilList.Value);
            //Assert DinlerList
            var DinlerList = _panelController.DinlerList();
            Assert.IsTrue(DinlerList.IsSuccess);
            Assert.IsNotNull(DinlerList.Value);
            //Assert CinsiyetList
            var CinsiyetList = _panelController.CinsiyetList();
            Assert.IsTrue(CinsiyetList.IsSuccess);
            Assert.IsNotNull(CinsiyetList.Value);
            //Assert UlkeList
            var UlkeList = _panelController.UlkeList();
            Assert.IsTrue(UlkeList.IsSuccess);
            Assert.IsNotNull(UlkeList.Value);
            //Assert SehirList
            var SehirList = _panelController.SehirList(UlkeList.Value[0].TabloID);
            Assert.IsTrue(SehirList.IsSuccess);
            Assert.IsNotNull(SehirList.Value);
            //Assert BankaList
            var BankaList = _panelController.BankaList();
            Assert.IsTrue(BankaList.IsSuccess);
            Assert.IsNotNull(BankaList.Value);
            //Assert SubeList
            //var SubeList = _panelController.SubeList(BankaList.Value[0].TabloID);
            //Assert.IsTrue(SubeList.IsSuccess);
            //Assert.IsNotNull(SubeList.Value);
            //Assert MedeniHalList
            var MedeniHalList = _panelController.MedeniHalList();
            Assert.IsTrue(MedeniHalList.IsSuccess);
            Assert.IsNotNull(MedeniHalList.Value);
            //Assert AdresTipiList
            var AdresTipiList = _panelController.AdresTipiList();
            Assert.IsTrue(AdresTipiList.IsSuccess);
            Assert.IsNotNull(AdresTipiList.Value);
            //Assert OkulTipiList
            var OkulTipiList = _panelController.OkulTipiList();
            Assert.IsTrue(OkulTipiList.IsSuccess);
            Assert.IsNotNull(OkulTipiList.Value);
            //Assert TelefonTipiList
            var TelefonTipiList = _panelController.TelefonTipiList();
            Assert.IsTrue(TelefonTipiList.IsSuccess);
            Assert.IsNotNull(TelefonTipiList.Value);
            //Assert YabanciDilSeviyesiList
            var YabanciDilSeviyesiList = _panelController.YabanciDilSeviyesiList();
            Assert.IsTrue(YabanciDilSeviyesiList.IsSuccess);
            Assert.IsNotNull(YabanciDilSeviyesiList.Value);
            //Assert ListTip
            var ListTip = _panelController.ListTip(new KurumOrganizasyonBirimRequest()
            {
                IlgiliKurumID = 82,
                Name = "Rol",
                UstId = 0
            });
            Assert.IsTrue(ListTip.IsSuccess);
            Assert.IsNotNull(ListTip.Value);
            //Assert SistemDilleriGetir
            var SistemDilleriGetir = _panelController.SistemDilleriGetir(UlkeList.Value[0].KisiID);
            Assert.IsTrue(SistemDilleriGetir.IsSuccess);
            Assert.IsNotNull(SistemDilleriGetir.Value);
            //Assert SistemDilList
            var SistemDilList = _panelController.SistemDilList();
            Assert.IsTrue(SistemDilList.IsSuccess);
            Assert.IsNotNull(SistemDilList.Value);
            //Assert VergiDaireleriList
            var VergiDaireleriList = _panelController.VergiDaireleriList();
            Assert.IsTrue(VergiDaireleriList.IsSuccess);
            Assert.IsNotNull(VergiDaireleriList.Value);
            //Assert GetOrganizasyonBirim
            var GetOrganizasyonBirim = _panelController.GetOrganizasyonBirim();
            Assert.IsTrue(GetOrganizasyonBirim.IsSuccess);
            Assert.IsNotNull(GetOrganizasyonBirim.Value);
            //Assert CalisanSayilariList
            var CalisanSayilariList = _panelController.CalisanSayilariList();
            Assert.IsTrue(CalisanSayilariList.IsSuccess);
            Assert.IsNotNull(CalisanSayilariList.Value);
            //Assert KurumLokasyonTipiList
            var KurumLokasyonTipiList = _panelController.KurumLokasyonTipiList();
            Assert.IsTrue(KurumLokasyonTipiList.IsSuccess);
            Assert.IsNotNull(KurumLokasyonTipiList.Value);

            //Assert UlkeMaskGetir
            var ulkeMaskGetir = _panelController.UlkeMaskGetir();
            Assert.IsTrue(ulkeMaskGetir.IsSuccess);
            Assert.IsNotNull(ulkeMaskGetir.Value);
        }
    }
}