using dnaborshchikova_github.Bea.EventManagement.Core.Interfaces;
using dnaborshchikova_github.Bea.EventManagement.Core.Services;
using dnaborshchikova_github.Bea.EventManagement.Infrastructure;
using dnaborshchikova_github.Bea.EventManagement.Infrastructure.Initializers;
using dnaborshchikova_github.Bea.EventManagement.WebApi.Handlers;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;
using System.Reflection;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, services, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration)
      .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.Database.Command"))
      .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.Update"))
      .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore.ChangeTracking"));
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<EventBatchHandler>();
builder.Services.AddScoped<IEventRepository, EventRepository>();

builder.Services.AddDbContext<EventManagementDbContext>(options =>
    options.UseNpgsql(config.GetConnectionString("Default")));
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    builder.Services.AddScoped<IDatabaseInitializer, DevelopmentDatabaseInitializer>();
}
else
{
    builder.Services.AddScoped<IDatabaseInitializer, ProductionDatabaseInitializer>();
}

var app = builder.Build();

// Инициализация БД до запуска приложения.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EventManagementDbContext>();
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    databaseInitializer.Initialize();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();