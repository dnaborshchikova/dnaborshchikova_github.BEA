using dnaborshchikova_github.Bea.EventManagement.Core.Interfaces;
using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using Microsoft.Extensions.Logging;

namespace dnaborshchikova_github.Bea.EventManagement.Core.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<EventService> _logger;

        public EventService(IEventRepository eventRepository, ILogger<EventService> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task SaveEventAsync(CashRegisterEvent сashRegisterEvent)
        {
            try
            {
                await _eventRepository.SaveAsync(сashRegisterEvent);
                _logger.LogDebug($"DB save SUCCESS. Id={сashRegisterEvent.Id}");
            }
            catch (DuplicateEventException ex)
            {
                _logger.LogError(ex, $"DB save FAILED (duplicate or constraint violation). Id={сashRegisterEvent.Id}"); //TODO: пересмотреть при обработке ошибок
            }
        }

        public async Task SaveEventBatchAsync(List<CashRegisterEvent> events)
        {
            _logger.LogInformation("Save batch START. Count={Count}", events.Count);

            await _eventRepository.SaveBatchAsync(events);

            _logger.LogInformation("Save batch END");
        }
    }
}
