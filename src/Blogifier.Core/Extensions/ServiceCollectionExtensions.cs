using Blogifier.Core.Data;
using Blogifier.Core.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using Microsoft.Extensions.Logging;
using Blogifier.Shared.Configurations;
using Microsoft.Extensions.Options;

namespace Blogifier.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {

        private static BlogifierConfiguration _blogifierConfiguration;

        public static IServiceCollection AddBlogDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("Blogifier");
            var conn = section.GetValue<string>("ConnString");
            var dbDebug = section.GetValue<bool>("DbDebug");
            //  _blogifierConfiguration =configuration.GetSection(IOptions<BlogifierConfiguration>)

            if (section.GetValue<string>("DbProvider") == "SQLite")
                services.AddDbContext<AppDbContext>(o =>
                {
                    o.UseSqlite(conn);

                    if (dbDebug)
                    {
                        o.EnableDetailedErrors();
                        o.EnableSensitiveDataLogging();
                        o.ConfigureWarnings(c => c.Log((RelationalEventId.CommandExecuting, LogLevel.Warning)));
                    }

                });

            if (section.GetValue<string>("DbProvider") == "SqlServer")
                services.AddDbContext<AppDbContext>(o => o.UseSqlServer(conn));

            if (section.GetValue<string>("DbProvider") == "Postgres")
                services.AddDbContext<AppDbContext>(o => o.UseNpgsql(conn));

            //TODO: this is not tested
            if (section.GetValue<string>("DbProvider") == "MySql")
            {
                services.AddDbContextPool<AppDbContext>(
                    dbContextOptions => dbContextOptions.UseMySql(
                        section.GetValue<string>("ConnString"),
                        new MySqlServerVersion(new Version(8, 0, 21))
                    //mySqlOptions => mySqlOptions.HasCharSet("utf8mb4", DelegationModes.ApplyToAll) //CharSetBehavior(CharSetBehavior.NeverAppend)
                    )
                ) ;
            }
            services.AddDatabaseDeveloperPageExceptionFilter();
            return services;
        }

        public static IServiceCollection AddBlogProviders(this IServiceCollection services)
        {
            services.AddScoped<IAuthorProvider, AuthorProvider>();
            services.AddScoped<IBlogProvider, BlogProvider>();
            services.AddScoped<IPostProvider, PostProvider>();
            services.AddScoped<IStorageProvider, StorageProvider>();
            services.AddScoped<IFeedProvider, FeedProvider>();
            services.AddScoped<ICategoryProvider, CategoryProvider>();
            services.AddScoped<IAnalyticsProvider, AnalyticsProvider>();
            services.AddScoped<INewsletterProvider, NewsletterProvider>();
            services.AddScoped<IEmailProvider, MailKitProvider>();
            services.AddScoped<IThemeProvider, ThemeProvider>();
            services.AddScoped<ISyndicationProvider, SyndicationProvider>();
            services.AddScoped<IAboutProvider, AboutProvider>();


            return services;
        }
    }
}
