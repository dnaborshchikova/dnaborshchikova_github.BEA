using dnaborshchikova_github.Bea.EventManagement.Core.Models;

namespace dnaborshchikova_github.Bea.EventManagement.Core.Interfaces
{
    public interface IEventRepository
    {
        bool IsEventExists(CashRegisterEvent сashRegisterEvent);
        Task SaveAsync(CashRegisterEvent сashRegisterEvent);
        Task SaveBatchAsync(List<CashRegisterEvent> сashRegisterEvent);
    }
}
