using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Baz.ElasticSearch.Abstract;
using Baz.ElasticSearch.Model;
using Baz.Model.Entity;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager;
using BazWebApp;
using BazWebApp.Handlers;
using BazWebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using WebAppUnitTests.Helper;

namespace WebAppUnitTests
{
    [TestClass()]
    public class PanelKisiControllerTests
    {
        private readonly RequestHelper _helper;

        private readonly RequestHelper _globalHelper;
        private readonly PanelController _panelController;
        private readonly bool isDevelopment = false;

        public PanelKisiControllerTests()
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
            var excelReadWriteHandler = _testServer.Services.GetService<IExcelReadWriteHandler>();
            var kurumLisansService = _testServer.Services.GetService<IKurumLisansService>();

            _panelController = new PanelController(localizationService, bazCookieService, kisiService,
                medyaKutuphanesiService, menuService, paramOrganizasyonBirimleriService,
                kurumOrganizasyonBirimTanimlariService, genelParametrelerService, kurumService, dilService,
                loginRegisterService, kurumIliskiService, kisiIliskiService, searchService, notificationService, kurumLisansService, serviceProvider, kurumMesajlasmaModuluIzinIslemleriService, excelReadWriteHandler);
#if DEBUG
            isDevelopment = true;
#endif
        }

