namespace dnaborshchikova_github.Bea.Collector.Core.Models
{
    public class PaidBillEvent : BillEvent
    {
        public Guid? BuyerId { get; set; }

        public PaidBillEvent()
        {

        }

        public PaidBillEvent(Guid id, DateTime operationDateTime, Guid userId, string eventType
            , decimal amount, string number, Guid? buyerId) : base(id, operationDateTime,
                                                                   userId, eventType, amount, number)
        {
            BuyerId = buyerId;
        }
    }
}
