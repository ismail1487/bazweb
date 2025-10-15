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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebAppUnitTests.Helper;

namespace WebAppUnitTests
{
    [TestClass()]
    public class HomeControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly HomeController _homeController;
        private readonly IBazCookieService _cookieService;

        public HomeControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper();
            _globalHelper._headers = _helper._headers;

            var logger = _testServer.Services.GetService<ILogger<HomeController>>();
            var bazCookieService = _testServer.Services.GetService<IBazCookieService>();
            _cookieService = bazCookieService;
            var localizationService = _testServer.Services.GetService<ILocalizationService>();
            var menuService = _testServer.Services.GetService<IMenuService>();
            var serviceProvider = _testServer.Services.GetService<IServiceProvider>();
            _homeController = new HomeController(logger, bazCookieService, localizationService, menuService, serviceProvider);
        }

        [TestMethod()]
        public async Task ViewHomeController()
        {
            //Act - 2 Index
            var actual1 = await _homeController.Index();
            _helper.Get<ViewResult>("/Home/Index");

            //Act - 3 Privacy
            var actual2 = _homeController.Privacy() as ViewResult;
            _helper.Get<ViewResult>("/Home/Privacy");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual2.ViewName) || actual2.ViewName == "Privacy");

            //Act - 4 Error
            var m = _helper.Get<ViewResult>("/Home/Error");

            //Act - 11 PurchaseLicense
            var actual9 = _homeController.PurchaseLicense() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual9.ViewName) || actual9.ViewName == "PurchaseLicense");
        }

        [TestMethod()]
        public void ApiHomeController()
        {
            //AdminMi
            var adminMi = _homeController.AdminMi();
            Assert.IsTrue(adminMi.IsSuccess);
            Assert.IsNotNull(adminMi.Value);
        }
    }
}