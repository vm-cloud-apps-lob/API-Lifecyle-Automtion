using MediatR;
using PA.QuoteSubmission.SharedKernel.Interfaces;

namespace PA.QuoteSubmission.SharedKernel.DomainObjects
{
    /// <summary>
    /// Domain event base class. This has to extended when implementing specific events
    /// </summary>
    public abstract class DomainEvent : INotification
    {
        /// <summary>
        /// ID of an event
        /// </summary>
        public string EventId { get; set; }

        /// <summary>
        /// Date time of the event occurred
        /// </summary>
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;

        /// <summary>
        /// Type of the event occurred
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Process generated event on an aggregate root based on the event type.
        /// </summary>
        /// <param name="aggregateRoot"></param>
        public abstract void ProcessEvent(IAggregateRoot aggregateRoot);

    }
    public enum EventType
    {
        CREATE,
        UPDATE,
        DELETE
    }
}
