using dnaborshchikova_github.Bea.EventManagement.Core.Models;

namespace dnaborshchikova_github.Bea.EventManagement.Core.Services
{
    public interface IEventService
    {
        Task SaveEventAsync(CashRegisterEvent сashRegisterEvent);
        Task SaveEventBatchAsync(List<CashRegisterEvent> сashRegisterEvents);
    }
}
