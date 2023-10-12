using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PA.QuoteSubmission.SharedKernel.DomainObjects
{
    /// <summary>
    /// Entity Base class
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Entity ID
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// list of domain events an entity to hold
        /// </summary>
        private List<DomainEvent> _domainEvents = new();

        [JsonIgnore]
        [NotMapped]
        public IEnumerable<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Register Domain event to an entity
        /// </summary>
        /// <param name="domainEvent">domain event</param>
        public void RegisterDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        /// <summary>
        /// Clear the domain events generated for an entity
        /// </summary>
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
