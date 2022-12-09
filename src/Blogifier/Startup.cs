using Blogifier.Shared.Configurations;
using Blogifier.Core.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using WebEssentials.AspNetCore.OutputCaching;
using Microsoft.Identity.Web;
using Blogifier.Core.Security;
using Microsoft.Identity.Web.UI;
using WebMarkupMin.AspNetCore6;
using WebMarkupMin.Core;
using WilderMinds.MetaWeblog;


using IWmmLogger = WebMarkupMin.Core.Loggers.ILogger;

using WmmNullLogger = WebMarkupMin.Core.Loggers.NullLogger;
using System;
using Microsoft.Net.Http.Headers;
using Blogifier.Admin;
using Microsoft.AspNetCore.HttpOverrides;

namespace Blogifier
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration()
               .ReadFrom.Configuration(configuration)
               .Enrich.FromLogContext()
               .CreateLogger();

            Log.Warning("Application start");
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Log.Warning("Start configure services");

            services.AddSwaggerGen();

            services.Configure<BlogifierConfiguration>(Configuration.GetSection("Blogifier"));

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                // Only loopback proxies are allowed by default. Clear that restriction because forwarders are
                // being enabled by explicit configuration.
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });            

            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //}).AddCookie();

            services.AddCors(o => o.AddPolicy("BlogifierPolicy", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));

            services.AddBlogDatabase(Configuration);

            services.AddBlogProviders();


            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);


            //services.AddProgressiveWebApp(
            //    new WebEssentials.AspNetCore.Pwa.PwaOptions
            //    {
            //        OfflineRoute = "/shared/offline/"
            //    });

            services.AddOutputCaching(
                options =>
                {
                    options.Profiles["default"] = new OutputCacheProfile
                    {
                        Duration = 1
                    };
                });

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
              .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
                  .EnableTokenAcquisitionToCallDownstreamApi()
                      .AddMicrosoftGraph(Configuration.GetSection("DownstreamApi"))
                      .AddInMemoryTokenCaches();

            services.AddAuthentication()
                      .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"),
                                                  JwtBearerDefaults.AuthenticationScheme)
                      .EnableTokenAcquisitionToCallDownstreamApi();


            services.AddAuthorization(options =>
            {
                options.AddPolicy(AdminAuthorizationPolicy.Name,
                                  AdminAuthorizationPolicy.Build);
            });

            services.AddControllersWithViews().AddMicrosoftIdentityUI();


            // HTML minification (https://github.com/Taritsyn/WebMarkupMin)
            services
                .AddWebMarkupMin(
                    options =>
                    {
                        options.AllowMinificationInDevelopmentEnvironment = true;
                        options.DisablePoweredByHttpHeaders = true;
                    })
                .AddHtmlMinification(
                    options =>
                    {
                        options.MinificationSettings.RemoveOptionalEndTags = false;
                        options.MinificationSettings.WhitespaceMinificationMode = WhitespaceMinificationMode.Safe;
                    });
            services.AddSingleton<IWmmLogger, WmmNullLogger>(); // Used by HTML minifier

            //services.AddWebOptimizer(pipeline =>
            //{
            //    pipeline.MinifyJsFiles();
            //    pipeline.CompileScssFiles()
            //            .InlineImages(1);
            //});



            Log.Warning("Done configure services");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<BlogifierConfiguration> blogifierConfig)
        {

            var pathBase = blogifierConfig.Value.PathBase;
           // app.UseForwardedHeaders();
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("~/Error");
                app.UseStatusCodePagesWithReExecute("~/Error/{0}");
            }
           // app.UseWebOptimizer();

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (context) =>
                {
                    var time = TimeSpan.FromDays(365);
                    context.Context.Response.Headers[HeaderNames.CacheControl] = $"max-age={time.TotalSeconds.ToString()}";
                    context.Context.Response.Headers[HeaderNames.Expires] = DateTime.UtcNow.Add(time).ToString("R");
                }
            });

           // app.UseOutputCaching();
          // app.UseWebMarkupMin();

            app.UseBlazorFrameworkFiles();


            app.UseStatusCodePagesWithReExecute("/Shared/Error");

            app.UseRouting();
            app.UseCors("BlogifierPolicy");

           // app.UseMetaWeblog("/metaweblog");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                      name: "default",
                      pattern: "{controller=Home}/{action=Index}/{id?}"
                 );
                endpoints.MapRazorPages();
                endpoints.MapFallbackToFile("admin/{*path:nonfile}", "index.html");
                endpoints.MapFallbackToFile("account/{*path:nonfile}", "index.html");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blazor API V1");
            });


        }
    }
}
