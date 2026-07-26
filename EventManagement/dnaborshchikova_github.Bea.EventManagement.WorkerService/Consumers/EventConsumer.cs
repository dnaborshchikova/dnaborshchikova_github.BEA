using dnaborshchikova_github.Bea.EventManagement.Core.Mappers;
using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using dnaborshchikova_github.Bea.EventManagement.Core.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace dnaborshchikova_github.Bea.EventManagement.WorkerService.Consumers
{
    public class EventConsumer
    {
        private readonly IConnection _connection;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<EventConsumer> _logger;

        public EventConsumer(IConnection connection, IServiceScopeFactory serviceScopeFactory
            , ILogger<EventConsumer> logger)
        {
            _connection = connection;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken token)
        {
            var channel = await _connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "send-events", durable: true, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var cashRegisterEventDto = JsonSerializer.Deserialize<CashRegisterEventDto>(message);

                var cashRegisterEventMapper = _serviceScopeFactory
                    .CreateScope().ServiceProvider.GetRequiredService<ICashRegisterEventMapper>();
                var cashRegisterEvent = cashRegisterEventMapper.ToDomain(cashRegisterEventDto);

                var eventService = _serviceScopeFactory
                    .CreateScope().ServiceProvider.GetRequiredService<IEventService>();
                await eventService.SaveEventAsync(cashRegisterEvent);

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync(queue: "send-events", autoAck: false, consumer: consumer);

            await Task.Delay(Timeout.Infinite, token);
        }
    }
}
