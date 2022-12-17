using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using Sotsera.Blazor.Toaster.Core.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blogifier.Admin
{
	public class Program
	{
		public static async Task Main(string[] args)
		{



            var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("#app");
            
			builder.Services.AddLocalization();

			builder.Services.AddOptions();
			builder.Services.AddAuthorizationCore();

			builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress ) });

			builder.Services.AddScoped<AuthenticationStateProvider, BlogAuthenticationStateProvider>();

			builder.Services.AddToaster(config =>
			{
				config.PositionClass = Defaults.Classes.Position.BottomRight;
				config.PreventDuplicates = true;
				config.NewestOnTop = false;
			});

            builder.Services.AddSingleton<BlogStateProvider>();

            //var levelSwitch = new LoggingLevelSwitch();
            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.ControlledBy(levelSwitch)
            //    .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
            //    .WriteTo.Sink(Serilog.Events.LogEventLevel.Verbose)
            //    .CreateLogger();
           // builder.Logging.AddProvider(new SerilogLoggerProvider());

            builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true).SetMinimumLevel(LogLevel.Trace));

            await builder.Build().RunAsync();
		}
	}
}
