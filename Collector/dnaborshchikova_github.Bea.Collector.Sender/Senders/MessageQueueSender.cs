using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace dnaborshchikova_github.Bea.Collector.Senders
{
    public class MessageQueueSender : IEventSender
    {
        private readonly IConnection _connection;
        private readonly ILogger<MessageQueueSender> _logger;

        public MessageQueueSender(IConnection connection, ILogger<MessageQueueSender> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public void Send(EventProcessRange range)
        {
            #region delete
            //_logger.LogInformation($"Start send {DateTime.Now}. Thread id {Thread.CurrentThread.ManagedThreadId}." +
            //    $"Range id {range.Id}.");
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
            //Thread.Sleep(20);

            //foreach (var sendEvent in range.SendEvents)
            //{
            //    const int maxRetries = 3;
            //    for (var attempt = 1; attempt <= maxRetries; attempt++)
            //    {
            //        try
            //        {
            //            break;
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogInformation($"Attempt #{attempt} failed. " +
            //                $"Thread id {Thread.CurrentThread.ManagedThreadId}. {ex}");

            //            if (attempt != maxRetries)
            //            {
            //                Thread.Sleep(1000 * attempt);
            //            }
            //        }
            //    }
            //}

            //stopwatch.Stop();
            //_logger.LogInformation($"End send {DateTime.Now}. Thread id {Thread.CurrentThread.ManagedThreadId}." +
            //    $"Range id {range.Id}. Work time: {stopwatch.ElapsedMilliseconds} ms.");

            #endregion

            SendAsync(range).GetAwaiter().GetResult();
        }

        public async Task SendAsync(EventProcessRange range)
        {
            using var channel = await _connection.CreateChannelAsync();

            await channel.QueueDeclareAsync("send-events", durable: true, exclusive: false, autoDelete: false);

            foreach (var sendEvent in range.SendEvents)
            {
                Console.WriteLine($"Publishing message {sendEvent.Id}...");
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(sendEvent));
                await channel.BasicPublishAsync(exchange: "", routingKey: "send-events", body: body);
                Console.WriteLine("Published");
            }
        }
    }
}

