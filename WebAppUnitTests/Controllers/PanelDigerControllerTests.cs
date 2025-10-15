using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WebAppUnitTests.Helper;

namespace WebAppUnitTests
{
    [TestClass()]
    public class PanelDigerControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly PanelController _panelController;

        public PanelDigerControllerTests()
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
        }

        [TestMethod()]
        public void PanelDigerController()
        {
            // Assert-2 YetkiIcinSayfaGetir
            var yetkiIcinSayfaGetir = _panelController.YetkiIcinSayfaGetir();
            Assert.IsTrue(yetkiIcinSayfaGetir.IsSuccess);
            Assert.IsNotNull(yetkiIcinSayfaGetir.Value);

            // Assert-3 GetNotification
            var getNotification = _panelController.GetNotification();
            Assert.IsTrue(getNotification.IsSuccess);
            Assert.IsNotNull(getNotification.Value);

            // Assert-4 SetSeen
            var setSeen = _panelController.SetSeen();
            Assert.IsTrue(setSeen.IsSuccess);
            Assert.IsNotNull(setSeen.Value);

            // Assert-5 HataUretme
            //var hataUretme = _panelController.HataUretme()

            var bildirimModel = new BildirimAksiyonLogModel
            {
                BildirimIcerik = "web app test",
                BildirimId = 1
            };

            // Assert-6 LogNotificationAction
            var logNotificationAction = _panelController.LogNotificationAction(bildirimModel);
            Assert.IsTrue(logNotificationAction.IsSuccess);
            Assert.IsNotNull(logNotificationAction.Value);

            // Assert-7 SonGiris
            var sonGiris = _panelController.SonGiris();
            Assert.IsTrue(sonGiris.IsSuccess);
            Assert.IsNotNull(sonGiris.Value);

            var sifreModel = new SifreAtamaModel
            {
                KisiId = 129,
                KisiSifre = "12345aA!"
            };

            // Assert-8 SifreAtama
            var sifreAtama = _panelController.SifreAtama(sifreModel);
            Assert.IsTrue(sifreAtama.IsSuccess);
            Assert.IsNotNull(sifreAtama.Value);

            sifreModel.KisiSifre = "12345Aa!";
            // Assert-8.1 SifreAtama1
            var sifreAtama1 = _panelController.SifreAtama(sifreModel);
            Assert.IsTrue(sifreAtama1.IsSuccess);
            Assert.IsNotNull(sifreAtama1.Value);

            // Assert-9 MesajlarIcindeArama
            var mesajlarIcindeArama = _panelController.MesajlarIcindeArama("test");
            Assert.IsTrue(mesajlarIcindeArama.IsSuccess);
            Assert.IsNotNull(mesajlarIcindeArama.Value);

            //Assert sifreyenilememail
            var sifreyenilememail = _globalHelper.Get<Result<SistemLoginSifreYenilemeAktivasyonHareketleri>>(LocalPortlar.KisiServis + "/api/KisiService/SifreYenileMailile/" + "b@mail.com");

            string[] value2 = sifreyenilememail.Result.Value.SifreYenilemeSayfasiGeciciUrl.Split('=');

            var sifre1Model = new SifreModel()
            {
                Email = "b@mail.com",
                GecerlilikZamani = DateTime.Now,
                OldPassword = "12345Aa!",
                NewPassword = "123456aA!",
                NewPasswordTekrar = "123456aA!",
                kontrolGUID = value2[1]
            };

            // Assert-10 UpdatePassword
            var updatePassword = _panelController.UpdatePassword(sifre1Model);
            Assert.IsNotNull(updatePassword);

            //Assert sifreyenilememail
            var sifreyenilememail1 = _globalHelper.Get<Result<SistemLoginSifreYenilemeAktivasyonHareketleri>>(LocalPortlar.KisiServis + "/api/KisiService/SifreYenileMailile/" + "b@mail.com");

            string[] value3 = sifreyenilememail1.Result.Value.SifreYenilemeSayfasiGeciciUrl.Split('=');
            var sifre2Model = new SifreModel()
            {
                Email = "b@mail.com",
                GecerlilikZamani = DateTime.Now,
                OldPassword = "123456aA!",
                NewPassword = "12345Aa!",
                NewPasswordTekrar = "12345Aa!",
                kontrolGUID = value3[1]
            };

            // Assert-10.1 UpdatePassword
            var updatePassword1 = _panelController.UpdatePassword(sifre2Model);
            Assert.IsNotNull(updatePassword1);

            // Assert-11 SetCulture
            var setCulture = _panelController.SetCulture("TR-tr");
            Assert.IsTrue(setCulture);

            var stream = new MemoryStream();
            var stream1 = new MemoryStream();
            var formFile = new FormFile(stream, 0, stream.Length, "test", "test.txt");
            var formFile2 = new FormFile(stream1, 0, stream1.Length, "test1", "test1.txt");
            var listFile = new List<IFormFile> { formFile, formFile2 };

            // Assert-12 ResimYukle
            _panelController.ResimYukle(formFile);

            // Assert-13 ResimYukle
            _panelController.UploadMultiple(listFile);

            var chunkModel = new ChunkModel
            {
                file = formFile,
                index = 1,
                isRecording = true,
                kisiId = 129,
                kurumId = 82,
                odaId = 1,
                randGuid = Guid.NewGuid().ToString().Substring(0, 10),
                room = "abc"
            };

            // Assert-14 UploadChunk
            _panelController.UploadChunk(chunkModel);

            // Assert-15 GetOnlineUserCount
            _panelController.GetOnlineUserCount();

            // Assert-1 UlkeMaskList
            //var UlkeMaskList = _panelController.UlkeMaskGetir();
            //Assert.IsNotNull(UlkeMaskList.Value);
            //Assert.IsTrue(UlkeMaskList.IsSuccess);
        }
    }
}