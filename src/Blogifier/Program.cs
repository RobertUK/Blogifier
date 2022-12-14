using Blogifier.Core.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Blogifier
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();

            using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var dbContext = services.GetRequiredService<AppDbContext>();

				try
				{
					if (dbContext.Database.GetPendingMigrations().Any())
						dbContext.Database.Migrate();
				}
				catch { }
			}

			host.Run();
        }



		public static IHostBuilder CreateHostBuilder(string[] args) =>
			 Host.CreateDefaultBuilder(args)
				  .ConfigureWebHostDefaults(webBuilder =>
				  {
                      webBuilder
                      ///.UseContentRoot(Directory.GetCurrentDirectory())
                      //.UseKestrel()
					 // .UseIISIntegration()
					  .UseStartup<Startup>();
				  })
             .UseSerilog();
    }
}
