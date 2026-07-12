using dnaborshchikova_github.Bea.Collector.Core.Interfaces;
using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Collector.Sender;
using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.Collector.Senders
{
    public class ApiSender : IEventSender
    {
        private readonly IEventsClient _eventsClient;
        private readonly ILogger<ApiSender> _logger;

        public ApiSender(IEventsClient eventsClient, ILogger<ApiSender> logger)
        {
            _eventsClient = eventsClient;
            _logger = logger;
        }

        public async Task SendAsync(EventProcessRange range)
        {
            _logger.LogInformation("SendBatch RangeId={RangeId} Count={Count}",
                range.Id, range.SendEvents.Count);

            var events = range.SendEvents.Select(MapToDto);

            await _eventsClient.BatchAsync(events);
        }

        public void Send(EventProcessRange range)
        {
            SendAsync(range).GetAwaiter().GetResult();
        }

        private CashRegisterEventDto MapToDto(SendEvent e)
        {
            return new CashRegisterEventDto
            {
                Id = e.Id,
                Date = e.Date,
                UserId = e.UserId,
                EventType = e.EventType,
                Data = e.Data
            };
        }
    }
}
