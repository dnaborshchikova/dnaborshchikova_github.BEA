using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace dnaborshchikova_github.Bea.Collector.Core.Models
{
    public class BillEvent
    {
        [JsonIgnore]
        [Required]
        public Guid Id { get; set; }

        [JsonIgnore]
        [Required]
        public DateTime OperationDateTime { get; set; }

        [JsonIgnore]
        [Required]
        public Guid UserId { get; set; }

        [JsonIgnore]
        [Required]
        public string EventType { get; set; } // "bill_payed" или "bill_canceled"

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Number { get; set; }

        public BillEvent()
        {

        }

        public BillEvent(Guid id, DateTime operationDateTime, Guid userId, string eventType
            , decimal amount, string number)
        {
            Id = id;
            OperationDateTime = operationDateTime;
            UserId = userId;
            EventType = eventType;
            Amount = amount;
            Number = number;
        }
    }
}
