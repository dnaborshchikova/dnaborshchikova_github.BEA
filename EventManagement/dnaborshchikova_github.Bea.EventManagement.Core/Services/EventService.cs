using dnaborshchikova_github.Bea.EventManagement.Core.Interfaces;
using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using dnaborshchikova_github.Bea.EventManagement.Core.Models.Exceptions;
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
                _logger.LogInformation("Saved event id={Id}.", сashRegisterEvent.Id);
            }
            catch (DuplicateEventException)
            {
                _logger.LogInformation("Event already exists id={Id}.", сashRegisterEvent.Id);
            }
        }

        public async Task SaveEventBatchAsync(List<CashRegisterEvent> cashRegisterEvents)
        {
            await _eventRepository.SaveBatchAsync(cashRegisterEvents);
        }
    }
}