        [TestMethod()]
        public async Task PanelKisiControllerCrudTest()
        {
            //Assert GetUserProfile
            var GetUserProfile = _panelController.GetUserProfile();
            Assert.IsNotNull(GetUserProfile);

            //Assert KisiTemelKaydet
            var kisiTemelKaydet = _panelController.KisiTemelKaydet(new KisiTemelKayitModel()
            {
                Adi = "Test Webb App Ekleme",
                Soyadi = "Testidir",
                AnneAdi = "Hacer",
                BabaAdi = "Mustafa",
                Cinsiyeti = 1,
                Departman = 4,
                Dini = 1,
                DogduguSehir = 1,
                DogduguUlke = 1,
                DogumTarihi = DateTime.Now,
                Kurum = 82,
                Rol = 825,
                Pozisyon = 430,
                TCKimlikNo = "23424212332",
                EpostaAdresi = "canerweb31@mail.com",
                MedeniHali = 1,
                IseGirisTarihi = DateTime.Now,
                Lokasyon = 1,
                SicilNo = "233432",
                //TemelBilgiTabloID = 97,
                DilListesi = new List<DilModel>()
                   {
                        new DilModel()
                        {
                            DilSeviye = "",
                            YabanciDilTipi = 1
                        }
                   },
                AdresListesi = new List<AdresModel>()
                   {
                        new AdresModel()
                        {
                            Adres = "sdffd",
                            AdresTipi = 1,
                            Sehir = 1,
                            Ulke = 1
                        }
                   },
                OkulListesi = new List<OkulModel>()
                   {
                        new OkulModel()
                        {
                            Fakulte = "MUhendislik",
                            MezuniyetTarihi = DateTime.Now,
                            OkulAdi = "Istanbul",
                            OkulTipi = 1
                        }
                   },
                TelefonListesi = new List<TelefonModel>()
                   {
                        new TelefonModel()
                        {
                            TelefonNo = "",
                            TelefonTipi = 1
                        }
                   },
            });
            Assert.IsTrue(kisiTemelKaydet.IsSuccess);
            Assert.IsNotNull(kisiTemelKaydet.Value);

            //Assert KisiTemelKayitVerileriGetir
            var kisiTemelKayitVerileriGetir = _panelController.KisiTemelKayitVerileriGetir(kisiTemelKaydet.Value.TemelBilgiTabloID);
            Assert.IsTrue(kisiTemelKayitVerileriGetir.IsSuccess);
            Assert.IsNotNull(kisiTemelKayitVerileriGetir.Value);

            //Assert KisiTemelKayitVerileriGetirView
            var kisiTemelKayitVerileriGetir1 = _panelController.KisiTemelKayitVerileriGetirView(kisiTemelKaydet.Value.TemelBilgiTabloID);
            Assert.IsTrue(kisiTemelKayitVerileriGetir1.IsSuccess);
            Assert.IsNotNull(kisiTemelKayitVerileriGetir1.Value);

            //Assert KisiTemelKayitVerileriGuncelle
            var kisiTemelKayitVerileriGuncelle = _panelController.KisiTemelKayitVerileriGuncelle(new KisiTemelKayitModel()
            {
                TemelBilgiTabloID = kisiTemelKaydet.Value.TemelBilgiTabloID,
                Adi = "Test Webb App Güncel",
                Soyadi = "Testidir",
                AnneAdi = "Hacer",
                BabaAdi = "Mustafa",
                Cinsiyeti = 1,
                Departman = 4,
                Dini = 1,
                DogduguSehir = 1,
                DogduguUlke = 1,
                DogumTarihi = DateTime.Now,
                Kurum = 82,
                Rol = 825,
                Pozisyon = 430,
                TCKimlikNo = "23424212332",
                EpostaAdresi = "canerweb31@mail.com",
                MedeniHali = 1,
                IseGirisTarihi = DateTime.Now,
                Lokasyon = 1,
                SicilNo = "233432",
                //TemelBilgiTabloID = 97,
                DilListesi = new List<DilModel>()
                   {
                        new DilModel()
                        {
                            DilSeviye = "",
                            YabanciDilTipi = 1
                        }
                   },
                AdresListesi = new List<AdresModel>()
                   {
                        new AdresModel()
                        {
                            Adres = "sdffd",
                            AdresTipi = 1,
                            Sehir = 1,
                            Ulke = 1
                        }
                   },
                OkulListesi = new List<OkulModel>()
                   {
                        new OkulModel()
                        {
                            Fakulte = "MUhendislik",
                            MezuniyetTarihi = DateTime.Now,
                            OkulAdi = "Istanbul",
                            OkulTipi = 1
                        }
                   },
                TelefonListesi = new List<TelefonModel>()
                   {
                        new TelefonModel()
                        {
                            TelefonNo = "",
                            TelefonTipi = 1
                        }
                   },
            });
            Assert.IsTrue(kisiTemelKayitVerileriGuncelle.IsSuccess);
            Assert.IsNotNull(kisiTemelKayitVerileriGuncelle.Value);

            //Assert UpdateProfile
            var UpdateProfile = _panelController.UpdateProfile(new KisiTemelBilgiler()
            {
                TabloID = kisiTemelKaydet.Value.TemelBilgiTabloID,
                KisiAdi = "Webb App Test Kisi Profil Güncele",
                KisiSoyadi = "Testi",
                KisiKullaniciAdi = "test.tester",
                KisiEkranAdi = "mahmut",
                KisiEposta = "bazwebapptestbu@hotmail.com",
                GuncellenmeTarihi = DateTime.Now
            });
            Assert.IsTrue(UpdateProfile);

            //Assert TemelKisiListesiGetir
            var temelKisiListesiGetir = _panelController.TemelKisiListesiGetir();
            Assert.IsTrue(temelKisiListesiGetir.IsSuccess);
            Assert.IsNotNull(temelKisiListesiGetir.Value);

            //Assert PasifKisiListesiGetir
            var pasifKisiListesiGetir = _panelController.PasifKisiListesiGetir();
            Assert.IsTrue(pasifKisiListesiGetir.IsSuccess);
            Assert.IsNotNull(pasifKisiListesiGetir.Value);

            //Assert AmirTemsilciyeGoreKisiListesi
            var amirTemsilciyeGoreKisiListesi = _panelController.AmirTemsilciyeGoreKisiListesi();
            Assert.IsTrue(amirTemsilciyeGoreKisiListesi.IsSuccess);
            Assert.IsNotNull(amirTemsilciyeGoreKisiListesi.Value);

            //Assert KisiIdileKisiListele
            var kisiIdileKisiListele = _panelController.KisiIdileKisiListele(new List<int>(kisiTemelKaydet.Value.TemelBilgiTabloID));
            Assert.IsTrue(kisiIdileKisiListele.IsSuccess);
            Assert.IsNotNull(kisiIdileKisiListele.Value);

            //Assert KisiMusteriTemsilcisiMi
            var kisiMusteriTemsilcisiMi = _panelController.KisiMusteriTemsilcisiMi(kisiTemelKaydet.Value.TemelBilgiTabloID);
            Assert.IsTrue(kisiMusteriTemsilcisiMi.IsSuccess);
            Assert.IsTrue(kisiMusteriTemsilcisiMi.Value);

            //Assert TemelKisiAktifYap
            var temelKisiAktifYap = _panelController.TemelKisiAktifYap(kisiTemelKaydet.Value.TemelBilgiTabloID);
            Assert.IsTrue(temelKisiAktifYap.IsSuccess);
            Assert.IsTrue(temelKisiAktifYap.Value);

            //Assert TemelKisiSilindiYap
            var temelKisiSilindiYap = _panelController.TemelKisiSilindiYap(kisiTemelKaydet.Value.TemelBilgiTabloID);
            Assert.IsTrue(temelKisiSilindiYap.IsSuccess);
            Assert.IsTrue(temelKisiSilindiYap.Value);

            var direc = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "mymediadir").Replace("mymediadir", "");

