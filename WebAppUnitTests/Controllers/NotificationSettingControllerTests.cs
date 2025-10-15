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
using BazWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WebAppUnitTests.Helper;

namespace WebAppUnitTests
{
    [TestClass()]
    public class NotificationSettingControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly NotificationSettingController _notificationSettingController;

        public NotificationSettingControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper { _headers = _helper._headers };
            var hatirlatmaAyarService = _testServer.Services.GetService<IHatirlatmaAyarService>();
            var postaciIslemlerService = _testServer.Services.GetService<IPostaciIslemlerService>();
            var bazCookieService = _testServer.Services.GetService<IBazCookieService>();
            var hatirlatmaKayitService = _testServer.Services.GetService<IHatirlatmaKayitService>();

            _notificationSettingController = new NotificationSettingController(hatirlatmaAyarService,
                postaciIslemlerService, bazCookieService, hatirlatmaKayitService);
        }

        [TestMethod()]
        public void ViewNotificationSettingController()
        {
            //Act - Add
            var Add = _notificationSettingController.Add() as ViewResult;
            _helper.Get<ViewResult>("/notificationsetting/Add");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(Add.ViewName) || Add.ViewName == "Add");

            //Act -List
            var List = _notificationSettingController.List() as ViewResult;
            _helper.Get<ViewResult>("/notificationsetting/List");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(List.ViewName) || List.ViewName == "List");

            //Act -SpecificNotificationList
            var SpecificNotificationList = _notificationSettingController.NotfList() as ViewResult;
            _helper.Get<ViewResult>("/notificationsetting/SpecificNotificationList");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(SpecificNotificationList.ViewName) || SpecificNotificationList.ViewName == "SpecificNotificationList");

            //Act -Update
            var Update = _notificationSettingController.Update() as ViewResult;
            _helper.Get<ViewResult>("/notificationsetting/Update/25");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(Update.ViewName) || Update.ViewName == "Update");

            //Act -AddSpecificNotification
            var AddSpecificNotification = _notificationSettingController.AddSpecificNotification() as ViewResult;
            _helper.Get<ViewResult>("/notificationsetting/SpecificNotification/Add");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(AddSpecificNotification.ViewName) || AddSpecificNotification.ViewName == "AddSpecificNotification");
        }

        [TestMethod()]
        public void ApiNotificationSettingControllerSettings()
        {
            //Assert HatirlatmaAddTest
            var ayaradd = _notificationSettingController.Add(new HatirlatmaGenelAyarlar
            {
                KurumID = 82,
                HatirlatmaAralikSikligiDakikaBazinda = 5,
                HatirlatmaMaksimumSayisi = 2,
                HatirlatmaEpostaYollasinMi = true,
                HatirlatmaSmsyollasinMi = false,
                HatirlatmaBaslamaZamanTipi = "GÜN",
                HatirlatmaBaslamaZamanTipiDegeri = 60
            });
            Assert.AreEqual(ayaradd.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(ayaradd.Value);

            //Assert negativeUpdate
            var negativeUpdate = _notificationSettingController.Update(new HatirlatmaGenelAyarlar
            {
            });
            Assert.IsFalse(negativeUpdate.IsSuccess);

            //Assert ayarupdate
            var ayarupdate = _notificationSettingController.Update(new HatirlatmaGenelAyarlar
            {
                TabloID = ayaradd.Value.TabloID,
                KurumID = 82,
                HatirlatmaAralikSikligiDakikaBazinda = 5,
                HatirlatmaMaksimumSayisi = 2,
                HatirlatmaEpostaYollasinMi = true,
                HatirlatmaSmsyollasinMi = false,
                HatirlatmaBaslamaZamanTipi = "SAAT",
                HatirlatmaBaslamaZamanTipiDegeri = 30
            });
            Assert.AreEqual(ayarupdate.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(ayarupdate.Value);

            //list
            var list = _notificationSettingController.DataList();
            Assert.AreEqual(list.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(list.Value);

            //Assert negativeGet
            var negativeGet = _notificationSettingController.Get(0);
            Assert.IsFalse(negativeGet.IsSuccess);

            //Assert get
            var get = _notificationSettingController.Get(ayaradd.Value.TabloID);
            Assert.AreEqual(get.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(get.Value);

            //Assert negativeDelete
            var negativeDelete = _notificationSettingController.Remove(0);
            Assert.IsFalse(negativeDelete.Value);

            //Assert Delete
            var delete = _notificationSettingController.Remove(ayaradd.Value.TabloID);
            Assert.AreEqual(delete.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(delete.Value);
        }

        [TestMethod()]
        public void ApiNotificationSettingControllerSpecNof()
        {
            //Assert AddRangeOlumsuzTest
            OzelBildirimModel addRangeModel2 = new()
            {
                BildirimGonderilecekKisiList = new List<int>() { 129 }
            };
            var AddRangeOlumsuzTest = _notificationSettingController.AddRange(addRangeModel2);
            Assert.IsFalse(AddRangeOlumsuzTest.IsSuccess);

            //notspeclist
            var notspeclist = _notificationSettingController.SpecNotList();
            Assert.AreEqual(notspeclist.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(notspeclist.Value);
        }
    }
}