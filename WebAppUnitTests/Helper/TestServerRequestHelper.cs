using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.RequestManager;
using Baz.RequestManager.Abstracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Baz.Model.Pattern;
using Baz.ProcessResult;
using BazWebApp;
using BazWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Net.Http.Headers;
using BazWebApp.Services;

namespace WebAppUnitTests.Helper
{
    public class TestServerRequestHelper
    {
        public IRequestHelper CreateHelper(TestLoginUserManager manager = null)
        {
            var _server = new TestServer(new WebHostBuilder()
                .UseConfiguration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.Development.json")
                    .Build()
                )
                .ConfigureTestServices(services =>
                {
                    services.AddHttpContextAccessor();
                    services.RemoveAll(typeof(IBazCookieService));
                    services.AddSingleton<IBazCookieService, TestBazCookieService>();
                })
                .UseStartup<Startup>()

            );

            var _client = _server.CreateClient();
            var _helper = new RequestHelper(_client);

            if (_helper._headers == null)
            {
                var x = new RequestHelper().Post<Result<string>>(LocalPortlar.UserLoginregisterService + "/api/LoginRegister/Login", new LoginModel
                {
                    EmailOrUserName = "b@mail.com",
                    Password = "12345Aa!"
                });
                _helper._headers = new RequestHelperHeader();
                _helper._headers.Add("sessionId", x.Result.Value);
            }

            return _helper;
        }
    }

    public class TestBazCookieService : IBazCookieService
    {
        public CookieModel MyCookie { get; set; }

        public async Task<CookieModel> GetCookie(bool sessionControl = true)
        {
            await Task.CompletedTask;
            if (MyCookie != null)
            {
                return MyCookie;
            }
            return new CookieModel
            {
                KisiId = "129",
                KurumId = "82",
                Mail = "b@mail.com",
                Name = "Bilal",
                SessionId = ""
            };
        }

        public async Task SetCookie(CookieModel cookieModel)
        {
            await Task.CompletedTask;
            MyCookie = cookieModel;
        }
    }

    public class TestLoginUserManager : ILoginUser
    {
        public TestLoginUserManager()
        {
        }

        /// <summary>
        /// Test için login olan kişlinin default değerleri
        /// </summary>
        public int KurumID { get; set; } = 82;

        /// <summary>
        /// Test için login olan kişlinin default değerleri
        /// </summary>
        public int KisiID { get; set; } = 129;

        public List<int> YetkiliKisiIdleri { get; set; } = new()
        {
            129,
            130,
            210,
            216,
            218,
            236,
            239,
            240,
            242,
            243,
            389,
            392,
            393,
            394,
            395,
            396,
            397,
            398,
            399,
            401,
            403,
            404,
            405,
            406,
            442,
            444,
            445,
            446,
            447,
            488,
            1488,
            1489,
            1490,
            1491,
            1492,
            1493,
            3232,
            3603,
            3942,
            3943,
            4350,
            4450
        };

        public List<int> YetkiliKurumIdleri { get; set; } = new()
        {
            82,
            85,
            2497,
            3448,
            3593,
            3771,
            3792
        };

        public int LisansId { get; set; } = 1031;
    }

    public class TestServerRequestHelperNoHeader
    {
        public static IRequestHelper CreateHelper(TestLoginUserManager manager = null)
        {
            var _server = new TestServer(new WebHostBuilder()
                .UseConfiguration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build()
                )
                .ConfigureTestServices(services =>
                {
                    //if (manager == null)
                    //{
                    //    services.RemoveAll(typeof(ILoginUser));
                    //    services.AddTransient<ILoginUser, TestLoginUserManager>();
                    //}
                })
                .UseStartup<Startup>()

            );
            var _client = _server.CreateClient();
            _client.Timeout = TimeSpan.FromMinutes(15);

            var _helper = new RequestHelper(_client);

            return _helper;
        }
    }
}