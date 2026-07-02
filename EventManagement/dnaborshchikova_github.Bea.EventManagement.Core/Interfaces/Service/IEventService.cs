using dnaborshchikova_github.Bea.EventManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace dnaborshchikova_github.Bea.EventManagement.Core.Interfaces
{
    public interface IEventService
    {
        Task SaveEventAsync(CashRegisterEvent сashRegisterEvent);
        Task SaveEventBatchAsync(List<CashRegisterEvent> сashRegisterEvents);
    }
}
