namespace dnaborshchikova_github.Bea.Collector.Core.Models
{
    public class EventProcessRange
    {
        public int Id {  get; set; }
        //public List<BillEvent> BillEvents { get; set; }
        public List<SendEvent> SendEvents { get; set; }

        public EventProcessRange(int id, List<SendEvent> sendEvents)
        {
            this.Id = id;
            //this.BillEvents = billEvents;
            this.SendEvents = sendEvents;
        }
    }
}
