using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Serilog;
using Sotsera.Blazor.Toaster.Core.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog.Core;

namespace Blogifier.Admin
{
	public class Program
	{
		public static async Task Main(string[] args)

            


		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);

            IConfiguration Configuration;

            Configuration = builder.Configuration;

            Log.Logger = (ILogger)new LoggerConfiguration()
                  .Enrich.FromLogContext()
                  //.WriteTo.Console();
                  .WriteTo.File("Logs/log-admin.txt", rollingInterval: RollingInterval.Day)
                  .WriteTo.Console()
                  .CreateLogger();

            Log.Warning("Application start");



            builder.RootComponents.Add<App>("#app");

			builder.Services.AddLocalization();

			builder.Services.AddOptions();
			builder.Services.AddAuthorizationCore();

            Uri x = new Uri(builder.HostEnvironment.BaseAddress + "robert");
            Log.Warning(x.OriginalString);

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "robert") });

			builder.Services.AddScoped<AuthenticationStateProvider, BlogAuthenticationStateProvider>();

			builder.Services.AddToaster(config =>
			{
				config.PositionClass = Defaults.Classes.Position.BottomRight;
				config.PreventDuplicates = true;
				config.NewestOnTop = false;
			});

            builder.Services.AddSingleton<BlogStateProvider>();

			await builder.Build().RunAsync();
		}

       
    }
}
