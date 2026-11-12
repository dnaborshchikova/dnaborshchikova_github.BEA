using RabbitMQ.Client;

namespace dnaborshchikova_github.Bea.EventManagement.WorkerService.RabbitMq
{
    public class RabbitMqQueueInitializer
    {
        public async Task CreateQueueAsync(IChannel channel)
        {
            await channel.ExchangeDeclareAsync(
                RabbitMqConstants.DeadLetterExchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            await channel.QueueDeclareAsync(
                RabbitMqConstants.DeadLetterQueue,
                durable: true,
                exclusive: false,
                autoDelete: false);

            await channel.QueueBindAsync(
                RabbitMqConstants.DeadLetterQueue,
                RabbitMqConstants.DeadLetterExchange,
                RabbitMqConstants.EventsRoutingKey);

            var queueArgs = new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = RabbitMqConstants.DeadLetterExchange
            };

            await channel.QueueDeclareAsync(
                RabbitMqConstants.EventsQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArgs);
        }
    }
}
