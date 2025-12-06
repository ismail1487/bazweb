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
                
                //handle HTTP codes - sadece gerçek hata kodları
                switch (context.Response.StatusCode)
                {
                    case 404:
                        logger.LogWarning("Page not found: {0} {1}", context.Response.StatusCode, context.Request.Path);
                        HandlePageNotFound(context);
                        break;

                    case 500:
                    case 501:
                    case 502:
                    case 503:
                    case 504:
                        logger.LogError("Server error: {0} {1}", context.Response.StatusCode, context.Request.Path);
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
                logger.LogError(e, "Unhandled exception occurred. Path: {0}, Host: {1}", context.Request.Path, context.Request.Host.Value);
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
