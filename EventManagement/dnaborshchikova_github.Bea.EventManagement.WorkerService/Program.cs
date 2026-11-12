using dnaborshchikova_github.Bea.EventManagement.Core.Interfaces;
using dnaborshchikova_github.Bea.EventManagement.Core.Mappers;
using dnaborshchikova_github.Bea.EventManagement.Core.Services;
using dnaborshchikova_github.Bea.EventManagement.Infrastructure;
using dnaborshchikova_github.Bea.EventManagement.WorkerService;
using dnaborshchikova_github.Bea.EventManagement.WorkerService.Consumers;
using dnaborshchikova_github.Bea.EventManagement.WorkerService.Services;
using dnaborshchikova_github.Bea.EventManagement.Infrastructure.Initializers;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Serilog;
using Serilog.Filters;
using dnaborshchikova_github.Bea.EventManagement.WorkerService.RabbitMq;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<EventConsumer>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddSingleton<ICashRegisterEventMapper, CashRegisterEventMapper>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddSingleton<DlqReprocessingService>();
builder.Services.AddScoped<RabbitMqQueueInitializer>();

builder.Services.AddDbContextFactory<EventManagementDbContext>(options =>
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

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IDatabaseInitializer, DevelopmentDatabaseInitializer>();
}
else
{
    builder.Services.AddScoped<IDatabaseInitializer, ProductionDatabaseInitializer>();
}

builder.Services.AddSingleton<IConnection>((sp =>
{
    var rabbitConfig = config.GetSection("RabbitMq");

    var factory = new ConnectionFactory
    {
        HostName = rabbitConfig["Host"],
        UserName = rabbitConfig["Username"],
        Password = rabbitConfig["Password"]
    };

    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
}));

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .Enrich.FromLogContext()
    .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.Database.Command"))
    .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.Update"))
    .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.ChangeTracking"))
    .Filter.ByExcluding(Matching.FromSource("System.Net.Http.HttpClient"))
    .CreateLogger();
builder.Services.AddSerilog();

var host = builder.Build();

if (!args.Contains("--reprocess-dlq"))
{
    using var scope = host.Services.CreateScope();

    var databaseInitializer = scope.ServiceProvider
        .GetRequiredService<IDatabaseInitializer>();

    databaseInitializer.Initialize();
}

if (args.Contains("--reprocess-dlq"))
{
    try
    {
        var reprocessor = host.Services.GetRequiredService<DlqReprocessingService>();
        var count = await reprocessor.ReprocessAsync(CancellationToken.None);
        Log.Information("DLQ reprocessing completed. Processed: {Count}", count);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "DLQ reprocessing failed.");
        Environment.ExitCode = 1;
    }
    finally
    {
        host.Dispose();
    }
}
else
{
    await host.RunAsync();
}
