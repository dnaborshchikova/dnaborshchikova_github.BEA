using dnaborshchikova_github.Bea.Collector.Common;
using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.Core.Services;
using dnaborshchikova_github.Bea.Collector.DataAccess;
using dnaborshchikova_github.Bea.Collector.DataAccess.Initializers;
using dnaborshchikova_github.Bea.Collector.DataAccess.Initializers.Interfaces;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories;
using dnaborshchikova_github.Bea.Collector.DataAccess.Repositories.Interfaces;
using dnaborshchikova_github.Bea.Collector.Parser.Handlers;
using dnaborshchikova_github.Bea.Collector.Processor.Handlers;
using dnaborshchikova_github.Bea.Collector.Processor.Processors;
using dnaborshchikova_github.Bea.Collector.Processor.Services;
using dnaborshchikova_github.Bea.Collector.Sender.Handlers;
using dnaborshchikova_github.Bea.Collector.WorkerService.Models;
using dnaborshchikova_github.Bea.Collector.WorkerService.Services;
using dnaborshchikova_github.Bea.Collector.WorkerService.Validators;
using dnaborshchikova_github.Bea.Generator;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// Чтение настроек из конфигурации
var generatorSettingsSection = config.GetSection(nameof(GeneratorSettings));
config.GetRequired<GeneratorSettings>("GeneratorSettings", "PaidBillEventCount", "CancelledBillEventCount");
var generatorSettings = generatorSettingsSection.Get<GeneratorSettings>();

var processingSettingsSection = config.GetSection(nameof(ProcessingSettings));
config.GetRequired<ProcessingSettings>("ProcessingSettings", "ThreadCount");
var processingSettings = processingSettingsSection.Get<ProcessingSettings>();

var workerServiceSettingsSection = config.GetSection(nameof(WorkerServiceSettings));
config.GetRequired<WorkerServiceSettings>("WorkerServiceSettings", "IntervalHours");
var workerServiceSettings = workerServiceSettingsSection.Get<WorkerServiceSettings>();

var workerSettingsValidator = new WorkerSettingsValidator();
workerSettingsValidator.Validate(generatorSettings, processingSettings, workerServiceSettings);

var appSettingsService = new AppSettingsService();
var appSettings = appSettingsService.CreateAppSettings(generatorSettings, processingSettings);

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.Database.Command"))
    .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.Update"))
    .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.ChangeTracking"))
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Регистрация зависимостей
        services.AddSingleton(appSettings);
        services.AddSingleton(generatorSettings);

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            services.AddScoped<IDatabaseInitializer, DevelopmentDatabaseInitializer>();
        }
        else
        {
            services.AddScoped<IDatabaseInitializer, ProductionDatabaseInitializer>();
        }

        services.AddScoped<ThreadProcessor>();
        services.AddScoped<TaskProcessor>();

        services.AddScoped<Func<string, IProcessor>>(provider => key =>
        {
            return key switch
            {
                "Thread" => provider.GetRequiredService<ThreadProcessor>(),
                "Task" => provider.GetRequiredService<TaskProcessor>(),
                "ThreadProcessorWithLock" => provider.GetRequiredService<ThreadProcessorWithLock>()
            };
        });

        services.AddScoped<IEventSender, DataBaseSender>();
        services.AddScoped<IParser, CsvParser>();
        services.AddScoped<IEventProcessor, EventProcessorService>();
        services.AddScoped<ISendEventLogRepository, SendEventLogRepository>();
        services.AddScoped<IFileSelectionStrategy, WorkerFileSelectionStrategy>();
        services.AddScoped<AppRunner>();

        // Настройка подключения к базе данных
        services.AddDbContextFactory<CollectorDbContext>(options =>
        {
            options.UseNpgsql(config.GetConnectionString("Default"),
            npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null
                );
            });
        });

        // Условия для разных режимов работы
        if (appSettings.ProcessingSettings.RunMode == "OneTime")
            services.AddHostedService<SingleRunHostedService>();
        if (appSettings.ProcessingSettings.RunMode == "ScheduledService")
            services.AddHostedService<PeriodicHostedService>();
    })
    .UseSerilog()
    .Build();

using (var scope = host.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    databaseInitializer.Initialize();
}

var appRunnerScope = host.Services.CreateScope();
var appRunner = appRunnerScope.ServiceProvider.GetRequiredService<AppRunner>();
var basePath = Path.Combine(AppContext.BaseDirectory, appSettings.ProcessingSettings.InputFolder);
appRunner.Generate(basePath);

host.Run();
