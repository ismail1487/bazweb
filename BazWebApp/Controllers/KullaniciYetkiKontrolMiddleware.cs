using Baz.Model.Entity.ViewModel;
using Baz.SharedSession;
using BazWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Controllers
{
    public class YetkiKontrolMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        //private readonly IBazCookieService _bazCookieService;
        //private readonly ISharedSession _sharedSession;
        //private readonly IKurumService _kurumService;
        //private readonly List<string> _sayfaListesi;

        public YetkiKontrolMiddleware(RequestDelegate next, IMemoryCache cache/* IKurumService kurumService, IBazCookieService bazCookieService, ISharedSession sharedSession*/)
        {
            _next = next;
            _cache = cache;
            //_bazCookieService = bazCookieService;
            //_sharedSession = sharedSession;
            //_kurumService = kurumService;
            //_sayfaListesi = _kurumService.SistemSayfalariGetir().Value.Select(item => item.SayfaUrl).ToList();
        }

        public async Task Invoke(HttpContext context, IServiceScopeFactory scopeFactory)
        {

            using (var scope = scopeFactory.CreateScope())
            {
                var kurumService = scope.ServiceProvider.GetRequiredService<IKurumService>();
                var bazCookieService = scope.ServiceProvider.GetRequiredService<IBazCookieService>();
                var sharedSession = scope.ServiceProvider.GetRequiredService<ISharedSession>();

                // **Cache mekanizması**: sayfa listesini cache’den al, eğer yoksa getir
                if (!_cache.TryGetValue("SayfaListesi", out List<string> sayfaListesi))
                {
                    sayfaListesi = kurumService.SistemSayfalariGetir().Value.Select(item => item.SayfaUrl).ToList();
                    _cache.Set("SayfaListesi", sayfaListesi, TimeSpan.FromMinutes(10)); // 10 dakika cache’de tut
                }

                var currentPath = context.Request.Path;

                var endpoint = context.GetEndpoint();
                if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
                {
                    await _next(context);
                    return;
                }

                if (sayfaListesi.Contains(currentPath))
                {
                    var cookie = await bazCookieService.GetCookie();
                    var session = await sharedSession.Get<KullaniciSession>(cookie.SessionId);

                    // Kullanıcı yetkilerini cache’den getir (eğer daha önce alınmışsa)
                    string cacheKey = $"Yetkiler_{cookie.SessionId}";
                    if (!_cache.TryGetValue(cacheKey, out List<string> yetkiListesi))
                    {
                        yetkiListesi = session.KullaniciYetkiListesi;
                        _cache.Set(cacheKey, yetkiListesi, TimeSpan.FromMinutes(10));
                    }

                    if (yetkiListesi.Contains(currentPath))
                    {
                        await _next(context);
                    }
                    else
                    {
                        context.Response.StatusCode = 403;
                        context.Response.Redirect("/Error/403");
                    }
                }
                else
                {
                    await _next(context);
                }
            }
        }
    }
}