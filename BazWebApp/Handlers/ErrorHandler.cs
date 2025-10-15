using Baz.Model.Entity;
using BazWebApp.Services;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Handlers
{
    /// <summary>
    /// Hataların yakalanması ve sayfalarının yönetilmesi işlemlerini gerçekleştiren Handler class'ı
    /// </summary>
    public class ErrorHandler
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// Hataların yakalanması ve sayfalarının yönetilmesi işlemlerini gerçekleştiren Handler class'ın yapıcı metodu
        /// </summary>
        /// <param name="next"></param>
        public ErrorHandler(RequestDelegate next)
        {

            _next = next;
        }
        /// <summary>
        /// Handler işlemlerinin gerçekleştiği task
        /// </summary>
        /// <param name="context">HttpContext türünde context nesnesi</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {

            var logger = context.RequestServices.GetService<Microsoft.Extensions.Logging.ILogger<ErrorHandler>>();

            try
            {
                await _next(context);
                if (context.Response.StatusCode != 200)
                    logger.LogError("Error {0} {1}", context.Response.StatusCode, context.Request.Host.Value);
                //handle HTTP codes
                switch (context.Response.StatusCode)
                {
                    case 404:
                        HandlePageNotFound(context);
                        break;

                    case 418:
                        //Not implemented, put here as an example
                        break;

                    default:
                        break;
                }

            }
            catch (Exception e)
            {
                logger.LogError("Error {0} {1}", context.Response.StatusCode, context.Request.Host.Value);
                //Handle uncaught global exceptions as 500 Error
                HandleException(context);
            }
        }

        /// <summary>
        /// Exception oluşması durumunda çağırılan method, 500 hata sayfasına yönlendirme sağlar.
        /// </summary>
        /// <param name="context">HttpContext nesnesi</param>
        private static void HandleException(HttpContext context)
        {
            var cookieService = context.RequestServices.GetService<IBazCookieService>();
            var cookie = cookieService.GetCookie().GetAwaiter().GetResult();
            if (cookie == null)
                context.Response.Redirect("/?returnurl=" + context.Request.Path);
            else
                context.Response.Redirect("/error/500");
        }
        /// <summary>
        /// Sayfa bulunamadı hatalarında çağırılan method, 404 hata sayfasına yönlendirme sağlar.
        /// </summary>
        /// <param name="context">HttpContext nesnesi</param>
        private static void HandlePageNotFound(HttpContext context)
        {
            context.Response.Redirect("/Error/404");
        }
    }
}
