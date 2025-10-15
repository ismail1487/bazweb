using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class GeographyControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly GeographyController _geographyController;

        public GeographyControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper();
            _globalHelper._headers = _helper._headers;
            var cografyaService = _testServer.Services.GetService<ICografyaService>();

            _geographyController = new GeographyController(cografyaService);
        }

        [TestMethod()]
        public void ViewGeographyController()
        {
            //Act - 1 CografyaAyrintiListesi
            var actual = _geographyController.CografyaAyrintiListesi() as ViewResult;

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual.ViewName) || actual.ViewName == "CografyaAyrintiListesi");

            //Act - 2 CografyaAyrintiKayit
            var actual1 = _geographyController.CografyaAyrintiKayit() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual1.ViewName) || actual1.ViewName == "CografyaAyrintiKayit");

            //Act - 3 CografyaAyrintiGuncelle
            var actual2 = _geographyController.CografyaAyrintiGuncelle(1) as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual2.ViewName) || actual2.ViewName == "CografyaAyrintiGuncelle");
        }

        [TestMethod()]
        public void ApiGeographyController()
        {
            // Assert-1 CografyaTanimKayit
            var add = _geographyController.CografyaTanimKayit(new CografyaListViewModel
            {
                CografyaTanim = "Unitdfg XXX1 sadads" + Guid.NewGuid().ToString().Substring(0, 10),
                CografyaAciklama = "Unit Test1 sdadsd" + Guid.NewGuid().ToString().Substring(0, 10),
                KurumId = 82,
                KisiId = 129,
                UlkeId = 1,
                SehirlerIDList = new() { 1, 2, 3 }
            });
            Assert.AreEqual(add.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(add.IsSuccess);
            Assert.IsNotNull(add.Value);

            // Assert-1.1 CografyaTanimKayit Negative
            var AddNegative = _geographyController.CografyaTanimKayit(new CografyaListViewModel
            {
                KurumId = 82,
                KisiId = 129
            });
            Assert.IsFalse(AddNegative.IsSuccess);
            Assert.AreEqual(0, AddNegative.Value);

            //Assert-2 CografyaTanimGuncelle
            var update = _geographyController.CografyaTanimGuncelle(new CografyaListViewModel
            {
                KisiId = 129,
                CografyaKutupanesiId = add.Value,
                CografyaTanim = "Unit XX asxsad" + Guid.NewGuid().ToString().Substring(0, 10),
                CografyaAciklama = "Unit Test2 saxaqsd" + Guid.NewGuid().ToString().Substring(0, 10),
                UlkeId = 1,
                SehirlerIDList = new() { 1, 2, 3, 4 }
            });
            Assert.AreEqual(update.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(update.IsSuccess);
            Assert.IsNotNull(update.Value);

            // Assert-2.1 CografyaTanimGuncelle Negative
            var updateNegative = _geographyController.CografyaTanimGuncelle(new CografyaListViewModel
            {
                KisiId = 129,
                UlkeId = 1,
                SehirlerIDList = new() { 1, 2, 3, 4 }
            });
            Assert.IsFalse(updateNegative.IsSuccess);
            Assert.IsNull(updateNegative.Value);

            // Assert-3 CografyaTanimIdsineGoreGetir
            var IdGetir = _geographyController.CografyaTanimIdsineGoreGetir(add.Value);
            Assert.AreEqual(IdGetir.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(IdGetir.IsSuccess);
            Assert.IsNotNull(IdGetir.Value);

            //Assert-3.1 CografyaTanimIdsineGoreGetir  Negative
            var IdGetirNegative = _geographyController.CografyaTanimIdsineGoreGetir(0);
            Assert.IsFalse(IdGetirNegative.IsSuccess);
            Assert.IsNull(IdGetirNegative.Value);

            // Assert-9 CografyaTanimSil
            var sil = _geographyController.CografyaTanimSil(add.Value);
            Assert.AreEqual(sil.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(sil.IsSuccess);
            Assert.IsNotNull(sil.Value);

            //Assert-9.1 CografyaTanimSil Negative
            var silNegative = _geographyController.CografyaTanimSil(0);
            Assert.IsFalse(silNegative.IsSuccess);
            Assert.IsFalse(silNegative.Value);
        }
    }
}