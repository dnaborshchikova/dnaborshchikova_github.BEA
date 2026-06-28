using dnaborshchikova_github.Bea.Collector.Core.Interfaces;

namespace dnaborshchikova_github.Bea.Collector.WorkerService.Services
{
    public class SingleRunHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _appLifetime;

        public SingleRunHostedService(IServiceProvider serviceProvider
            , IHostApplicationLifetime appLifetime)
        {
            _serviceProvider = serviceProvider;
            _appLifetime = appLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();
                await processor.ProcessAsync();
            }
            finally
            {
                _appLifetime.StopApplication();
            }
        }
    }
}
