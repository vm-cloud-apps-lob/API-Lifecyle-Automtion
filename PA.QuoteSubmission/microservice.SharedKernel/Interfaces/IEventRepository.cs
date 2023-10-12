using PA.QuoteSubmission.SharedKernel.DomainObjects;

namespace PA.QuoteSubmission.SharedKernel.Interfaces
{
    public interface IEventRepository<T> where T: DomainEvent
    {
        Task<List<T>> GetAllEvents(String partitionKey, CancellationToken cancellationToken = default);
    }
}
