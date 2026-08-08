using dnaborshchikova_github.Bea.EventManagement.Core.Interfaces;
using dnaborshchikova_github.Bea.EventManagement.Core.Mappers;
using dnaborshchikova_github.Bea.EventManagement.Core.Services;
using dnaborshchikova_github.Bea.EventManagement.Infrastructure;
using dnaborshchikova_github.Bea.EventManagement.WorkerService;
using dnaborshchikova_github.Bea.EventManagement.WorkerService.Consumers;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Serilog;
using Serilog.Filters;

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
host.Run();
