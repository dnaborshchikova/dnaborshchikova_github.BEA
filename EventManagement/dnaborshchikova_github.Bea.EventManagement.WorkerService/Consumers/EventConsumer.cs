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
        private const int MaxRetryCount = 3;
        private const string RetryCountHeader = "x-retry-count";

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

            await CreateQueue(channel);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                    var cashRegisterEventDto = JsonSerializer.Deserialize<CashRegisterEventDto>(message);

                    using var scope = _serviceScopeFactory.CreateScope();
                    var cashRegisterEventMapper = scope.ServiceProvider.GetRequiredService<ICashRegisterEventMapper>();
                    var cashRegisterEvent = cashRegisterEventMapper.ToDomain(cashRegisterEventDto);

                    var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
                    await eventService.SaveEventAsync(cashRegisterEvent);
                    _logger.LogInformation("Event saved successfully. Id={Id}", cashRegisterEvent.Id);
                    await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                    _logger.LogInformation("RabbitMQ message acknowledged. Id={Id}", cashRegisterEvent.Id);
                }
                catch (DuplicateEventException)
                {
                    //_logger.LogInformation("Event is already been added. Id={Id}", cashRegisterEvent.Id);
                    await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    var retryCount = GetRetryCount(eventArgs);
                    _logger.LogError(ex, "Failed to process message. Retry attempt: {RetryCount}/{MaxRetryCount}", retryCount, MaxRetryCount);
                    if (retryCount < MaxRetryCount)
                    {
                        await RetryMessageAsync(channel, eventArgs, retryCount + 1);
                    }
                    else
                    {
                        _logger.LogError(
                            "Max retry count reached. Sending message to DLQ.");

                        await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
                    }
                }
            };

            await channel.BasicConsumeAsync(queue: "send-events", autoAck: false, consumer: consumer);

            _logger.LogInformation("RabbitMQ consumer started.");

            await Task.Delay(Timeout.Infinite, token);
        }

        private async Task CreateQueue(IChannel channel)
        {
            await channel.ExchangeDeclareAsync(exchange: "send-events.dlx", type: ExchangeType.Direct, durable: true, autoDelete: false);
            await channel.QueueDeclareAsync(queue: "send-events.dlq", durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(queue: "send-events.dlq", exchange: "send-events.dlx", routingKey: "send-events");

            var queueArgs = new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = "send-events.dlx"
            };
            await channel.QueueDeclareAsync(queue: "send-events", durable: true
                , exclusive: false, autoDelete: false, queueArgs);
        }

        private async Task RetryMessageAsync(IChannel channel, BasicDeliverEventArgs eventArgs, int retryCount)
        {
            var properties = new BasicProperties
            {
                Persistent = true,
                Headers = new Dictionary<string, object?>
                {
                    [RetryCountHeader] = Encoding.UTF8.GetBytes(
                 retryCount.ToString())
                }
            };

            await channel.BasicPublishAsync(exchange: "", routingKey: "send-events", mandatory: false
                , basicProperties: properties, body: eventArgs.Body.ToArray());

            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        }

        private int GetRetryCount(BasicDeliverEventArgs eventArgs)
        {
            if (eventArgs.BasicProperties.Headers == null 
                || !eventArgs.BasicProperties.Headers.TryGetValue(RetryCountHeader, out var retryCount))
            {
                return 0;
            }

            if (retryCount is byte[] bytes && int.TryParse(UTF8Encoding.UTF8.GetString(bytes), out var count))
            {
                return count;
            }

            return 0;
        }
    }
}
