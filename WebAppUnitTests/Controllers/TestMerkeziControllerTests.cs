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
    public class TestMerkeziControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly TestMerkeziController _testMerkeziController;

        public TestMerkeziControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper { _headers = _helper._headers };
            var testService = _testServer.Services.GetService<ITestService>();
            var paramService = _testServer.Services.GetService<IParamService>();
            var bazCookieService = _testServer.Services.GetService<IBazCookieService>();

            _testMerkeziController = new TestMerkeziController(testService, paramService, bazCookieService);
        }

        [TestMethod()]
        public void ViewTestMerkeziController()
        {
            //Act - 1
            var actual = _testMerkeziController.TestView() as ViewResult;
            _helper.Get<ViewResult>("/TestCenter/TestView");

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual.ViewName) || actual.ViewName == "TestView");

            //Act - 2
            var actual1 = _testMerkeziController.FaturaView() as ViewResult;
            _helper.Get<ViewResult>("/testcenter/fatura");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual1.ViewName) || actual1.ViewName == "fatura");
        }
    }
}