namespace dnaborshchikova_github.Bea.EventManagement.Core.Models
{
    public class CashRegisterEvent
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public string EventType { get; set; } // enum?
        public string Data { get; set; } // JSON
    }
}
