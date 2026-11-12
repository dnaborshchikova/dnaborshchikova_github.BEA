using RabbitMQ.Client;
using System.Text;

namespace dnaborshchikova_github.Bea.EventManagement.WorkerService.Services
{
    public class DlqReprocessingService
    {
        private readonly IConnection _connection;
        private readonly ILogger<DlqReprocessingService> _logger;

        public DlqReprocessingService(IConnection connection, ILogger<DlqReprocessingService> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task<int> ReprocessAsync(CancellationToken cancellationToken)
        {
            var channelOptions = new CreateChannelOptions(publisherConfirmationsEnabled: true
                , publisherConfirmationTrackingEnabled: true);
            await using var channel = await _connection.CreateChannelAsync(channelOptions, cancellationToken);

            var processedCount = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await channel.BasicGetAsync(RabbitMqConstants.DeadLetterQueue, autoAck: false, cancellationToken);
                if (message == null)
                {
                    _logger.LogInformation("DLQ is empty.");
                    break;
                }

                try
                {
                    var properties = GetMessageProperties(message);

                    await channel.BasicPublishAsync(exchange: string.Empty, RabbitMqConstants.EventsQueue
                        , mandatory: true, properties, message.Body, cancellationToken);
                    await channel.BasicAckAsync(message.DeliveryTag, multiple: false, cancellationToken);

                    processedCount++;
                    _logger.LogInformation("Message successfully reprocessed. MessageId={MessageId}",
                        properties.MessageId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to reprocess message. Original message will remain in DLQ.");
                    throw;
                }
            }

            _logger.LogInformation("DLQ reprocessing completed. Messages processed: {Count}", processedCount);

            return processedCount;
        }

        private BasicProperties GetMessageProperties(BasicGetResult message)
        {
            var originalProperties = message.BasicProperties;
            var headers = originalProperties.Headers != null
                ? new Dictionary<string, object?>(originalProperties.Headers)
                : new Dictionary<string, object?>();

            headers[RabbitMqConstants.RetryCountHeader] = Encoding.UTF8.GetBytes("0");
            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = originalProperties.ContentType,
                ContentEncoding = originalProperties.ContentEncoding,
                CorrelationId = originalProperties.CorrelationId,
                MessageId = originalProperties.MessageId,
                Type = originalProperties.Type,
                Headers = headers
            };

            return properties;
        }
    }
}
