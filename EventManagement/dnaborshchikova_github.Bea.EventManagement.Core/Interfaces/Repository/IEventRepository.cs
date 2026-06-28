using dnaborshchikova_github.Bea.EventManagement.Core.Models;

namespace dnaborshchikova_github.Bea.EventManagement.Core.Interfaces
{
    public interface IEventRepository
    {
        Task SaveAsync(CashRegisterEvent сashRegisterEvent);
    }
}