            var direc1 = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "mymediadir").Replace("mymediadir", "");

            var file = File.OpenRead(direc + "TempExcel/KisiListesiImport.xlsx");
            var file1 = File.OpenRead(direc1 + "TempExcel/KisiListesiImport1.xls");
            var stream = new MemoryStream();
            var stream1 = new MemoryStream();

            file.CopyTo(stream);
            file1.CopyTo(stream1);

            var formFile = new FormFile(stream, 0, stream.Length, "KisiListesiImport", "KisiListesiImport.xlsx")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/excel",
                ContentDisposition = "form-data"
            };
            var formFile1 = new FormFile(stream1, 0, stream1.Length, "KisiListesiImport1", "KisiListesiImport1.xls")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/excel",
                ContentDisposition = "form-data"
            };

            //Assert ExcelIleKisiKayit
            var excelIleKisiKayit = _panelController.ExcelIleKisiKayit(formFile);
            Assert.IsTrue(excelIleKisiKayit.IsSuccess);
            Assert.IsTrue(excelIleKisiKayit.Value);

            //Assert ExcelIleKisiKayit1
            var excelIleKisiKayit1 = _panelController.ExcelIleKisiKayit(formFile1);

            Assert.IsTrue(excelIleKisiKayit1.IsSuccess);
            Assert.IsTrue(excelIleKisiKayit1.Value);

            var sonKisi = _globalHelper.Get<Result<KisiTemelBilgiler>>(LocalPortlar.KisiServis + "/api/KisiService/SonKisi");

            _globalHelper.Get<Result<KisiTemelBilgiler>>(LocalPortlar.KisiServis + "/api/KisiService/TemelBilgilerSil/" + sonKisi.Result.Value.TabloID);

            _globalHelper.Get<Result<KisiTemelBilgiler>>(LocalPortlar.KisiServis + "/api/KisiService/TemelBilgilerSil/" + Convert.ToInt32(sonKisi.Result.Value.TabloID - 1));
        }

        //[TestMethod()]
        //public void ExcT()
        //{
        //    var url = "/panel/ExcelKisiKayit";

        //    var direc = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "mymediadir").Replace("mymediadir", "");

        //    //var direc1 = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "mymediadir").Replace("mymediadir", "");

        //    var file = File.OpenRead(direc + "TempExcel/KisiListesiImport.xlsx");
        //    //var file1 = File.OpenRead(direc1 + "TempExcel/KisiListesiImport1.xls");
        //    var stream = new MemoryStream();
        //    file.CopyTo(stream);
        //    //var formData = new MultipartFormDataContent();
        //    //var content = new StreamContent(file);
        //    // formData.Add(content, "KisiListesiImport", "KisiListesiImport.xlsx");

        //     FormFile formFile = new FormFile(stream, 0, stream.Length, "KisiListesiImport", "KisiListesiImport.xlsx")
        //     {
        //         Headers = new HeaderDictionary(),
        //         ContentType = "application/excel",
        //         ContentDisposition = "form-data"
        //     };

        //    //var excelIleKisiKayit =  _helper._client.PostAsJsonAsync(url, formData);

        //    var excelIleKisiKayit = _panelController.ExcelIleKisiKayit(formFile);

        //    Assert.IsTrue(excelIleKisiKayit.IsSuccess);
        //    Assert.IsTrue(excelIleKisiKayit.Value);
        //    // var response = await _helper._client.PostAsJsonAsync(url, formFile);

        //}

        //[TestMethod()]
        //public void ExcT2()
        //{
        //    HttpClientHandler clientHandler = new()
        //    {
        //        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
        //        {
        //            if (isDevelopment)
        //            {
        //                return true; // for development, trust all certificates
        //            }

        //            return sslPolicyErrors == SslPolicyErrors.None;
        //        }
        //    };
        //    HttpClient _client = new(clientHandler);
        //    var baseURl = _helper._client.BaseAddress;

        //    _helper._client = _client;
        //    _helper._client.BaseAddress = baseURl;

        //    var formData = new MultipartFormDataContent();

        //    var direc = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "mymediadir").Replace("mymediadir", "");

        //    var file = File.OpenRead(direc + "TempExcel/KisiListesiImport.xlsx");

        //    var content = new StreamContent(file);
        //    formData.Add(content, "KisiListesiImport", "KisiListesiImport.xlsx");

        //    var response =_helper._client.PostAsync("/panel/ExcelKisiKayit", formData).Result;
        //    Result<bool> medya = JsonConvert.DeserializeObject<Result<bool>>(response.Content.ReadAsStringAsync().Result);

        //}

        [TestMethod()]
        public void PanelKisiControllerEkParametrelerTest()
        {
            // Kisi Ek Parametre Kaydı
            var kisiekparamkaydet = _globalHelper.Post<Result<KisiEkParametreler>>(LocalPortlar.KisiServis + "/api/KisiEkParametre/KisiEkParametreKaydet", new KisiEkParametreViewModel
            {
                ParametreAdi = "Baz Webb Unit Test",
                ParametreGosterimTipi = 5,
                ParametreTipi = 2
            });
            Assert.AreEqual(kisiekparamkaydet.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(kisiekparamkaydet.Result);

            var testModel = new KisiEkParametreKayitViewModel()
            {
                IlgiliKisiID = 129,
                KisiID = 129,
                KurumID = 82,
                Degerler = new List<KisiEkParametreKayitParametreViewModel>
                {
                    new KisiEkParametreKayitParametreViewModel
                    {
                        Deger = "Baz Webb Unit Test Degeri",
                        ParametreID = kisiekparamkaydet.Result.Value.TabloID
                    }
                }
            };

            //Assert Ek Parametre Kaydı
            var ekParametreKayit = _panelController.KisiEkParametreKayit(testModel);
            Assert.IsTrue(ekParametreKayit.IsSuccess);
            Assert.IsTrue(ekParametreKayit.Value);

            //Assert KisiEkParametreListesi ID ile
            var kisiEkParametreListesiIDIle = _panelController.KisiEkParametreListesi(testModel.KisiID);
            Assert.IsTrue(kisiEkParametreListesiIDIle.IsSuccess);
            Assert.IsNotNull(kisiEkParametreListesiIDIle.Value);

            //Assert KisiEkParametreListesi
            var kisiEkParametreListesi = _panelController.KisiEkParametreListesi();
            Assert.IsTrue(kisiEkParametreListesi.IsSuccess);
            Assert.IsNotNull(kisiEkParametreListesi.Value);

            //Assert Delete
            var kisiekparamsil = _globalHelper.Get<Result<KisiEkParametreler>>(LocalPortlar.KisiServis + "/api/KisiEkParametre/KisiEkParamHardDelete/" + kisiekparamkaydet.Result.Value.TabloID);
            Assert.IsTrue(kisiekparamsil.Result.IsSuccess);
            Assert.IsNotNull(kisiekparamsil.Result.Value);
        }

        [TestMethod()]
        public void PanelKisiControllerKisiIliskiTest()
        {
            //Assert KisiIliskiKayit
            var kisiIliskiKayit = _panelController.KisiIliskiKaydet(new KisiIliskiKayitModel()
            {
                BuKisiID = 210,
                BununKisiID = 129,
                IliskiTuruID = 7,
                KurumID = 82
            });
            Assert.IsTrue(kisiIliskiKayit.IsSuccess);
            Assert.IsNotNull(kisiIliskiKayit.Value);

            //Assert KisiIliskiGuncelle
            var kisiIliskiGuncelle = _panelController.KisiIliskiGuncelle(new KisiIliskiKayitModel()
            {
                TabloID = kisiIliskiKayit.Value.TabloID,
                BuKisiID = 130,
                BununKisiID = 401,
                IliskiTuruID = 7,
                KurumID = 82
            });
            Assert.IsTrue(kisiIliskiGuncelle.IsSuccess);
            Assert.IsNotNull(kisiIliskiGuncelle.Value);

            //Assert KisiIliskiTuruGetir
            var kisiIliskiTuruGetir = _panelController.KisiIliskiTuruGetir();
            Assert.IsTrue(kisiIliskiTuruGetir.IsSuccess);
            Assert.IsNotNull(kisiIliskiTuruGetir.Value);

            //Assert KisiIliskiList
            var kisiIliskiList = _panelController.KisiIliskiList();
            Assert.IsTrue(kisiIliskiList.IsSuccess);
            Assert.IsNotNull(kisiIliskiList.Value);

            //Assert KisiIliskiSil
            var kisiIliskiSil = _panelController.KisiIliskiSil(kisiIliskiKayit.Value.TabloID);
            Assert.IsTrue(kisiIliskiSil.IsSuccess);
            Assert.IsNotNull(kisiIliskiSil.Value);
        }
    }
}