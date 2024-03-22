using Darkside.LeasingCalc.Core.Configuration;
using Darkside.LeasingCalc.Core.Repositories;
using Darkside.LeasingCalc.Core.Service;
using Darkside.LeasingCalc.Core.Validation;
using Darkside.LeasingCalc.Data.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, configuration) =>
    {
        configuration.SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", true, reloadOnChange: true);

    })
    .ConfigureServices((hostContext, serviceCollection) =>
    {
        var settings = new Settings(serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>());
        serviceCollection.AddTransient<ILeaseCalculatorService, LeaseMilageCalculatorService>();
        serviceCollection.AddTransient<ILeaseCalculatorRepository, LeaseCalculatorRepository>();
        serviceCollection.AddTransient<ICarLeaseRepository, CarLeaseRepository>();
        serviceCollection.AddTransient<IValidationService, ValidationService>();
        serviceCollection.AddApplicationInsightsTelemetryWorkerService();
        serviceCollection.ConfigureFunctionsApplicationInsights();
        serviceCollection.AddDbContext<DbContext, DarksideLeasingCalcDbContext>(options =>
        {
            options.UseSqlServer(settings.SqlDbConnectionString);
        }, ServiceLifetime.Transient, ServiceLifetime.Transient);
        serviceCollection.Configure<KestrelServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });

    })
    .Build();

host.Run();
