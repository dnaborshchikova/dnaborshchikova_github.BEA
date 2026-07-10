namespace dnaborshchikova_github.Bea.EventManagement.Core.Models
{
    public class DuplicateEventException : Exception
    {
        public Guid EventId { get; }

        public DuplicateEventException(Guid eventId)
            : base($"Event {eventId} already exists.")
        {
            EventId = eventId;
        }
    }
}
