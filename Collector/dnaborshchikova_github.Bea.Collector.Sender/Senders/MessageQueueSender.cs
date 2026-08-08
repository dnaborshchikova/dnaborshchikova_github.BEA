using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Diagnostics;
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
            SendAsync(range).GetAwaiter().GetResult();
        }

        public async Task SendAsync(EventProcessRange range)
        {
            using var channel = await _connection.CreateChannelAsync();

            await channel.QueueDeclareAsync("send-events", durable: true, exclusive: false, autoDelete: false);

            _logger.LogInformation($"Start send events. Range id: {range.Id}. Event count: {range.SendEvents.Count}. " +
                $"Thread id: {Thread.CurrentThread.ManagedThreadId}.");
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var sendEvent in range.SendEvents)
            {
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(sendEvent));
                await channel.BasicPublishAsync(exchange: "", routingKey: "send-events", body: body);
            }

            stopwatch.Stop();
            _logger.LogInformation($"End send events. Range id: {range.Id}. Event count: {range.SendEvents.Count}. "
                + $"Thread id: {Thread.CurrentThread.ManagedThreadId}. "
                + $"Work time: {stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}

