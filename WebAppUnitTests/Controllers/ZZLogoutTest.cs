using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Baz.RequestManager;
using Baz.RequestManager.Abstracts;
using Baz.SharedSession;
using BazWebApp.Controllers;
using BazWebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAppUnitTests.Helper;

namespace WebAppUnitTests.Controllers
{
    [TestClass()]
    public class ZZLogoutTest
    {
        public readonly RequestHelper _helper;
        private readonly LoginRegisterController _loginRegisterController;

        public ZZLogoutTest()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            //new RequestHelper {_headers = _helper._headers}
            var logger = _testServer.Services.GetService<ILogger<LoginRegisterController>>();
            var bazCookieService = _testServer.Services.GetService<IBazCookieService>();
            _testServer.Services.GetService<IRequestHelper>();
            var kurumService = _testServer.Services.GetService<IKurumService>();
            var loginRegisterService = _testServer.Services.GetService<ILoginRegisterService>();
            var sharedSession = _testServer.Services.GetService<ISharedSession>();
            var kisiService = _testServer.Services.GetService<IKisiService>();

            _loginRegisterController = new LoginRegisterController(logger, bazCookieService,
                kurumService, loginRegisterService, sharedSession, kisiService);
        }

        [TestMethod]
        public void Logout()
        {
            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor()
            };

            _loginRegisterController.ControllerContext = new ControllerContext(actionContext);
            var logout = _loginRegisterController.Logout();
            Assert.IsNotNull(logout);
        }
    }
}