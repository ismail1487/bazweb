using Baz.Model.Entity.Constants;
using Baz.Model.Pattern;
using Baz.RequestManager;
using Baz.RequestManager.Abstracts;
using Baz.SharedSession;
using BazWebApp.Handlers;
using BazWebApp.Helpers;
using BazWebApp.Services;
using Elasticsearch.Net;
using Ganss.XSS;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WebEssentials.AspNetCore.Pwa;
using Baz.AOP.Logger.ExceptionLog;
using BazWebApp.Controllers;

namespace BazWebApp
{
    /// <summary>
    /// Web application baþlangýcýnda tetiklenen metod
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Startup
    {
        /// <summary>
        /// Web application baþlangýcýnda tetiklenen metod konst.
        /// </summary>
        /// <param name="env"></param>
        public Startup(IWebHostEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables().Build();
        }

        /// <summary>
        /// Web app. ayarlarý için tetiklenen interface
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// Web application ayarlarý için tetiklenen metod
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            SetCoreURL(Configuration);

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddHttpContextAccessor();//IHttpContextAccessor servisi oluþur, API'ýn Mevcut sesionId ye Eriþimini saðlar (HTTPCONTEXT)
            services.AddDistributedSqlServerCache(p =>
            {
                p.ConnectionString = Configuration.GetConnectionString("SessionConnection");
                p.SchemaName = "dbo";
                p.TableName = "SQLSessions";
            });//DistributedCache Servisi oluþuyor, SQLSessions Konfigürasyonu
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.Path = "/";
                options.Cookie.Name = "Test.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });//ISession SessionId nin saklanacaðý Test.Session'ý oluþturur (kullancýý 60 sn boyunca etkileþime geçmezse oturum sonlanýr!)
            services.AddSession();
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt => { opt.LoginPath = "/"; opt.AccessDeniedPath = "/"; opt.LogoutPath = "/"; })
    .AddFacebook(face =>
            {
                face.AppId = "322550989515570";
                face.AppSecret = "c7d97661d444e943f103441a82b231bc";
            })
    .AddGoogle(g =>
    {
        g.ClientId = "187645571727-pncubjv8gop85rrufq37scdln0rb5muk.apps.googleusercontent.com";
        g.ClientSecret = "bjZK3KId4vWszBE8RU-pP9Bh";
    });
            services.AddSingleton<IHtmlSanitizer>(_ => new HtmlSanitizer());
            services.AddSingleton<ISharedSession, BaseSharedSession>();
            //services.AddSingleton<ISocketService, SocketService>();

            services.AddScoped<IBazCookieService, BazCookieService>();
            services.AddScoped<IBazAuthorizationService, BazAuthorizationService>();
            services.AddTransient<IRequestHelper, RequestHelper>(provider =>
            {
                return new RequestHelper("", new RequestManagerHeaderHelperForCookie(provider).SetDefaultHeader());
            });
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IDilService, DilService>();
            services.AddScoped<IKurumService, KurumService>();
            services.AddScoped<IKurumIliskiService, KurumIliskiService>();
            services.AddScoped<ILoginRegisterService, LoginRegisterService>();
            services.AddScoped<IKisiService, KisiService>();
            services.AddScoped<IKisiIliskiService, KisiIliskiService>();
            services.AddScoped<IMedyaKutuphanesiService, MedyaKutuphanesiService>();
            services.AddScoped<IParamOrganizasyonBirimleriService, ParamOrganizasyonBirimleriService>();
            services.AddScoped<IKurumOrganizasyonBirimTanimlariService, KurumOrganizasyonBirimTanimlariService>();
            services.AddScoped<IPostaciIslemlerService, PostaciIslemlerService>();
            services.AddScoped<ISistemMenuTanimlariAyrintilar, SistemMenuTanimlariAyrintilar>();
            services.AddScoped<IGenelParametrelerService, GenelParametrelerService>();
            services.AddScoped<IParamService, ParamService>();
            services.AddScoped<IYetkiMerkeziService, YetkiMerkeziService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ISmtpService, SmtpService>();
            services.AddScoped<ICografyaService, CografyaService>();
            services.AddScoped<IParamDisPlatformlarService, ParamDisPlatformlarService>();

            services.AddScoped<ITargetGroupService, TargetGroupService>();

            services.AddScoped<IIYSService, IYSService>();

            services.AddScoped<IModulService, ModulService>();
            services.AddTransient<ISearchService, SearchService>();

