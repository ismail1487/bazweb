using System;
using BazWebApp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Baz.AOP.Logger.ExceptionLog;

namespace WebAppUnitTests.Helper
{
    /// <summary>
    /// Socket  Test için  kullanılan helper
    /// </summary>
    /// <returns></returns>
    public static class TestHubHelper
    {
        /// <summary>
        /// Socket  Test için RequestHelper oluşturma
        /// </summary>
        /// <returns></returns>
        public static HubConnection CreateHelper(string url)
        {
            var _server = new TestServer(new WebHostBuilder()
                .UseConfiguration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.Development.json")
                    .Build()
                )
                .UseStartup<Startup>()
            );
           var config= _server.Services.GetService<IConfiguration>();
            if (config == null)
            {
                throw new OctapullException(OctapullExceptions.ServiceProviderError);
            }
            var connection = new HubConnectionBuilder()
                .WithUrl(
                    config.GetValue<string>("SocketLive") +"/" + url,
                    o => o.HttpMessageHandlerFactory = _ => _server.CreateHandler())
                .Build();

            connection.HandshakeTimeout = TimeSpan.FromHours(1);
            connection.ServerTimeout = TimeSpan.FromHours(2);
            connection.KeepAliveInterval = TimeSpan.FromHours(1);

            return connection;
        }
    }
}