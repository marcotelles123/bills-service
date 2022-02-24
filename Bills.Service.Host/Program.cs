using Bills.Service.CheckReceipts.Domain;
using Bills.Service.Email.Infra;
using Bills.Service.RefreshPaids.Domain;
using Bills.Service.RefreshPaids.Infra;
using Bills.Service.SendEmail.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Text.Json;

namespace Bills.Service.Host
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} ({ThreadID}) [{Level}] {Message}{NewLine}{Exception}", path: @"D:\projects\bills-service\Bills.Service.Host\publish\logs\bills-service-logs.txt", restrictedToMinimumLevel: LogEventLevel.Debug, rollingInterval: RollingInterval.Month)
                    .CreateLogger();

            var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    // Build configuration
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                        .AddJsonFile("appsettings.json", false)
                        .Build();

                    services.AddHostedService<Worker>()
                    .AddSingleton<IConfigurationRoot>(configuration)
                    .AddSingleton((ILogger)Log.Logger)
                    .AddSingleton<IRefreshPaidsRepository, RefreshPaidsRepositoryMongo>()
                    .AddSingleton<IEmailSMTP, EmailGmail>()
                    .AddSingleton<ISendEmailDueBill, SendEmailDueBillImpl>()
                    .AddSingleton<ICheckReceipts, CheckReceiptsImpl>()
                    .AddSingleton<IRefreshPaids, RefreshPaidsImpl>();
                });

            return hostBuilder;
        }
    }
}
