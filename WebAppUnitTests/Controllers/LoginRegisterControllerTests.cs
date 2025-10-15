using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Baz.Model.Entity;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager;
using Baz.RequestManager.Abstracts;
using Baz.SharedSession;
using BazWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebAppUnitTests.Helper;
using Microsoft.AspNetCore.TestHost;

namespace WebAppUnitTests
{
    [TestClass()]
    public class LoginRegisterControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly IRequestHelper _helperNoHeader;
        private readonly RequestHelper _globalHelper;
        private readonly LoginRegisterController _loginRegisterController;
        private readonly TestServer _testServer;

        public LoginRegisterControllerTests()
        {
            _helperNoHeader = TestServerRequestHelperNoHeader.CreateHelper();
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper { _headers = _helper._headers };
            var logger = _testServer.Services.GetService<ILogger<LoginRegisterController>>();
            var bazCookieService = _testServer.Services.GetService<IBazCookieService>();
            _testServer.Services.GetService<IRequestHelper>();
            var kurumService = _testServer.Services.GetService<IKurumService>();
            var loginRegisterService = _testServer.Services.GetService<ILoginRegisterService>();
            var sharedSession = _testServer.Services.GetService<ISharedSession>();
            var kisiService = _testServer.Services.GetService<IKisiService>();

            _loginRegisterController = new LoginRegisterController(logger, bazCookieService,
                kurumService, loginRegisterService, sharedSession, kisiService);
        }

