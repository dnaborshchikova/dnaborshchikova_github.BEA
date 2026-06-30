using System.Globalization;

namespace dnaborshchikova_github.Bea.EventManagement.Core.Models
{
    public class CashRegisterEvent
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public string EventType { get; set; }
        public string Data { get; set; }

        public CashRegisterEvent(Guid id, DateTime date, Guid userId, string eventType, string data)
        {
            this.Id = id;
            this.Date = date;
            this.UserId = userId;
            this.EventType = eventType;
            this.Data = data;
        }
    }
}
