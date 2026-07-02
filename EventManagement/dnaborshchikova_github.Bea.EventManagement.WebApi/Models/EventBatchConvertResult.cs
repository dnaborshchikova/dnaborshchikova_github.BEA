using dnaborshchikova_github.Bea.EventManagement.Core.Models;

namespace dnaborshchikova_github.Bea.EventManagement.WebApi.Models
{
    public class EventBatchConvertResult
    {
        public List<CashRegisterEvent> Events { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}
