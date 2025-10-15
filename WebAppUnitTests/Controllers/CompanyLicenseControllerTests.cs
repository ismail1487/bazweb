using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager;
using BazWebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NWebsec.AspNetCore.Core.Web;
using WebAppUnitTests.Helper;

namespace WebAppUnitTests
{
    [TestClass()]
    public class CompanyLicenseControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly CompanyLicenseController _companyLicenseController;
        private readonly HomeController _homeController;

        public CompanyLicenseControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper();
            _globalHelper._headers = _helper._headers;

            var kurumLisansService = _testServer.Services.GetService<IKurumLisansService>();
            var bazCookieService = _testServer.Services.GetService<IBazCookieService>();
            var serviceProvider = _testServer.Services.GetService<IServiceProvider>();
            _companyLicenseController = new CompanyLicenseController(kurumLisansService, bazCookieService, serviceProvider);

            var logger = _testServer.Services.GetService<ILogger<HomeController>>();
            var localizationService = _testServer.Services.GetService<ILocalizationService>();
            var menuService = _testServer.Services.GetService<IMenuService>();
            _homeController = new HomeController(logger, bazCookieService, localizationService, menuService, serviceProvider);
        }

        #region View Test Metod

        [TestMethod()]
        public void ViewCompanyLicenseController()
        {
            // LisansAddOrUpdate
            var lisansEkle = _globalHelper.Post<Result<LisansViewModel>>(LocalPortlar.LisansServis + "/api/Lisans/AddOrUpdate", new LisansViewModel()
            {
                Name = "test",
            });
            // KurumLisansAdd
            var KurumLisansAdd = _globalHelper.Post<Result<KurumLisansViewModel>>(LocalPortlar.LisansServis + "/api/KurumLisans/Add", new KurumLisansViewModel()
            {
                IlgiliKurumId = 82,
                KurumID = 82,
                LisansId = lisansEkle.Result.Value.TabloID,
                Name = "test",
                SonKullanimTarihi = DateTime.Now,
                LisansKisiSayisi = 12
            });

            //Act-1 CompanyLicenseDefinition
            var actual = _companyLicenseController.CompanyLicenseDefinition() as ViewResult;

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual.ViewName) || actual.ViewName == "CompanyLicenseDefinition");

            //Act-2 CompanyLicenseUpdate
            var actual1 = _companyLicenseController.CompanyLicenseUpdate(KurumLisansAdd.Result.Value.TabloID) as ViewResult;

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual1.ViewName) || actual1.ViewName == "CompanyLicenseUpdate");

            //Act-3 CompanyLicenseList
            var actual2 = _companyLicenseController.CompanyLicenseList() as ViewResult;

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual2.ViewName) || actual2.ViewName == "CompanyLicenseList");

            //Act-4 EmployeeManagement
            var actual3 = _companyLicenseController.EmployeeManagement() as ViewResult;

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual3.ViewName) || actual3.ViewName == "EmployeeManagement");

            //LisansKurumRemove
            var LisansKurumRemove = _globalHelper.Get<Result<bool>>(LocalPortlar.LisansServis + "/api/KurumLisans/SetDelete/" + KurumLisansAdd.Result.Value.TabloID);

            // LisansRemove
            var LisansRemove = _globalHelper.Get<Result<bool>>(LocalPortlar.LisansServis + "/api/Lisans/Remove/" + lisansEkle.Result.Value.TabloID);
        }

        #endregion View Test Metod

        #region Api Test Metod

        [TestMethod()]
        public void ApiCompanyLicenseController()
        {
            // LisansAddOrUpdate
            var lisansEkle = _globalHelper.Post<Result<LisansViewModel>>(LocalPortlar.LisansServis + "/api/Lisans/AddOrUpdate", new LisansViewModel()
            {
                Name = "test",
            });

            var model = new KurumLisansDetayKayitModel
            {
                KurumID = 82,
                IlgiliKurumId = 82,
                LisansId = lisansEkle.Result.Value.TabloID,
                Name = "test1",
                SonKullanimTarihi = DateTime.Now.AddMonths(11),
                LisansZamanId = 1,
                LisansKisiSayisi = 50
            };

            //Assert-1 Add
            var kurumLisansAdd = _companyLicenseController.AddOrUpdate(model);
            Assert.AreEqual(kurumLisansAdd.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kurumLisansAdd.IsSuccess);
            Assert.IsNotNull(kurumLisansAdd.Value);

            model.TabloID = kurumLisansAdd.Value.TabloID;
            model.Name = "test2";

            //Assert-2 Update
            var kurumLisansUpdate = _companyLicenseController.AddOrUpdate(model);
            Assert.AreEqual(kurumLisansUpdate.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kurumLisansUpdate.IsSuccess);
            Assert.IsNotNull(kurumLisansUpdate.Value);

            // Assert-3 ListForKurum
            var listForKurum = _companyLicenseController.ListForKurum();
            Assert.AreEqual(listForKurum.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(listForKurum.IsSuccess);
            Assert.IsNotNull(listForKurum.Value);

            // Assert-4 GetKurumLisansData
            var getKurumLisansData = _companyLicenseController.GetKurumLisansData(kurumLisansAdd.Value.TabloID);
            Assert.AreEqual(getKurumLisansData.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(getKurumLisansData.IsSuccess);
            Assert.IsNotNull(getKurumLisansData.Value);

            var kurumKisiModel = new LisansKurumKisiAbonelikTanimlariViewModel()
            {
                LisansAboneKisiId = 129,
                LisansAbonelikBaslangicTarihi = DateTime.Now,
                LisansGenelTanimId = kurumLisansAdd.Value.TabloID,
                LisansAboneKurumId = 82,
                LisansAbonelikBitisTarihi = DateTime.Now
            };

            // Assert-5 KurumKisiLisansEslestirmesi
            var kurumKisiLisansEslestirmesi = _companyLicenseController.KurumKisiLisansEslestirmesi(kurumKisiModel);
            Assert.AreEqual(kurumKisiLisansEslestirmesi.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kurumKisiLisansEslestirmesi.IsSuccess);
            Assert.IsNotNull(kurumKisiLisansEslestirmesi.Value);

            //Assert-6 KurumLisansaBagliAktifKisiSayisi
            var kurumLisansaBagliAktifKisiSayisi = _companyLicenseController.KurumLisansaBagliAktifKisiSayisi(lisansEkle.Result.Value.TabloID);
            Assert.AreEqual(kurumLisansaBagliAktifKisiSayisi.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kurumLisansaBagliAktifKisiSayisi.IsSuccess);
            Assert.IsNotNull(kurumLisansaBagliAktifKisiSayisi.Value);

            //Assert-7 KurumLisansKisiSayisiGetir
            var kurumLisansKisiSayisiGetir = _companyLicenseController.KurumLisansKisiSayisiGetir(kurumLisansAdd.Value.TabloID);
            Assert.AreEqual(kurumLisansKisiSayisiGetir.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kurumLisansKisiSayisiGetir.IsSuccess);
            Assert.IsNotNull(kurumLisansKisiSayisiGetir.Value);

            // Assert-8 KurumKisiLisansVerileriGetir
            var kurumKisiLisansVerileriGetir = _companyLicenseController.KurumKisiLisansVerileriGetir();
            Assert.AreEqual(kurumKisiLisansVerileriGetir.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kurumKisiLisansVerileriGetir.IsSuccess);
            Assert.IsNotNull(kurumKisiLisansVerileriGetir.Value);

            // Assert-9 LisansDeaktifEt
            var lisansDeaktifEt = _companyLicenseController.LisansDeaktifEt(kurumKisiLisansEslestirmesi.Value.TabloID);
            Assert.AreEqual(lisansDeaktifEt.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(lisansDeaktifEt.IsSuccess);
            Assert.IsNotNull(lisansDeaktifEt.Value);

            // Assert- SetDelete
            var setDelete = _companyLicenseController.SetDelete(kurumLisansAdd.Value.TabloID);
            Assert.AreEqual(setDelete.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(setDelete.IsSuccess);
            Assert.IsTrue(setDelete.Value);

            // LisansRemove
            var LisansRemove = _globalHelper.Get<Result<bool>>(LocalPortlar.LisansServis + "/api/Lisans/Remove/" + lisansEkle.Result.Value.TabloID);

            kurumKisiModel.LisansGenelTanimId = 2210;
            var kurumKisiLisansEslestirmesi1 = _companyLicenseController.KurumKisiLisansEslestirmesi(kurumKisiModel);

            var KisiVarOlanLisansAbonelik = _companyLicenseController.KisiVarOlanLisansAbonelik();
            Assert.IsTrue(KisiVarOlanLisansAbonelik.IsSuccess);
            Assert.IsNotNull(KisiVarOlanLisansAbonelik.Value);

            _companyLicenseController.ControllerContext.HttpContext = new DefaultHttpContext();
            _companyLicenseController.ControllerContext.HttpContext.Connection.LocalIpAddress = IPAddress.Any;
            KurumLisansOdemeModel lisansModel = new()
            {
                KisiKimlikNo = "24586542735",
                GecerliOlduguGun = 30,
                LisansId = 1031,
                LisansKisiSayisi = 100,
                SonKullanimTarihi = DateTime.Now.AddMonths(12),
                KurumID = 82,
                IlgiliKurumId = 82,
                LisansZamanId = 13,
                Name = "OctaPull PRO+",
                FaturaAdresi = new()
                {
                    Address = "Teest",
                    City = "test",
                    ContactName = "test",
                    Country = "test"
                },
                CepTelefonNumarasi = "+905552222222"
            };

            var PurchaseLicense = _companyLicenseController.KurumLisansOdeme(lisansModel);
            Assert.IsTrue(PurchaseLicense.IsSuccess);
            Assert.IsNotNull(PurchaseLicense.Value);

            KurumLisansOdemeModel lisansModel2 = new()
            {
                KisiKimlikNo = "32145698765",
                GecerliOlduguGun = 30,
                LisansId = 1031,
                LisansKisiSayisi = 100,
                SonKullanimTarihi = DateTime.Now.AddMonths(12),
                KurumID = 82,
                IlgiliKurumId = 82,
                LisansZamanId = 13,
                Name = "OctaPull PRO+",
                FaturaAdresi = new()
                {
                    Address = "Teest",
                    City = "test",
                    ContactName = "test",
                    Country = "test"
                },
                IyzicoUrunPlaniToken = "0c07aab0-d85a-4bc9-98c2-456898c4aeaa", // OctaMeet 1 kişi plan reference code
                CepTelefonNumarasi = "+905552222222"
            };

            var PurchaseLicense2 = _companyLicenseController.KurumLisansAboneOdeme(lisansModel2);
            Assert.IsTrue(PurchaseLicense2.IsSuccess);
            Assert.IsNotNull(PurchaseLicense2.Value);

            var ListForView = _companyLicenseController.ListForView();
            Assert.IsTrue(ListForView.IsSuccess);
            Assert.IsNotNull(ListForView.Value);

            var tokenDictionary = new Dictionary<string, StringValues>();
            var key = "token";
            var tokenValue = PurchaseLicense.Value.Token;
            tokenDictionary.Add(key, tokenValue);
            FormCollection tokenCollection = new(tokenDictionary);
            _homeController.ControllerContext.HttpContext = new DefaultHttpContext();
            _homeController.ControllerContext.HttpContext.Request.Form = tokenCollection;
            _homeController.PaymentResult();
        }

        #endregion Api Test Metod
    }
}