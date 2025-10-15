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
    public class IcerikGenelControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly IcerikGenelController _icerikGenelController;

        public IcerikGenelControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper();
            _globalHelper._headers = _helper._headers;
            var icerikKutuphaneService = _testServer.Services.GetService<IIcerikKutuphaneService>();

            _icerikGenelController = new IcerikGenelController(icerikKutuphaneService);
        }

        [TestMethod()]
        public void ViewIcerikGenelController()
        {
            //Add
            var add = _icerikGenelController.AddOrUpdate(new IcerikHedefKitleViewModel
            {
                IcerikBaslik = "Test Başlık sdfsdfdf2234",
                IcerikOzetMetni = "Test Özet asdsd143",
                IcerikBitisZamani = DateTime.Now,
                IcerikYayinlanmaZamani = DateTime.Now,
                IcerikTamMetin = "Test metin 1123",
                IcerikTaslakMi = true,
                KisiIds = new() { 129, 130 },
                HedefIds = new() { 124 }
            });

            //Act - 1 IcerikAdd
            var actual = _icerikGenelController.IcerikAdd() as ViewResult;

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual.ViewName) || actual.ViewName == "Add");

            //Act - 2 IcerikList
            var actual1 = _icerikGenelController.IcerikList() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual1.ViewName) || actual1.ViewName == "List");

            //Act - 3 ViewingList
            var actual2 = _icerikGenelController.ViewingList(add.Value.TabloID) as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual2.ViewName) || actual2.ViewName == "ViewingList");

            //Act - 4 IcerikUpdate
            var actual3 = _icerikGenelController.IcerikUpdate(add.Value.TabloID) as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual3.ViewName) || actual3.ViewName == "Update");

            //Act - 5 IcerikGoruntule
            var actual4 = _icerikGenelController.IcerikGoruntule(add.Value.TabloID) as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual4.ViewName) || actual4.ViewName == "Detail");

            //Delete
            var dlt = _icerikGenelController.IcerikKutuphanesiSil(add.Value.TabloID);
        }

        [TestMethod()]
        public void ApiIcerikGenelController()
        {
            // Assert-1
            var add = _icerikGenelController.AddOrUpdate(new IcerikHedefKitleViewModel
            {
                IcerikBaslik = "Test Başlık sdfsdfdf22",
                IcerikOzetMetni = "Test Özet asdsd3",
                IcerikBitisZamani = DateTime.Now,
                IcerikYayinlanmaZamani = DateTime.Now,
                IcerikTamMetin = "Test metin 123",
                IcerikTaslakMi = true,
                KisiIds = new() { 129, 130 },
                HedefIds = new() { 124 }
            });
            Assert.AreEqual(add.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(add.IsSuccess);
            Assert.IsNotNull(add.Value);

            //Assert-1.1
            var addNegative = _icerikGenelController.AddOrUpdate(new IcerikHedefKitleViewModel
            {
                IcerikBitisZamani = DateTime.Now,
                IcerikTaslakMi = true,
                KisiIds = new() { 129, 130 },
                HedefIds = new() { 124 }
            });
            Assert.IsFalse(addNegative.IsSuccess);
            Assert.IsNull(addNegative.Value);

            // Assert-2 Update
            var update = _icerikGenelController.AddOrUpdate(new IcerikHedefKitleViewModel
            {
                TabloID = add.Value.TabloID,
                IcerikBaslik = "Test Başlıkasdsad sdfsdfdf22",
                IcerikOzetMetni = "Test Özetsadasd asdsd3",
                IcerikBitisZamani = DateTime.Now,
                IcerikYayinlanmaZamani = DateTime.Now,
                IcerikTamMetin = "Test metinsads 123",
                IcerikTaslakMi = true,
                KisiIds = new() { 129, 130 },
                HedefIds = new() { 124 }
            });
            Assert.AreEqual(update.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(update.IsSuccess);
            Assert.IsNotNull(update.Value);

            // Assert-2.1 Update Negative
            var updateNegative = _icerikGenelController.AddOrUpdate(new IcerikHedefKitleViewModel
            {
                TabloID = add.Value.TabloID,
                IcerikBitisZamani = DateTime.Now,
                KisiIds = new() { 129, 130 },
                HedefIds = new() { 124 }
            });
            Assert.IsFalse(updateNegative.IsSuccess);
            Assert.IsNull(updateNegative.Value);

            // Assert 3 Get
            var get = _icerikGenelController.Get(add.Value.TabloID);
            Assert.AreEqual(get.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(get.IsSuccess);
            Assert.IsNotNull(get.Value);

            // Assert 3.1 Get Negative
            var getNegative = _icerikGenelController.Get(0);
            Assert.IsFalse(getNegative.IsSuccess);
            Assert.IsNull(getNegative.Value);

            // Assert-4 List
            var list = _icerikGenelController.ListForView();
            Assert.AreEqual(list.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(list.IsSuccess);
            Assert.IsNotNull(list.Value);

            // Assert-5 Duyuru Göster
            var duyuru = _icerikGenelController.DuyuruGoster();
            Assert.AreEqual(duyuru.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(duyuru.IsSuccess);
            Assert.IsNotNull(duyuru.Value);

            // Assert-6 Görülme zamanı
            var gorulme = _icerikGenelController.GorulmeZamaniKaydet(new IcerikKutuphanesiGorulmeZamanlari
            {
                KisiID = 129,
                IcerikKutuphanesiID = add.Value.TabloID
            });
            Assert.AreEqual(gorulme.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(gorulme.IsSuccess);
            Assert.IsNotNull(gorulme.Value);

            // Assert-6.1 Gorulme Negative
            var gorulmeNegative = _icerikGenelController.GorulmeZamaniKaydet(new IcerikKutuphanesiGorulmeZamanlari
            {
                KisiID = 129,
                IcerikKutuphanesiID = 2
            });
            Assert.IsFalse(gorulmeNegative.IsSuccess);
            Assert.IsNull(gorulmeNegative.Value);

            // Assert-7 Gorulme zamanı List
            var gorulmeList = _icerikGenelController.GorulmeZamanlariListele(gorulme.Value.TabloID);
            Assert.AreEqual(gorulmeList.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(gorulmeList.IsSuccess);
            Assert.IsNotNull(gorulmeList.Value);

            // Assert-8 Icerik Sil
            var sil = _icerikGenelController.IcerikKutuphanesiSil(add.Value.TabloID);
            Assert.AreEqual(sil.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(sil.IsSuccess);
            Assert.IsTrue(sil.Value);

            // Assert-8.1 Icerik Sil Negative
            var silNegative = _icerikGenelController.IcerikKutuphanesiSil(0);
            Assert.IsFalse(silNegative.IsSuccess);
            Assert.IsFalse(silNegative.Value);

            //Delete
            var delete =
                _globalHelper.Get<Result<IcerikKutuphanesiGorulmeZamanlari>>(LocalPortlar.IYSService + "/api/IcerikGenel/GorulmeZamanlariSil/" + gorulme.Value.TabloID);
        }
    }
}