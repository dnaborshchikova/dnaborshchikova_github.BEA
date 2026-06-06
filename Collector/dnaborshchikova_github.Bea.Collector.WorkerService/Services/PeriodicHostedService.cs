using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.WorkerService.Models;

namespace dnaborshchikova_github.Bea.Collector.WorkerService.Services
{
    public class PeriodicHostedService : BackgroundService
    {
        private readonly WorkerServiceSettings _serviceSettings;
        private readonly IServiceProvider _serviceProvider;

        public PeriodicHostedService(WorkerServiceSettings serviceSettings
            , IServiceProvider serviceProvider)
        {
            _serviceSettings = serviceSettings;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope(); 
            var eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();

            var interval = TimeSpan.FromHours(_serviceSettings.IntervalHours);
            while (!cancellationToken.IsCancellationRequested)
            {
                await eventProcessor.ProcessAsync();
                await Task.Delay(interval, cancellationToken);
            }
        }
    }
}
