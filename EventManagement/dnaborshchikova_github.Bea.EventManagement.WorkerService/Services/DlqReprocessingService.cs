using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace dnaborshchikova_github.Bea.EventManagement.WorkerService.Services
{
    public class DlqReprocessingService
    {
        private const string DlqName = "send-events.dlq";
        private const string MainQueueName = "send-events";
        private const string RetryCountHeader = "x-retry-count";

        private readonly IConnection _connection;
        private readonly ILogger<DlqReprocessingService> _logger;

        public DlqReprocessingService(IConnection connection, ILogger<DlqReprocessingService> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task<int> ReprocessAsync(int batchSize, CancellationToken cancellationToken)
        {
            var channelOptions = new CreateChannelOptions(publisherConfirmationsEnabled: true
                , publisherConfirmationTrackingEnabled: true);
            await using var channel = await _connection.CreateChannelAsync(channelOptions, cancellationToken);

            var processedCount = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await channel.BasicGetAsync(queue: DlqName, autoAck: false, cancellationToken);
                if (message == null)
                {
                    _logger.LogInformation("DLQ is empty.");
                    break;
                }

                try
                {
                    var originalProperties = message.BasicProperties;
                    var headers = originalProperties.Headers != null
                        ? new Dictionary<string, object?>(originalProperties.Headers)
                        : new Dictionary<string, object?>();

                    headers[RetryCountHeader] = Encoding.UTF8.GetBytes("0");
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

                    await channel.BasicPublishAsync(exchange: "", routingKey: MainQueueName, mandatory: true,
                        basicProperties: properties, message.Body, cancellationToken);

                    // Если выполнение дошло сюда,
                    // публикация получила подтверждение.
                    await channel.BasicAckAsync(message.DeliveryTag, multiple: false, cancellationToken);

                    processedCount++;
                    _logger.LogInformation(
                        "Message successfully reprocessed. MessageId={MessageId}",
                        properties.MessageId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to reprocess message. Original message will remain in DLQ.");

                    throw;
                }
            }

            _logger.LogInformation(
               "DLQ reprocessing completed. Messages processed: {Count}",
               processedCount);

            return processedCount;
        }
    }
}
