using dnaborshchikova_github.Bea.EventManagement.Core.Interfaces;
using dnaborshchikova_github.Bea.EventManagement.Core.Models;

namespace dnaborshchikova_github.Bea.EventManagement.Core.Services
{
    public class EventService : IEventService
    {
        private static IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task CreateAsync(CashRegisterEvent сashRegisterEvent)
        {
            await _eventRepository.SaveAsync(сashRegisterEvent);
        }
    }
}
