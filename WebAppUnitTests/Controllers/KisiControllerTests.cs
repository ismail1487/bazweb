using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Baz.ProcessResult;
using Baz.RequestManager;
using BazWebApp.Services;
using Microsoft.Extensions.DependencyInjection;
using WebAppUnitTests.Helper;

namespace WebAppUnitTests
{
    [TestClass()]
    public class KisiControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly KisiController _kisiController;
        public KisiControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper();
            _globalHelper._headers = _helper._headers;
            var bazCookieService = _testServer.Services.GetService<IBazCookieService>();
            var kisiService = _testServer.Services.GetService<IKisiService>();

            _kisiController = new KisiController(bazCookieService,kisiService);
        }

        
        [TestMethod()]
        public void ApiKisiController()
        {
            // Assert-1 KurumaBagliKisiListGetirById
            var kurumaBagliKisilerList = _kisiController.KurumaBagliKisiListGetirById(82);
            Assert.AreEqual(kurumaBagliKisilerList.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kurumaBagliKisilerList.IsSuccess);
            Assert.IsNotNull(kurumaBagliKisilerList.Value);
        }
    }
}