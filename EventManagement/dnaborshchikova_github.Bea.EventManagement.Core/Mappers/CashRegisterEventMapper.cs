using dnaborshchikova_github.Bea.EventManagement.Core.Models;

namespace dnaborshchikova_github.Bea.EventManagement.Core.Mappers
{
    public class CashRegisterEventMapper : ICashRegisterEventMapper
    {
        public CashRegisterEvent ToDomain(CashRegisterEventDto eventDto)
        {
            return new CashRegisterEvent(eventDto.Id, eventDto.Date, eventDto.UserId
                    , eventDto.EventType, eventDto.Data);
        }
    }
}
