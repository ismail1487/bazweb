using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Baz.Model.Entity;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager;
using BazWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WebAppUnitTests.Helper;

namespace WebAppUnitTests
{
    [TestClass()]
    public class EkParametrelerControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly EkParametrelerController _ekParametrelerController;

        public EkParametrelerControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new();
            _globalHelper._headers = _helper._headers;

            var paramService = _testServer.Services.GetService<IParamService>();
            var bazCookieService = _testServer.Services.GetService<IBazCookieService>();
            var ekParametrelerService = _testServer.Services.GetService<IEkParametrelerService>();
            _ekParametrelerController = new EkParametrelerController(paramService, bazCookieService, ekParametrelerService);
        }

        [TestMethod()]
        public void ViewEkParametrelerController()
        {
            var kurumEk = _ekParametrelerController.KurumEkParametreKaydet(new KurumEkParametreViewModel
            {
                ParametreAdi = "Test Unit 124523rfc23" + Guid.NewGuid().ToString().Substring(0, 10),
                ParametreGosterimTipi = 2,
                ParametreTipi = 1,
            });

            var kisiEk = _ekParametrelerController.KisiEkParametreKaydet(new KisiEkParametreViewModel
            {
                ParametreAdi = "Unit Test 234r fwdf23r" + Guid.NewGuid().ToString().Substring(0, 10),
                ParametreGosterimTipi = 5,
                ParametreTipi = 2
            });

            //Act - 1 KurumEkParametrelerTanimlamaSayfasi
            var actual = _ekParametrelerController.KurumEkParametrelerTanimlamaSayfasi() as ViewResult;

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual.ViewName) || actual.ViewName == "KurumEkParametrelerTanimlamaSayfasi");

            //Act - 2 KurumEkParametrelerListelemeSayfasi
            var actual1 = _ekParametrelerController.KurumEkParametrelerListelemeSayfasi() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual1.ViewName) || actual1.ViewName == "KurumEkParametrelerListelemeSayfasi");

            //Act - 3 KurumEkParametreGuncelleGit
            var actual2 = _ekParametrelerController.KurumEkParametreGuncelleGit(kurumEk.Value.TabloID) as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual2.ViewName) || actual2.ViewName == "KurumEkParametrelerGuncellemeSayfasi");

            //Act - 4 KisiEkParametrelerTanimlamaSayfasi
            var actual3 = _ekParametrelerController.KisiEkParametrelerTanimlamaSayfasi() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual3.ViewName) || actual3.ViewName == "KisiEkParametrelerTanimlamaSayfasi");

            //Act - 5 KisiEkParametrelerListelemeSayfasi
            var actual4 = _ekParametrelerController.KisiEkParametrelerListelemeSayfasi() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual4.ViewName) || actual4.ViewName == "KisiEkParametrelerListelemeSayfasi");

            //Act - 6 KisiEkParametreGuncelleGit
            var actual5 = _ekParametrelerController.KisiEkParametreGuncelleGit(kisiEk.Value.TabloID) as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual5.ViewName) || actual5.ViewName == "KisiEkParametrelerGuncellemeSayfasi");

            _ekParametrelerController.KurumEkParametreSil(kurumEk.Value.TabloID);
            _globalHelper.Get<Result<bool>>(LocalPortlar.KurumService + "/api/KurumEkParametre/KurumEkParamHardDelete/" + kurumEk.Value.TabloID);
            _ekParametrelerController.KisiEkParametreSil(kisiEk.Value.TabloID);
            _globalHelper.Get<Result<KisiEkParametreler>>(LocalPortlar.KisiServis + "/api/KisiEkParametre/KisiEkParamHardDelete/" + kisiEk.Value.TabloID);
        }

        [TestMethod()]
        public void ApiEkParametrelerController()
        {
            var kurumModel = new KurumEkParametreViewModel
            {
                ParametreAdi = "Test Unit 124321e3124" + Guid.NewGuid().ToString().Substring(0, 10),
                ParametreGosterimTipi = 2,
                ParametreTipi = 2,
                OlasiDegerler = new List<KurumEkParametreOlasiDegerViewModel>
                {
                    new KurumEkParametreOlasiDegerViewModel
                    {
                        OlasiDegerAdi = "test123312312" + Guid.NewGuid().ToString().Substring(0, 10)
                    }
                }
            };

            // Assert-1 KurumEkParametreKaydet
            var kurumEk = _ekParametrelerController.KurumEkParametreKaydet(kurumModel);
            Assert.AreEqual(kurumEk.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kurumEk.IsSuccess);
            Assert.IsNotNull(kurumEk.Value);

            // Assert-1.1 KurumEkParametreKaydet Negative
            var kurumEkNegative = _ekParametrelerController.KurumEkParametreKaydet(new KurumEkParametreViewModel
            {
                ParametreGosterimTipi = 2,
                ParametreTipi = 2,
            });

            Assert.IsFalse(kurumEkNegative.IsSuccess);
            Assert.IsNull(kurumEkNegative.Value);

            // Assert-2 KurumEkParametreList
            var kurumEkList = _ekParametrelerController.KurumEkParametreList();
            Assert.AreEqual(kurumEkList.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kurumEkList.IsSuccess);
            Assert.IsNotNull(kurumEkList.Value);

            //Assert-3 EkParametreListesiID
            var kurumEkListId = _ekParametrelerController.EkParametreListesiID(kurumEk.Value.TabloID);
            Assert.AreEqual(kurumEkListId.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kurumEkListId.IsSuccess);
            Assert.IsNotNull(kurumEkListId.Value);

            // Assert-3.1 kurumEkListIdNegative
            var kurumEkListIdNegative = _ekParametrelerController.EkParametreListesiID(0);
            Assert.IsFalse(kurumEkListIdNegative.IsSuccess);
            Assert.IsNull(kurumEkListIdNegative.Value);

            kurumModel.ParametreAdi = "Test Unit XX12 234234";
            kurumModel.TabloID = kurumEk.Value.TabloID;
            kurumModel.OlasiDegerler = new List<KurumEkParametreOlasiDegerViewModel>
            {
                new KurumEkParametreOlasiDegerViewModel
                {
                    OlasiDegerAdi = "test12331231235 2342341",
                    TabloID = kurumModel.OlasiDegerler[0].TabloID
                }
            };

            // Assert-4 KurumEkParametreGuncelle
            var kurumGun = _ekParametrelerController.KurumEkParametreGuncelle(kurumModel);
            Assert.AreEqual(kurumGun.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kurumGun.IsSuccess);
            Assert.IsNotNull(kurumGun.Value);

            // Assert-4.1 KurumEkParametreKaydet Negative
            var kurumGunNegative = _ekParametrelerController.KurumEkParametreGuncelle(new KurumEkParametreViewModel
            {
                ParametreGosterimTipi = 2,
                ParametreTipi = 2
            });
            Assert.IsFalse(kurumGunNegative.IsSuccess);
            Assert.IsNull(kurumGunNegative.Value);

            // Assert-5 Kurum Ek Sil
            var kurumSil = _ekParametrelerController.KurumEkParametreSil(kurumEk.Value.TabloID);
            Assert.AreEqual(kurumSil.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kurumSil.IsSuccess);
            Assert.IsTrue(kurumSil.Value);

            // Assert-5.1  Kurum Ek Sil Negative
            var kurumEkSilNegative = _ekParametrelerController.KurumEkParametreSil(0);
            Assert.IsFalse(kurumEkSilNegative.IsSuccess);
            Assert.IsFalse(kurumEkSilNegative.Value);

            // hard delete
            var y = _globalHelper.Get<Result<bool>>(LocalPortlar.KurumService + "/api/KurumEkParametre/KurumEkParamHardDelete/" + kurumEk.Value.TabloID);

            var kisiModel = new KisiEkParametreViewModel
            {
                ParametreAdi = "Unit Test 23423tfg f" + Guid.NewGuid().ToString().Substring(0, 10),
                ParametreGosterimTipi = 5,
                ParametreTipi = 2
            };

            // Assert-6 KisiEkParametreKaydet
            var kisiEk = _ekParametrelerController.KisiEkParametreKaydet(kisiModel);
            Assert.AreEqual(kisiEk.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kisiEk.IsSuccess);
            Assert.IsNotNull(kisiEk.Value);

            // Assert-6.1 KisiEkParametreKaydet Negative
            var kisiEkNegative = _ekParametrelerController.KisiEkParametreKaydet(new KisiEkParametreViewModel
            {
                ParametreGosterimTipi = 5,
                ParametreTipi = 2
            });
            Assert.IsFalse(kisiEkNegative.IsSuccess);
            Assert.IsNull(kisiEkNegative.Value);

            // Assert-7 KisiEkParametreList
            var kisiEkList = _ekParametrelerController.KisiEkParametreList();
            Assert.AreEqual(kisiEkList.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kisiEkList.IsSuccess);
            Assert.IsNotNull(kisiEkList.Value);

            //Assert-8 KisiEkParametreListesiID
            var kisiEkListId = _ekParametrelerController.KisiEkParametreListesiID(kisiEk.Value.TabloID);
            Assert.AreEqual(kisiEkListId.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kisiEkListId.IsSuccess);
            Assert.IsNotNull(kisiEkListId.Value);

            // Assert-8.1 kişiEkListIdNegative
            var kisiEkListIdNegative = _ekParametrelerController.KisiEkParametreListesiID(0);
            Assert.IsFalse(kisiEkListIdNegative.IsSuccess);
            Assert.IsNull(kisiEkListIdNegative.Value);

            kisiModel.ParametreAdi = "Test Unit XX12 234ed " + Guid.NewGuid().ToString().Substring(0, 10);
            kisiModel.TabloID = kisiEk.Value.TabloID;

            // Assert-9 KişiEkParametreGuncelle
            var kisiGun = _ekParametrelerController.KisiEkParametreGuncelle(kisiModel);
            Assert.AreEqual(kisiGun.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kisiGun.IsSuccess);
            Assert.IsNotNull(kisiGun.Value);

            // Assert-9.1 KişiEkParametreKaydet Negative
            var kisiGunNegative = _ekParametrelerController.KisiEkParametreGuncelle(new KisiEkParametreViewModel
            {
                ParametreGosterimTipi = 5,
                ParametreTipi = 2
            });
            Assert.IsFalse(kisiGunNegative.IsSuccess);
            Assert.IsNull(kisiGunNegative.Value);

            // Assert-10 Kişi Ek Sil
            var kisiSil = _ekParametrelerController.KisiEkParametreSil(kisiEk.Value.TabloID);
            Assert.AreEqual(kisiSil.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kisiSil.IsSuccess);
            Assert.IsTrue(kisiSil.Value);

            // Assert-10.1  Kurum Ek Sil Negative
            var kisiEkSilNegative = _ekParametrelerController.KisiEkParametreSil(0);
            Assert.IsFalse(kisiEkSilNegative.IsSuccess);
            Assert.IsFalse(kisiEkSilNegative.Value);

            // hard delete
            _globalHelper.Get<Result<KisiEkParametreler>>(LocalPortlar.KisiServis + "/api/KisiEkParametre/KisiEkParamHardDelete/" + kisiEk.Value.TabloID);
        }
    }
}