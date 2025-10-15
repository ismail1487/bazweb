using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager;
using Baz.RequestManager.Abstracts;
using Baz.SharedSession;
using BazWebApp;
using BazWebApp.Controllers;
using BazWebApp.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NWebsec.AspNetCore.Core.Web;

namespace WebAppUnitTests.Helper
{
    public class TestServerController
    {
        public TestServer CreateServer()
        {
            var _testServer = new TestServer(new WebHostBuilder()
                .UseConfiguration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.Development.json")
                    .Build()
                )
                .ConfigureTestServices(services =>
                {
                    services.RemoveAll(typeof(IBazCookieService));
                    services.AddSingleton<IBazCookieService, TestBazCookieService>();
                })
                .UseStartup<Startup>()
            );
            var httpAssor = _testServer.Services.GetService<IHttpContextAccessor>();
            httpAssor.HttpContext = new DefaultHttpContext() { RequestServices = _testServer.Services };
            httpAssor.HttpContext.Features.Set<IRequestCultureFeature>(new RequestCultureFeature(new RequestCulture(CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture), new CookieRequestCultureProvider()));
            //var cookieTest = _testServer.Services.GetService<IBazCookieService>();
            //var sessionTest = _testServer.Services.GetService<ISharedSession>();

            //var logger = _testServer.Services.GetService<ILogger<LoginRegisterController>>();
            //var requestHelper = _testServer.Services.GetService<IRequestHelper>();
            //var kurumService = _testServer.Services.GetService<IKurumService>();
            //var loginRegisterService = _testServer.Services.GetService<ILoginRegisterService>();
            //var kisiService = _testServer.Services.GetService<IKisiService>();

            //var loginRegisterController = new LoginRegisterController(logger, cookieTest, requestHelper, kurumService, loginRegisterService, sessionTest, kisiService);
            //var loginResult = loginRegisterController.Login(new Baz.Model.Entity.ViewModel.LoginModel
            var _helper = new RequestHelper();

            var sessionId = _helper.Post<Result<string>>(LocalPortlar.UserLoginregisterService + "/api/LoginRegister/Login", new LoginModel
            {
                EmailOrUserName = "b@mail.com",
                Password = "12345Aa!"
            });
            var cookieTest = _testServer.Services.GetService<IBazCookieService>();
            cookieTest.SetCookie(new()
            {
                KisiId = "129",
                KurumId = "82",
                Mail = "b@mail.com",
                Name = "Bilal",
                SessionId = sessionId.Result.Value
            });

            return _testServer;
        }
    }

}