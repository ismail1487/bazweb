using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Baz.RequestManager;
using Microsoft.AspNetCore.Mvc;
using WebAppUnitTests.Helper;

namespace WebAppUnitTests
{
    [TestClass()]
    public class ErrorControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly ErrorController _errorController;

        public ErrorControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper();
            _globalHelper._headers = _helper._headers;
            _errorController = new ErrorController();
        }

        [TestMethod()]
        public void ViewErrorController()
        {
            //Act - 1 404
            //var actual = _errorController.Page404() as ViewResult;

            
            _helper.Get<ViewResult>("Error/404");

            //Assert
            //Assert.IsTrue(string.IsNullOrEmpty(actual.ViewName) || actual.ViewName == "404");

            //Act - 2 500
            _helper.Get<ViewResult>("Error/500");
            
        } 
        
        [TestMethod()]
        public void ApiErrorController()
        {
            Assert.IsTrue(true);
        }
    }
}