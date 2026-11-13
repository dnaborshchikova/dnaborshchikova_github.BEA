using dnaborshchikova_github.Bea.EventManagement.Core.Mappers;
using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using dnaborshchikova_github.Bea.EventManagement.Core.Services;
using dnaborshchikova_github.Bea.EventManagement.WorkerService.RabbitMq;
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
        private readonly RabbitMqQueueInitializer _rabbitMqQueueInitializer;

        public EventConsumer(IConnection connection, IServiceScopeFactory serviceScopeFactory
            , ILogger<EventConsumer> logger, RabbitMqQueueInitializer rabbitMqQueueInitializer)
        {
            _connection = connection;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _rabbitMqQueueInitializer = rabbitMqQueueInitializer;
        }

        public async Task StartAsync(CancellationToken token)
        {
            var channelOptions = new CreateChannelOptions(publisherConfirmationsEnabled: true
                , publisherConfirmationTrackingEnabled: true);
            var channel = await _connection.CreateChannelAsync(channelOptions, token);

            await _rabbitMqQueueInitializer.CreateQueueAsync(channel);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                await TryProccessMessage(channel, eventArgs);
            };

            await channel.BasicConsumeAsync(queue: "send-events", autoAck: false, consumer: consumer);
            _logger.LogInformation("RabbitMQ consumer started.");
            await Task.Delay(Timeout.Infinite, token);
        }

        private async Task TryProccessMessage(IChannel channel, BasicDeliverEventArgs eventArgs)
        {
            try
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var cashRegisterEventDto = JsonSerializer.Deserialize<CashRegisterEventDto>(message);

                using var scope = _serviceScopeFactory.CreateScope();
                var cashRegisterEventMapper = scope.ServiceProvider.GetRequiredService<ICashRegisterEventMapper>();
                var cashRegisterEvent = cashRegisterEventMapper.ToDomain(cashRegisterEventDto);

                var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
                //throw new Exception("TEST RETRY"); //TODO: удалить, оставлено для тестирования DLQ

                await eventService.SaveEventAsync(cashRegisterEvent);
                _logger.LogInformation("Event saved successfully. Id={Id}", cashRegisterEvent.Id);

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                _logger.LogInformation("RabbitMQ message acknowledged. Id={Id}", cashRegisterEvent.Id);
            }
            catch (DuplicateEventException ex)
            {
                _logger.LogInformation("Event is already been added. Id={Id}", ex.EventId);
                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                var retryCount = GetRetryCount(eventArgs);
                _logger.LogError(ex, "Failed to process message. Retry attempt: {RetryCount}/{MaxRetryCount}. {Exception}"
                    , retryCount, RabbitMqConstants.MaxRetryCount, ex.ToString());

                if (retryCount < RabbitMqConstants.MaxRetryCount)
                {
                    await RetryMessageAsync(channel, eventArgs, retryCount + 1);
                }
                else
                {
                    _logger.LogError("Max retry count reached. Sending message to DLQ.");
                    await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
                }
            }
        }

        private async Task RetryMessageAsync(IChannel channel, BasicDeliverEventArgs eventArgs, int retryCount)
        {
            var properties = GetRetryMessageProperties(eventArgs, retryCount);

            await channel.BasicPublishAsync(exchange: string.Empty, RabbitMqConstants.EventsRoutingKey, mandatory: true
                , properties, eventArgs.Body.ToArray());

            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        }

        private BasicProperties GetRetryMessageProperties(BasicDeliverEventArgs eventArgs, int retryCount)
        {
            var originalProperties = eventArgs.BasicProperties;

            var headers = originalProperties.Headers != null
                ? new Dictionary<string, object?>(originalProperties.Headers)
                : new Dictionary<string, object?>();

            headers[RabbitMqConstants.RetryCountHeader] =
                Encoding.UTF8.GetBytes(retryCount.ToString());

            var properties = new BasicProperties
            {
                Persistent = true,
                Headers = headers,
                MessageId = originalProperties.MessageId,
                CorrelationId = originalProperties.CorrelationId,
                ContentType = originalProperties.ContentType,
                ContentEncoding = originalProperties.ContentEncoding,
                Type = originalProperties.Type
            };

            return properties;
        }

        private int GetRetryCount(BasicDeliverEventArgs eventArgs)
        {
            if (eventArgs.BasicProperties.Headers == null 
                || !eventArgs.BasicProperties.Headers.TryGetValue(RabbitMqConstants.RetryCountHeader, out var retryCount))
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
