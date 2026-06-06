namespace dnaborshchikova_github.Bea.Collector.Core.Models
{
    public class EventProcessRange
    {
        public int Id {  get; set; }
        public List<BillEvent> BillEvents { get; set; }

        public EventProcessRange(int id, List<BillEvent> billEvents)
        {
            this.Id = id;
            this.BillEvents = billEvents;
        }
    }
}
