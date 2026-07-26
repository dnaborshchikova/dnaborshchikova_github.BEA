using dnaborshchikova_github.Bea.EventManagement.Core.Interfaces;
using dnaborshchikova_github.Bea.EventManagement.Core.Mappers;
using dnaborshchikova_github.Bea.EventManagement.Core.Services;
using dnaborshchikova_github.Bea.EventManagement.Infrastructure;
using dnaborshchikova_github.Bea.EventManagement.WorkerService;
using dnaborshchikova_github.Bea.EventManagement.WorkerService.Consumers;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

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
    var factory = new ConnectionFactory
    {
        HostName = "localhost",
        UserName = "guest",
        Password = "guest"
    };

    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
}));

var host = builder.Build();
host.Run();
