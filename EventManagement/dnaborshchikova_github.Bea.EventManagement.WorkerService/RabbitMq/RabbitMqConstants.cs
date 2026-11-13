namespace dnaborshchikova_github.Bea.EventManagement.WorkerService.RabbitMq
{
    public static class RabbitMqConstants
    {
        public const string EventsQueue = "send-events";
        public const string DeadLetterQueue = "send-events.dlq";
        public const string DeadLetterExchange = "send-events.dlx";
        public const string EventsRoutingKey = "send-events";

        public const string RetryCountHeader = "x-retry-count";
        public const int MaxRetryCount = 3;
    }
}
