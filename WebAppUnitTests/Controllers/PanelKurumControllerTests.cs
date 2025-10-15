using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Baz.ElasticSearch.Abstract;
using Baz.ElasticSearch.Model;
using Baz.Model.Entity;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager;
using BazWebApp.Handlers;
using BazWebApp.Services;
using Castle.Components.DictionaryAdapter;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using WebAppUnitTests.Helper;

namespace WebAppUnitTests
{
    [TestClass()]
    public class PanelKurumControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly PanelController _panelController;

        public PanelKurumControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper { _headers = _helper._headers };
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
            var kurumMesajlasmaModuluIzinIslemleriService = _testServer.Services.GetService<IKurumMesajlasmaModuluIzinIslemleriService>();
            var excelReadWriteHandler = _testServer.Services.GetService<IExcelReadWriteHandler>(); var kurumLisansService = _testServer.Services.GetService<IKurumLisansService>();

            _panelController = new PanelController(localizationService, bazCookieService, kisiService,
                medyaKutuphanesiService, menuService, paramOrganizasyonBirimleriService,
                kurumOrganizasyonBirimTanimlariService, genelParametrelerService, kurumService, dilService,
                loginRegisterService, kurumIliskiService, kisiIliskiService, searchService, notificationService, kurumLisansService, serviceProvider, kurumMesajlasmaModuluIzinIslemleriService, excelReadWriteHandler);
        }

        [TestMethod()]
        public void PanelKurumController()
        {
            var kurumModel = new KurumTemelKayitModel
            {
                KurumAdi = "Kurum Web App. Unit Test" + Guid.NewGuid().ToString().Substring(0, 10),
                KurulusTarihi = DateTime.Now,
                TicaretSicilNo = "",
                VergiDairesiAdi = "",
                KurumLogo = 1,
                EpostaAdresi = "bilalas@bilal.com",
                WebSitesi = "",
                VergiNo = "",
                FaxNo = "",
                KurumResimUrl = "",
                KurumId = 82,
                KurumVergiDairesiId = "12345df" + Guid.NewGuid().ToString().Substring(0, 10),
                AdresListesi = new EditableList<KurumAdresModel>
                {
                    new KurumAdresModel
                    {
                        Adres = "",
                        LokasyonAdi = "",
                        LokasyonTipi = 1,
                        Sehir = 1,
                        SehirAdi = "",
                        Ulke = 1,
                        UlkeAdi = ""
                    }
                },
                BankaListesi = new List<KurumBankaModel>
                {
                    new KurumBankaModel
                    {
                        BankaAdi = "Kurum Servis test bank",
                        BankaId = 1,
                        SubeAdi = "test sube",
                        HesapNo = "123456789",
                        Iban = "TR0000000000000000000000",
                        IlgiliKurumId = 0,
                        SubeId = 2
                    }
                },
                KurumTips = new List<KurumTipi>
                {
                    new KurumTipi
                    {
                        TabloID = 8,
                        Tanim = "Müşteri"
                    }
                },
                MusteriTemsilciIds = new List<MusteriTemsilcisi>
                {
                    new MusteriTemsilcisi
                    {
                        AdSoyad = "",
                        TabloID = 129
                    }
                }
            };

            // Assert-1 KurumList
            var kurumList = _panelController.KurumList();
            Assert.IsTrue(kurumList.IsSuccess);
            Assert.IsNotNull(kurumList.Value);

            // Assert-2 KurumListForKurum
            var kurumListForKurum = _panelController.KurumListForKurum();
            Assert.IsTrue(kurumListForKurum.IsSuccess);
            Assert.IsNotNull(kurumListForKurum.Value);

            // Assert-3 ListKendisiIle
            var listKendisiIle = _panelController.ListKendisiIle();
            Assert.IsTrue(listKendisiIle.IsSuccess);
            Assert.IsNotNull(listKendisiIle.Value);

            // Assert-4 KurumIliskiTuruGetir
            var kurumIliskiTuruGetir = _panelController.KurumIliskiTuruGetir();
            Assert.IsTrue(kurumIliskiTuruGetir.IsSuccess);
            Assert.IsNotNull(kurumIliskiTuruGetir.Value);

            // Assert-5 KurumMusteriTemsilcisiGetir
            var kurumMusteriTemsilcisiGetir = _panelController.KurumMusteriTemsilcisiGetir();
            Assert.IsTrue(kurumMusteriTemsilcisiGetir.IsSuccess);
            Assert.IsNotNull(kurumMusteriTemsilcisiGetir.Value);

            // Assert-6 KurumTemelVerileriKaydet
            var kurumTemelVerileriKaydet = _panelController.KurumTemelVerileriKaydet(kurumModel);
            Assert.IsTrue(kurumTemelVerileriKaydet.IsSuccess);
            Assert.IsNotNull(kurumTemelVerileriKaydet.Value);

            // Assert-7 KurumTemelVerileriGetir
            var kurumTemelVerileriGetir = _panelController.KurumTemelVerileriGetir(kurumTemelVerileriKaydet.Value.TabloID);
            Assert.IsTrue(kurumTemelVerileriGetir.IsSuccess);
            Assert.IsNotNull(kurumTemelVerileriGetir.Value);

            // Assert-8 KurumIdariVerileriGetir
            var kurumIdariVerileriGetir = _panelController.KurumIdariVerileriGetir(kurumTemelVerileriKaydet.Value.TabloID);
            Assert.IsTrue(kurumIdariVerileriGetir.IsSuccess);
            Assert.IsNotNull(kurumIdariVerileriGetir.Value);

            // Assert-9 CookieKurumID
            var cookieKurumId = _panelController.CookieKurumID();
            Assert.IsTrue(cookieKurumId.IsSuccess);
            Assert.IsNotNull(cookieKurumId.Value);

            // Assert-9 EkParametreListesi
            var ekParametreListesi = _panelController.EkParametreListesi();
            Assert.IsTrue(ekParametreListesi.IsSuccess);
            Assert.IsNotNull(ekParametreListesi.Value);

            // Assert-10 EkParametreListesi
            var ekParametreListesi1 = _panelController.EkParametreListesi(kurumTemelVerileriKaydet.Value.TabloID);
            Assert.IsTrue(ekParametreListesi1.IsSuccess);
            Assert.IsNotNull(ekParametreListesi1.Value);

            kurumModel.KurumAdi = "Kurum Web App. Unit Test1" + Guid.NewGuid().ToString().Substring(0, 10);
            kurumModel.TabloID = kurumTemelVerileriKaydet.Value.TabloID;

            // Assert-11 KurumTemelVerileriGuncelle
            var kurumTemelVerileriGuncelle = _panelController.KurumTemelVerileriGuncelle(kurumModel);
            Assert.IsTrue(kurumTemelVerileriGuncelle.IsSuccess);
            Assert.IsNotNull(kurumTemelVerileriGuncelle.Value);

            var kurumekparamkaydet2 = _globalHelper.Post<Result<KurumEkParametreler>>(LocalPortlar.KurumService + "/api/KurumEkParametre/KurumEkParametreKaydet", new KurumEkParametreViewModel
            {
                ParametreAdi = "Web App Unit Test Gerçekleşen" + Guid.NewGuid().ToString().Substring(0, 10),
                ParametreGosterimTipi = 2,
                ParametreTipi = 1
            });

            var kurumEkModel = new KurumEkParametreKayitViewModel
            {
                IlgiliKurumID = 82,
                Degerler = new List<KurumEkParametreKayitParametreViewModel>
                {
                    new KurumEkParametreKayitParametreViewModel
                    {
                        Deger = "Web App Test Gerçekleşen Değer",
                        ParametreID = kurumekparamkaydet2.Result.Value.TabloID
                    }
                }
            };

            // Assert-12 EkParametreKayit
            var ekParametreKayit = _panelController.EkParametreKayit(kurumEkModel);
            Assert.IsTrue(ekParametreKayit.IsSuccess);
            Assert.IsTrue(ekParametreKayit.Value);

            _globalHelper.Get<Result<bool>>(LocalPortlar.KurumService + "/api/KurumEkParametre/KurumEkParamHardDelete/" + kurumekparamkaydet2.Result.Value.TabloID);

            var kurumIliskiModel = new KurumIliskiKayitModel
            {
                BuKurumID = 85,
                IliskiTuruID = 5,
                BununKurumID = kurumTemelVerileriKaydet.Value.TabloID
            };

            // Assert-13 KurumIliskiKaydet
            var kurumIliskiKaydet = _panelController.KurumIliskiKaydet(kurumIliskiModel);
            Assert.IsTrue(kurumIliskiKaydet.IsSuccess);
            Assert.IsNotNull(kurumIliskiKaydet.Value);

            // Assert-14 KurumIliskiList
            var kurumIliskiList = _panelController.KurumIliskiList();
            Assert.IsTrue(kurumIliskiList.IsSuccess);
            Assert.IsNotNull(kurumIliskiList.Value);

            kurumIliskiModel.IliskiTuruID = 4;
            kurumIliskiModel.BuKurumID = 1;
            kurumIliskiModel.BununKurumID = 82;
            kurumIliskiModel.TabloID = kurumIliskiKaydet.Value.TabloID;
            kurumIliskiModel.GuncelleyenKisiID = 130;

            // Assert-15 KurumIliskiGuncelle
            var kurumIliskiGuncelle = _panelController.KurumIliskiGuncelle(kurumIliskiModel);
            Assert.IsTrue(kurumIliskiGuncelle.IsSuccess);
            Assert.IsNotNull(kurumIliskiGuncelle.Value);

            // Assert-16 KurumIliskiSil
            var kurumIliskiSil = _panelController.KurumIliskiSil(kurumIliskiKaydet.Value.TabloID);
            Assert.IsTrue(kurumIliskiSil.IsSuccess);
            Assert.IsTrue(kurumIliskiSil.Value);

            // Assert-17 AmirTemsilciyeGoreKurumListesi
            var amirTemsilciyeGoreKurumListesi = _panelController.AmirTemsilciyeGoreKurumListesi();
            Assert.IsTrue(amirTemsilciyeGoreKurumListesi.IsSuccess);
            Assert.IsNotNull(amirTemsilciyeGoreKurumListesi.Value);

            var kurumOrganizasyon = new KurumOrganizasyonBirimView
            {
                UstId = 0,
                Tanim = "Web App Test Muhasebe1" + Guid.NewGuid().ToString().Substring(0, 10),
                TipId = 1
            };

            var kurumOrganizasyonJson = new KurumOrganizasyonBirimView
            {
                UstId = 0,
                Tanim = "Web App Test json" + Guid.NewGuid().ToString().Substring(0, 10),
                TipId = 1
            };

            var kurumOrgPoz = new KurumOrganizasyonBirimView
            {
                KurumId = 82,
                IlgiliKurumID = 82,
                UstId = 0,
                Tanim = "Web App Test pozisyonu" + Guid.NewGuid().ToString().Substring(0, 10),
                TipId = 2
            };

            // Assert-18 AddKurumOrganizasyonBirimi
            var addKurumOrganizasyonBirimi = _panelController.AddKurumOrganizasyonBirimi(kurumOrganizasyon);
            Assert.IsTrue(addKurumOrganizasyonBirimi.IsSuccess);
            Assert.IsNotNull(addKurumOrganizasyonBirimi.Value);

            // Assert-19 AddKurumOrganizasyonBirimi2
            var addKurumOrganizasyonBirimi2 = _panelController.AddKurumOrganizasyonBirimi2(kurumOrgPoz);
            Assert.IsTrue(addKurumOrganizasyonBirimi2.IsSuccess);
            Assert.IsNotNull(addKurumOrganizasyonBirimi2.Value);

            // Assert-20 AddKurumOrganizasyonBirimiJson
            var addKurumOrganizasyonBirimiJson = _panelController.AddKurumOrganizasyonBirimiJson(kurumOrganizasyonJson);
            Assert.IsTrue(addKurumOrganizasyonBirimiJson.IsSuccess);
            Assert.IsNotNull(addKurumOrganizasyonBirimiJson.Value);

            kurumOrganizasyon.Tanim = "Web App Test Muhasebe1" + Guid.NewGuid().ToString().Substring(0, 10);
            kurumOrganizasyon.TabloId = addKurumOrganizasyonBirimi.Value;

            // Assert-21 UpdateForKurum
            var updateForKurum = _panelController.UpdateForKurum(kurumOrganizasyon);
            Assert.IsTrue(updateForKurum.IsSuccess);
            Assert.IsNotNull(updateForKurum.Value);

            kurumOrgPoz.Tanim = "Web App Test pozisyonu1" + Guid.NewGuid().ToString().Substring(0, 10);
            kurumOrgPoz.TabloId = addKurumOrganizasyonBirimi2.Value;
            kurumOrgPoz.TipId = 2;

            // Assert-22 UpdateForKurum2
            var updateForKurum2 = _panelController.UpdateForKurum2(kurumOrgPoz);
            Assert.IsTrue(updateForKurum2.IsSuccess);
            Assert.IsNotNull(updateForKurum2.Value);

            kurumOrganizasyon.Tanim = "Web App Test Muhasebe1" + Guid.NewGuid().ToString().Substring(0, 10);
            kurumOrganizasyon.TabloId = addKurumOrganizasyonBirimi.Value;

            // Assert-23 UpdateName
            var updateName = _panelController.UpdateName(kurumOrganizasyon);
            Assert.IsTrue(updateName.IsSuccess);
            Assert.IsNotNull(updateName.Value);

            // Assert-24 GetKurumBirim
            //var kurumBirim = _panelController.GetKurumBirim(3);
            //Assert.IsTrue(kurumBirim.IsSuccess);
            //Assert.IsNotNull(kurumBirim.Value);

            // Assert-25 GetKurumBirim2
            var kurumBirim2 = _panelController.GetKurumBirim2(4);
            Assert.IsTrue(kurumBirim2.IsSuccess);
            Assert.IsNotNull(kurumBirim2.Value);

            // Assert-26 GetKurumBirimList
            var kurumBirimList = _panelController.GetKurumBirimList(3);
            Assert.IsTrue(kurumBirimList.IsSuccess);
            Assert.IsNotNull(kurumBirimList.Value);

            // Assert-27 GetKurumBirimList2
            var kurumBirimList2 = _panelController.GetKurumBirimList2(2);
            Assert.IsTrue(kurumBirimList2.IsSuccess);
            Assert.IsNotNull(kurumBirimList2.Value);

            // Assert-28 GetKurumBirimListForLevel
            var kurumBirimListForLevel = _panelController.GetKurumBirimListForLevel(addKurumOrganizasyonBirimi.Value);
            Assert.IsTrue(kurumBirimListForLevel.IsSuccess);
            Assert.IsNotNull(kurumBirimListForLevel.Value);

            // Assert-29 GetKurumBirimList2
            var kurumBirimListForLevel2 = _panelController.GetKurumBirimListForLevel2(addKurumOrganizasyonBirimi2.Value, 82);
            Assert.IsTrue(kurumBirimListForLevel2.IsSuccess);
            Assert.IsNotNull(kurumBirimListForLevel2.Value);

            // Assert-30 DeleteKurumBirim
            var deleteKurumBirim = _panelController.DeleteKurumBirim(addKurumOrganizasyonBirimi.Value);
            Assert.IsTrue(deleteKurumBirim.IsSuccess);
            Assert.IsTrue(deleteKurumBirim.Value);

            // Assert-31 DeleteKurumBirim2
            var deleteKurumBirim2 = _panelController.DeleteKurumBirim(addKurumOrganizasyonBirimi2.Value);
            Assert.IsTrue(deleteKurumBirim2.IsSuccess);
            Assert.IsTrue(deleteKurumBirim2.Value);

            // Assert-32 DeleteKurumBirim3
            var deleteKurumBirim3 = _panelController.DeleteKurumBirim(addKurumOrganizasyonBirimiJson.Value);
            Assert.IsTrue(deleteKurumBirim3.IsSuccess);
            Assert.IsTrue(deleteKurumBirim3.Value);

            // Assert-33 TemelKurumSilindiYap
            var kurumSilindiYap = _panelController.TemelKurumSilindiYap(kurumTemelVerileriKaydet.Value.TabloID);
            Assert.IsTrue(kurumSilindiYap.IsSuccess);
            Assert.IsTrue(kurumSilindiYap.Value);

            var mesajlasmaModel = new KurumMesajlasmaModuluIzinIslemleriViewModel
            {
                IzinVerilmeyenBirimIDleri = new List<int> { 431, 1969 }
            };

            // Assert-34 IzinVerilmeyenPozisyonKaydi
            var izinVerilmeyenPozisyonKaydi = _panelController.IzinVerilmeyenPozisyonKaydi(mesajlasmaModel);
            Assert.IsTrue(izinVerilmeyenPozisyonKaydi.IsSuccess);
            Assert.IsTrue(izinVerilmeyenPozisyonKaydi.Value);

            // Assert-35 ListForView
            var listForView = _panelController.ListForView();
            Assert.IsTrue(listForView.IsSuccess);
            Assert.IsNotNull(listForView.Value);

            foreach (var ls in listForView.Value)
            {
                // Assert-36 DeleteRecord
                var deleteRecord = _panelController.DeleteRecord(ls.TabloID);
                Assert.IsTrue(deleteRecord.IsSuccess);
                Assert.IsTrue(deleteRecord.Value);
            }
        }
    }
}