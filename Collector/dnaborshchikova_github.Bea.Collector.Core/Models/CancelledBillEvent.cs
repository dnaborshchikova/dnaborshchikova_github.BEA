
namespace dnaborshchikova_github.Bea.Collector.Core.Models
{
    public class CancelledBillEvent : BillEvent
    {
        public decimal? CancelAmount { get; set; }

        public CancelledBillEvent()
        {
        }

        public CancelledBillEvent(Guid id, DateTime operationDateTime, Guid userId, string eventType
            , decimal amount, string number, decimal? cancelAmount) : base(id, operationDateTime, 
                                                                           userId, eventType, amount, number)
        {
            CancelAmount = cancelAmount;
        }
    }
}