            services.AddTransient<IExcelReadWriteHandler, ExcelReadWriteHandler>();

            services.AddTransient<IKurumMesajlasmaModuluIzinIslemleriService, KurumMesajlasmaModuluIzinIslemleriService>();
            services.AddTransient<ILoginUser, LoginUserManager>();

            services.AddLocalization(apt => apt.ResourcesPath = "Resources");
            services.AddMvc(config =>
                {
                    config.Filters.Add(typeof(ModelValidationFilter));
                })
                .AddDataAnnotationsLocalization()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            //***** Elastic Search calismasi icin connection ayarlari ve client init islemi *****//
            //var pool = new SingleNodeConnectionPool(new Uri(Configuration.GetValue<string>("ElasticSearch")));
            //var settings = new ConnectionSettings(pool).DefaultIndex("digiforce"); //digiforce
            //settings.BasicAuthentication("elastic", "123456aA!"); //user verileri buraya eklenecek.
            //settings.DisableDirectStreaming();
            //var client = new ElasticClient(settings);
            //services.AddSingleton(client);

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(
                        /*
                        LocalPortlar.WebApp,
                        LocalPortlar.SocketServer,
                        Configuration.GetValue<string>("WebAppLive"),
                        Configuration.GetValue<string>("WebApp77"),
                        Configuration.GetValue<string>("SocketLive"),
                        Configuration.GetValue<string>("SocketLocal"),
                        Configuration.GetValue<string>("SocketIP"),
                        Configuration.GetValue<string>("SocketIPHttps"),
                        Configuration.GetValue<string>("Socket77"),
                        Configuration.GetValue<string>("MedyaKutuphanesiLive"),
                        Configuration.GetValue<string>("MedyaKutuphanesiLocal"),
                        Configuration.GetValue<string>("WebAppJitsi"),
                        Configuration.GetValue<string>("WebAppHttpBind"),
                        Configuration.GetValue<string>("JitsiMeetBeta"),
                        Configuration.GetValue<string>("UploadChunk"),
                        Configuration.GetValue<string>("MedyaIP") */
                        )
                           .AllowAnyMethod()
                           .SetIsOriginAllowed((x) => true)
                           .AllowCredentials();
                });
            });

            services.AddProgressiveWebApp(new PwaOptions()
            {
                CacheId = "V_12",
                Strategy = ServiceWorkerStrategy.CacheFirstSafe,
                RegisterServiceWorker = true,
                RegisterWebmanifest = true
            });

            //services.AddLettuceEncrypt();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Web App. ayarlarý için tetiklenen metod
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="lifetime"></param>
        /// <param name="cache"></param>
        /// <param name="bazCookieService"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime,
            IDistributedCache cache, IBazCookieService bazCookieService)
        {

            // Configure the Localization middleware
            app.UseRequestLocalization();

            app.UseCors();
            app.UseSession();//  Her API vs. isteðine sessionId ekler |HttpContext.Session nesnesine sessionId'yi yükler

            lifetime.ApplicationStarted.Register(() =>
            {
                var currentTimeUTC = DateTime.UtcNow.ToString();
                byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(currentTimeUTC);
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(20));
                cache.Set("cachedTimeUTC", encodedCurrentTimeUTC, options);
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            var supportedCultures = new List<CultureInfo>
            {
            new CultureInfo("tr-TR"),
            new CultureInfo("en-US"),
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                DefaultRequestCulture = new RequestCulture("tr-TR")
            });
            app.UseMiddleware<ExceptionHandlerMiddlewareNotRepository>();
            app.UseMiddleware<ErrorHandler>();

            app.UseRouting();
            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                DefaultContentType = "text/plain"
            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<YetkiKontrolMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void SetCoreURL(IConfiguration configuration)
        {
            LocalPortlar.CoreUrl = configuration.GetValue<string>("CoreUrl");

            var section = configuration.GetSection("LocalPortlar");

            //LocalPortlar.WebApp = section.GetValue<string>("WebApp");
            LocalPortlar.UserLoginregisterService = section.GetValue<string>("UserLoginregisterService");
            LocalPortlar.KisiServis = section.GetValue<string>("KisiServis");
            LocalPortlar.MedyaKutuphanesiService = section.GetValue<string>("MedyaKutuphanesiService");
            LocalPortlar.IYSService = section.GetValue<string>("IYSService");
            LocalPortlar.KurumService = section.GetValue<string>("KurumService");
        }
    }
}