        [TestMethod()]
        public void ViewLoginRegisterController()
        {
            //Act - 1 confidentialityagreement
            var actual = _loginRegisterController.Confidentialityagreement() as ViewResult;
            _helper.Get<ViewResult>("/LoginRegister/confidentialityagreement");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual.ViewName) || actual.ViewName == "confidentialityagreement");

            //Act - 2 membershipconditions
            var actual1 = _loginRegisterController.Membershipconditions() as ViewResult;
            _helper.Get<ViewResult>("/LoginRegister/membershipconditions");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual1.ViewName) || actual1.ViewName == "membershipconditions");

            //Act - 3 Register
            var actual2 = _loginRegisterController.Register() as ViewResult;
            _helper.Get<ViewResult>("/LoginRegister/Register");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual2.ViewName) || actual2.ViewName == "Register");

            //Act - 4 SifreYenile
            var actual3 = _loginRegisterController.SifreYenile() as ViewResult;
            _helper.Get<ViewResult>("/LoginRegister/ResetPassword");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual3.ViewName) || actual3.ViewName == "SifreYenile");

            //Act - 5 RecoverPassword
            var actual4 = _loginRegisterController.RecoverPassword() as ViewResult;
            _helper.Get<ViewResult>("/LoginRegister/RecoverPassword");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual4.ViewName) || actual4.ViewName == "RecoverPassword");

            //Act - 6 ForgotPassword
            var actual5 = _loginRegisterController.ForgotPassword() as ViewResult;
            _helper.Get<ViewResult>("/LoginRegister/ForgotPassword");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual5.ViewName) || actual5.ViewName == "ForgotPassword");

            //Act - 7 ForgotPassword
            var actual6 = _loginRegisterController.ForgotPassword() as ViewResult;
            _helper.Get<ViewResult>("/LoginRegister/Activation");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual5.ViewName) || actual6.ViewName == "ForgotPassword");
        }

        [TestMethod()]
        public void ApiLoginRegisterController()
        {
            var login1 = new LoginModel
            {
                EmailOrUserName = "btest@b.com",
                Password = "12345Aa!!!",
                IpAdress = "",
                UserAgent = "",
                Facebook = true,
                Apple = true,
                Google = true
            };

            var loginFail = _loginRegisterController.Login(login1);
            Assert.IsNull(loginFail.Value);

            // Assert-1 Başarılı login işlemi
            var login = new LoginModel
            {
                EmailOrUserName = "btest@b.com",
                Password = "123456aA!",
                IpAdress = "",
                UserAgent = "",
                Facebook = false,
                Apple = false,
                Google = false
            };

            var loginSuccess = _loginRegisterController.Login(login);
            Assert.AreEqual(loginSuccess.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(loginSuccess.Value);

            var loginFail1 = _loginRegisterController.Login(login1);
            Assert.IsNull(loginFail1.Value);

            var loginFail2 = _loginRegisterController.Login(login1);
            Assert.IsNull(loginFail2.Value);

            var loginFail3 = _loginRegisterController.Login(login1);
            Assert.IsNull(loginFail3.Value);

            //Assert TemelKisiAktifYap
            var temelKisiAktifYap = _helper.Get<Result<bool>>("/panel/TemelKisiAktifYap/" + 3959);
            Assert.IsTrue(temelKisiAktifYap.IsSuccess);
            Assert.IsTrue(temelKisiAktifYap.Result.Value);

            //// Assert-3 Kullanıcı adı ve parola hatalı login işlemi
            //var login2 = new LoginModel
            //{
            //    EmailOrUserName = "test@test.com",
            //    Password = "digi321Aa!!!",
            //    IpAdress = "",
            //    UserAgent = ""
            //};
            //var loginFail2 = _loginRegisterController.Login(login2);
            //Assert.IsNull(loginFail2.Value);

            var posta = "Test" + Guid.NewGuid().ToString().Substring(0, 5) + "@mail.com";
            var posta1 = "TestKurum" + Guid.NewGuid().ToString().Substring(0, 5) + "@mail.com";
            var basicKisiModel = new BasicKisiModel()
            {
                KisiSifre = "12345Aa!",
                UyelikSartiKabulEttiMi = true,
                KisiAdi = posta,
                KisiSoyadi = "TestLastName",
                MeslekiUnvan = "TestUnvan",
                KisiEposta = posta,
                KisiKullaniciAdi = posta,
                KisiTelefon = "05555555555",
                KisiMail = posta,
                KurumsalMi = false,
            };
            var basicKurumModel = new BasicKurumModel()
            {
                KurumKisaUnvan = "TestCompany" + Guid.NewGuid().ToString().Substring(0, 10),
                KurumTicariUnvani = "TestUnvan" + Guid.NewGuid().ToString().Substring(0, 10),
                KurumTipiId = 1,
                UlkeID = 1,
                KurumVergiNo = "123456TestKurumVergi" + Guid.NewGuid().ToString().Substring(0, 10),
                ////LisansKisiSayisi = 100,
                //LisansId = 5,
                //LisansZamanId = 1
            };

            var basicKisiModel1 = new BasicKisiModel()
            {
                KisiSifre = "12345Aa!",
                UyelikSartiKabulEttiMi = true,
                KisiAdi = posta1,
                KisiSoyadi = "TestLastName",
                MeslekiUnvan = "TestUnvan",
                KisiEposta = posta1,
                KisiKullaniciAdi = posta1,
                KisiTelefon = "05555555555",
                KisiMail = posta1,
                KurumsalMi = true,
            };
            var basicKurumModel1 = new BasicKurumModel()
            {
                KurumKisaUnvan = "TestCompany1" + Guid.NewGuid().ToString().Substring(0, 10),
                KurumTicariUnvani = "TestUnvan1" + Guid.NewGuid().ToString().Substring(0, 10),
                KurumTipiId = 1,
                UlkeID = 1,
                KurumVergiNo = "123456TestKurumVergi1" + Guid.NewGuid().ToString().Substring(0, 10),
                //LisansKisiSayisi = 100,
               // LisansId = 5,
                //LisansZamanId = 1
            };
            var login2 = new LoginModel
            {
                EmailOrUserName = basicKisiModel1.KisiMail,
                Password = basicKisiModel1.KisiSifre,
                IpAdress = "",
                UserAgent = ""
            };
            var postModel = new KurumKisiPostModel { KisiModel = basicKisiModel, KurumModel = basicKurumModel };

            var postModel1 = new KurumKisiPostModel { KisiModel = basicKisiModel1, KurumModel = basicKurumModel1 };

            // Register
            var Register = _helperNoHeader.Post<Result<SistemLoginSifreYenilemeAktivasyonHareketleri>>("/LoginRegister/Register", postModel1);
            //var Register =_loginRegisterController.Register(postModel1);
            Assert.IsNotNull(Register.Result.Value);
            Assert.AreEqual(Register.Result.StatusCode, (int)ResultStatusCode.Success);

            postModel.KurumModel.KurumVergiNo = "KurumVergiBilmemkaç123" + Guid.NewGuid().ToString().Substring(0, 10);
            postModel.KisiModel.KisiMail = "Test@" + Guid.NewGuid().ToString().Substring(0, 10) + ".com";
            // RegisterBireysel
            var RegisterBireysel = _helperNoHeader.Post<Result<SistemLoginSifreYenilemeAktivasyonHareketleri>>("/LoginRegister/BireyselRegister", postModel);
            //var RegisterBireysel = _loginRegisterController.BireyselRegister(postModel);
            Assert.IsNotNull(RegisterBireysel.Result.Value);
            Assert.AreEqual(RegisterBireysel.Result.StatusCode, (int)ResultStatusCode.Success);

            //  kaydet Olumsuz Aynı vergi No
            var kurumKisiKaydetOlumsuzAynıVergi = _helperNoHeader.Post<Result<SistemLoginSifreYenilemeAktivasyonHareketleri>>("/LoginRegister/Register", postModel);
            //var kurumKisiKaydetOlumsuzAynıVergi = _loginRegisterController.Register(postModel);
            Assert.IsNull(kurumKisiKaydetOlumsuzAynıVergi.Result.Value);

            //  kaydet Olumsuz Aynı vergi No
            var kurumKisiKaydetOlumsuzAynıVergi1 = _helperNoHeader.Post<Result<SistemLoginSifreYenilemeAktivasyonHareketleri>>("/LoginRegister/BireyselRegister", postModel);
            //var kurumKisiKaydetOlumsuzAynıVergi1 = _loginRegisterController.BireyselRegister(postModel);
            Assert.IsNull(kurumKisiKaydetOlumsuzAynıVergi1.Result.Value);

            var url = Register.Result.Value.HesapAktivasyonSayfasiGeciciUrl;
            // HesapAktivasyonLinkiGecerliMi
            var HesapAktivasyonLinkiGecerliMi = _loginRegisterController.HesapAktivasyonLinkiGecerliMi(url[^36..]);
            Assert.AreEqual(HesapAktivasyonLinkiGecerliMi.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(HesapAktivasyonLinkiGecerliMi.Value);

            // Hesap Aktivasyonu
            var hesapAktivasyon = _helperNoHeader.Post<Result<KisiTemelBilgiler>>("/LoginRegister/HesapAktiflestirme", url[^36..]);
            //var hesapAktivasyon = _loginRegisterController.HesapAktiflestirme(url[^36..]);
            Assert.AreEqual(hesapAktivasyon.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(hesapAktivasyon.Result.Value);

            // Hesap Aktivasyonu2
            var url2 = RegisterBireysel.Result.Value.HesapAktivasyonSayfasiGeciciUrl;
            var hesapAktivasyon2 = _helperNoHeader.Post<Result<KisiTemelBilgiler>>("/LoginRegister/HesapAktiflestirme", url2[^36..]);
            //var hesapAktivasyon2 = _loginRegisterController.HesapAktiflestirme(url2[^36..]);
            Assert.AreEqual(hesapAktivasyon2.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(hesapAktivasyon2.Result.Value);

            //KurumList
            var KurumList = _loginRegisterController.KurumList("Orsa");
            Assert.AreEqual(KurumList.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(KurumList.Value);

            // Kullanıcı adı kontrolü
            var kullaniciAdikontrolü = _loginRegisterController.KullaniciAdiKontrolu(postModel.KisiModel.KisiEposta);
            Assert.AreEqual(kullaniciAdikontrolü.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(kullaniciAdikontrolü.Value);

            //SifreYenilemeIstegi
            var SifreYenilemeIstegi = _loginRegisterController.SifreYenilemeIstegi(basicKisiModel.KisiMail);
            Assert.AreEqual(SifreYenilemeIstegi.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(SifreYenilemeIstegi.Value);

            //SifreYenilemeGecerliMi
            var SifreYenilemeGecerliMi = _loginRegisterController.SifreYenilemeGecerliMi(SifreYenilemeIstegi.Value.SifreYenilemeSayfasiGeciciUrl);
            Assert.AreEqual(SifreYenilemeGecerliMi.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(SifreYenilemeGecerliMi.Value);

            var kurumList = _globalHelper.Get<Result<List<KurumTemelBilgiler>>>(LocalPortlar.KurumService +
                                                                    "/api/KurumService/List");

            var kurum1 = kurumList.Result.Value.LastOrDefault();
            var kurum2 = kurumList.Result.Value.SingleOrDefault(x => x.TabloID == (kurum1.TabloID - 1));
            var kisiList1 =
                _globalHelper.Get<Result<List<KisiTemelBilgiler>>>(LocalPortlar.KisiServis + "/api/KisiService/KurumaBagliKisilerList/" + kurum1.TabloID);
            var kisiList2 =
                _globalHelper.Get<Result<List<KisiTemelBilgiler>>>(LocalPortlar.KisiServis +
                                                                   "/api/KisiService/KurumaBagliKisilerList/" + kurum2.TabloID);

            var kisi1 = kisiList1.Result.Value.LastOrDefault();
            var kisi2 = kisiList2.Result.Value.LastOrDefault();
            string[] value2 = SifreYenilemeIstegi.Value.SifreYenilemeSayfasiGeciciUrl.Split('=');
            //SifreYenile
            var model1 = new SifreModel()
            {
                Email = "tester@mail.com",
                KisiID = 342,
                GecerlilikZamani = DateTime.Now,
                OldPassword = "123456aA!",
                NewPassword = "123456Aa!",
                NewPasswordTekrar = "123456Aa!",
                kontrolGUID = value2[1]
            };
            //SifreYenile
            var sifreYenile = _helperNoHeader.Post<Result<SifreModel>>("/LoginRegister/SifreYenile", model1);
            //var sifreYenile = _loginRegisterController.SifreYenile(model1);
            Assert.AreEqual(sifreYenile.Result.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(sifreYenile.Result.Value);

            _loginRegisterController.SifreYenile(new SifreModel()
            {
                Email = "tester@mail.com",
                KisiID = 342,
                GecerlilikZamani = DateTime.Now,
                OldPassword = model1.NewPassword,
                NewPassword = model1.OldPassword,
                NewPasswordTekrar = model1.OldPassword,
                kontrolGUID = value2[1]
            });

            // GetUserId
            var GetUserId = _loginRegisterController.GetUserId();
            Assert.AreEqual(GetUserId.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(GetUserId.Value);

            _globalHelper.Get<Result<bool>>(LocalPortlar.KurumService + "/api/KurumService/KurumDelete/" + kurum1.TabloID);

            _globalHelper.Get<Result<bool>>(LocalPortlar.KurumService + "/api/KurumService/KurumDelete/" + kurum2.TabloID);

            _globalHelper.Get<Result<bool>>(LocalPortlar.KisiServis + "/api/KisiService/TemelBilgilerSil/" + kisi1.TabloID);

            _globalHelper.Get<Result<bool>>(LocalPortlar.KisiServis + "/api/KisiService/TemelBilgilerSil/" + kisi2.TabloID);
        }
    }
}