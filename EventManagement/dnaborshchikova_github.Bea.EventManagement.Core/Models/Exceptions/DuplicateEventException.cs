using System;
using System.Collections.Generic;
using System.Text;

namespace dnaborshchikova_github.Bea.EventManagement.Core.Models.Exceptions
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
