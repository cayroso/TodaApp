using Data.App.DbContext;
using Data.Identity.DbContext;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Web.Provides;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var env = host.Services.GetService<IWebHostEnvironment>();
                var services = scope.ServiceProvider;

                try
                {
                    var ctx1 = services.GetRequiredService<IdentityWebContext>();
                    ctx1.Database.Migrate();
                    IdentityWebContextInitializer.Initialize(ctx1);

                    var ctx2 = services.GetRequiredService<AppDbContext>();
                    ctx2.Database.Migrate();
                    AppDbContextInitializer.Initialize(ctx1, ctx2, null);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                    throw;
                }

            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(ConfigConfiguration)
                .ConfigureLogging((ctx, builder) =>
                {

                    var config = new DBLoggerConfiguration
                    {
                        ConnectionString = ctx.Configuration.GetConnectionString($"LoggerDbContextConnectionSQLite"),
                        LogLevel = LogLevel.Error
                    };

                    builder.AddProvider(new DBLoggerProvider(config));
                })
                .UseStartup<Startup>();

        static void ConfigConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder config)
        {
            //ctx.HostingEnvironment.EnvironmentName = EnvironmentName.Production;

            config.SetBasePath(ctx.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                ;

        }
    }
}
