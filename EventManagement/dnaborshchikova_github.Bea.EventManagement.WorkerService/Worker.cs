using dnaborshchikova_github.Bea.EventManagement.WorkerService.Consumers;

namespace dnaborshchikova_github.Bea.EventManagement.WorkerService
{
    public class Worker(ILogger<Worker> logger, EventConsumer eventConsumer) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

            await eventConsumer.StartAsync(stoppingToken);
        }
    }
}
