using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class LicenseControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly LicenseController _licenseController;

        public LicenseControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper();
            _globalHelper._headers = _helper._headers;
            var lisansService = _testServer.Services.GetService<ILisansService>();

            _licenseController = new LicenseController(lisansService);
        }

        [TestMethod()]
        public void ViewLicenseController()
        {
            //Act - 1 LicenseDefinition
            var actual = _licenseController.LicenseDefinition() as ViewResult;

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual.ViewName) || actual.ViewName == "LicenseDefinition");

            //Act - 2 LicenseUpdate
            var actual1 = _licenseController.LicenseUpdate(1) as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual1.ViewName) || actual1.ViewName == "LicenseUpdate");

            //Act - 3 LicenseList
            var actual2 = _licenseController.LicenseList() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual2.ViewName) || actual2.ViewName == "LicenseList");
        }

        [TestMethod()]
        public void ApiLicenseController()
        {
            // LisansAddOrUpdate
            var lisansEkle = _globalHelper.Post<Result<LisansViewModel>>(LocalPortlar.LisansServis + "/api/Lisans/AddOrUpdate", new LisansViewModel()
            {
                Name = "LisansUnittest1",
            });

            var modulEkle = _globalHelper.Post<Result<ModulViewModel>>(LocalPortlar.LisansServis + "/api/Modul/AddOrUpdate", new ModulViewModel()
            {
                Name = "testUnit1",
            });

            var model = new LisansDetayKayitModel
            {
                Name = "Test234341" + Guid.NewGuid().ToString().Substring(0, 10),
                ModulId = new List<int>
                {
                    modulEkle.Result.Value.TabloID
                },
                LisansZamanlariList = new List<LisansZamanlariViewModel>
                {
                    new LisansZamanlariViewModel
                    {
                        LisansId = lisansEkle.Result.Value.TabloID,
                        LisansBedeliParaBirimiId = 1,
                        LisansBedeli = "10",
                        GecerliOlduguGun = 10
                    }
                }
            };

            // Assert-1 Add
            var addOrUpdate = _licenseController.AddOrUpdate(model);
            Assert.AreEqual(addOrUpdate.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(addOrUpdate.IsSuccess);
            Assert.IsNotNull(addOrUpdate.Value);

            model.Name = "Tesgsdfdf";
            model.TabloID = addOrUpdate.Value.TabloID;
            // Assert-2 Update
            var addOrUpdate1 = _licenseController.AddOrUpdate(model);
            Assert.AreEqual(addOrUpdate1.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(addOrUpdate1.IsSuccess);
            Assert.IsNotNull(addOrUpdate1.Value);

            // Assert-3 Lisans Detay
            var detay = _licenseController.GetLisansveDetayData(lisansEkle.Result.Value.TabloID);
            Assert.AreEqual(detay.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(detay.IsSuccess);
            Assert.IsNotNull(detay.Value);

            // Assert-3.1 Lisans Detay Negative
            var detayNegative = _licenseController.GetLisansveDetayData(0);
            Assert.IsFalse(detayNegative.IsSuccess);
            Assert.IsNull(detayNegative.Value);

            // Assert-4 LisansListForView
            var lisansList = _licenseController.LisansListForView();
            Assert.AreEqual(lisansList.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(lisansList.IsSuccess);
            Assert.IsNotNull(lisansList.Value);

            // Assert-5 ListZaman
            var lisZaman = _licenseController.ListZaman(lisansEkle.Result.Value.TabloID);
            Assert.AreEqual(lisZaman.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(lisZaman.IsSuccess);
            Assert.IsNotNull(lisZaman.Value);

            // Assert-6 Remove
            var rmv = _licenseController.Remove(lisansEkle.Result.Value.TabloID);
            Assert.AreEqual(rmv.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(rmv.IsSuccess);
            Assert.IsNotNull(rmv.Value);

            // Assert-7 Remove
            var rmv2 = _licenseController.Remove(addOrUpdate.Value.TabloID);

            //ModulRemove
            var ModulRemove = _globalHelper.Get<Result<bool>>(LocalPortlar.LisansServis + "/api/Modul/Remove/" + modulEkle.Result.Value.TabloID);
        }
    }
}