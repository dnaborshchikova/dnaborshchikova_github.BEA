using dnaborshchikova_github.Bea.EventManagement.Core.Models;

namespace dnaborshchikova_github.Bea.EventManagement.Core.Mappers
{
    public interface ICashRegisterEventMapper
    {
        CashRegisterEvent ToDomain(CashRegisterEventDto ingestEventDto);
    }
}
