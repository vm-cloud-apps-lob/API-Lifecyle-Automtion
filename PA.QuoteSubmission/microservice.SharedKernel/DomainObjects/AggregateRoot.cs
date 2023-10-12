using PA.QuoteSubmission.SharedKernel.Interfaces;

namespace PA.QuoteSubmission.SharedKernel.DomainObjects
{
    public abstract class AggregateRoot : Entity, IAggregateRoot
    {
        public abstract string Id { get; set; }
    }
}
