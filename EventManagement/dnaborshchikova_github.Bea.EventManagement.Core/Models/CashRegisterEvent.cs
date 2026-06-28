namespace dnaborshchikova_github.Bea.EventManagement.Core.Models
{
    public class CashRegisterEvent
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public string EventType { get; set; }
        public string Data { get; set; }
    }
}
