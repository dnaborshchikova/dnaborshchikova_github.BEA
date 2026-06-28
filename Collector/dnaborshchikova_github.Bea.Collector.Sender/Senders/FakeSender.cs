using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace dnaborshchikova_github.Bea.Collector.Sender.Handlers
{
    public class FakeSender : IEventSender
    {
        private readonly ILogger<FakeSender> _logger;

        public FakeSender(ILogger<FakeSender> logger)
        {
            _logger = logger;
        }

        public void Send(EventProcessRange range)
        {
            _logger.LogInformation($"Start send {DateTime.Now}. Thread id {Thread.CurrentThread.ManagedThreadId}." +
                $"Range id {range.Id}.");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Thread.Sleep(20);

            foreach (var billEvent in range.BillEvents)
            {
                var sendEvent = GetSendEvent(billEvent);

                const int maxRetries = 3;
                for (var attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"Attempt #{attempt} failed. " +
                            $"Thread id {Thread.CurrentThread.ManagedThreadId}. {ex}");

                        if (attempt != maxRetries)
                        {
                            Thread.Sleep(1000 * attempt);
                        }
                    }
                }
            }

            stopwatch.Stop();
            _logger.LogInformation($"End send {DateTime.Now}. Thread id {Thread.CurrentThread.ManagedThreadId}." +
                $"Range id {range.Id}. Work time: {stopwatch.ElapsedMilliseconds} ms.");
        }

        public Task SendAsync(EventProcessRange range)
        {
            throw new NotImplementedException();
        }

        private SendEvent GetSendEvent(BillEvent billEvent)
        {
            var billData = billEvent switch
            {
                PaidBillEvent paid => JsonSerializer.Serialize(paid),
                CancelledBillEvent cancelled => JsonSerializer.Serialize(cancelled)
            };

            return new SendEvent(billEvent.Id, billEvent.OperationDateTime, billEvent.UserId
                , billEvent.EventType, billData);
        }
    }
}

