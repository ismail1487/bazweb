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
    public class YetkiMerkeziControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly YetkiMerkeziController _yetkiMerkeziController;

        public YetkiMerkeziControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper { _headers = _helper._headers };
            var yetkiMerkeziService = _testServer.Services.GetService<IYetkiMerkeziService>();
            var bazCookieService = _testServer.Services.GetService<IBazCookieService>();

            _yetkiMerkeziController = new YetkiMerkeziController(yetkiMerkeziService, bazCookieService);
        }

        [TestMethod()]
        public void ViewYetkiMerkeziController()
        {
            //Act - 1 YetkilendirmeMerkezi
            var actual = _yetkiMerkeziController.YetkilendirmeMerkezi() as ViewResult;
            _helper.Get<ViewResult>("/AuthCenter");

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual.ViewName) || actual.ViewName == "YetkilendirmeMerkezi");

            //Act - 2 ErisimYetkiList
            var actual1 = _yetkiMerkeziController.ErisimYetkiList() as ViewResult;
            _helper.Get<ViewResult>("/AuthCenter/AccessAuthList");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual1.ViewName) || actual1.ViewName == "ErisimYetkiList");
        }

        [TestMethod()]
        public void ApiYetkiMerkeziController()
        {
            ErisimYetkileriKayitModel erisimYetkileriKayitModel = new()
            {
                ErisimYetkisiVerilenSayfaIdList = new List<int>()
                {
                    21,22
                },
                IlgiliKurumOrganizasyonBirimIdList = new List<int>()
                {
                    441
                },
                KisiID = 129,
                KurumID = 82
            };
            //Erisim SAVE
            var erisimYetkiKayit = _yetkiMerkeziController.ErisimYetkilendirmeTanimlariKaydet(erisimYetkileriKayitModel);
            Assert.AreEqual(erisimYetkiKayit.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(erisimYetkiKayit.IsSuccess);
            Assert.IsNotNull(erisimYetkiKayit.Value);

            //Erisim LİST
            var erisimYetkiList = _yetkiMerkeziController.ErisimYetkiTanimListGetir();
            Assert.AreEqual(erisimYetkiList.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(erisimYetkiList.IsSuccess);
            Assert.IsNotNull(erisimYetkiList.Value);

            //KisiYetkiList
            var KisiYetkiList = _yetkiMerkeziController.KisiYetkilerListGetir(129);
            Assert.AreEqual(KisiYetkiList.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(KisiYetkiList.IsSuccess);
            Assert.IsNotNull(KisiYetkiList.Value);

            //Erisim YetkiSil
            foreach (var eyk in erisimYetkiKayit.Value)
            {
                var erisimyetkitanimisil = _yetkiMerkeziController.ErisimYetkiTanimiSil(eyk.TabloID);
                Assert.AreEqual(erisimyetkitanimisil.StatusCode, (int)ResultStatusCode.Success);
                Assert.IsTrue(erisimyetkitanimisil.IsSuccess);
                Assert.IsTrue(erisimyetkitanimisil.Value);
            }

            // Yetki Tanımı Sil Negative
            var yetkitanimisilnegative = _yetkiMerkeziController.ErisimYetkiTanimiSil(0);
            Assert.IsFalse(yetkitanimisilnegative.IsSuccess);
        }
    }
}