using PA.QuoteSubmission.SharedKernel.DomainObjects;

namespace PA.QuoteSubmission.SharedKernel.Interfaces
{
    /// <summary>
    /// Implement this interface to perform Current state restoration of an Aggregate Root
    /// </summary>
    /// <typeparam name="T">Aggregate Root</typeparam>
    public interface IRestoreCurrentState<T> where T : AggregateRoot
    {
        /// <summary>
        /// This method is leveraged for extracting the current state of an Aggregate from cache
        /// </summary>
        /// <param name="cacheClient">cache client</param>
        /// <param name="cacheKey">key for an aggregate</param>
        /// <returns></returns>
        Task<T> GetCurrentStateFromCache(ICacheService<T> cacheClient, string cacheKey);

        /// <summary>
        /// This method is leveraged for replaying the events for an aggregate
        /// </summary>
        /// <typeparam name="Tevent">event type</typeparam>
        /// <param name="eventStore">event store</param>
        /// <param name="partitionKey">Partition key</param>
        /// <param name="aggregateRoot">aggregate root reference</param>
        /// <returns></returns>
        Task<T> GetCurrentStateByReplay<Tevent>(IEventRepository<Tevent> eventStore, string partitionKey, T aggregateRoot)
            where Tevent : DomainEvent;
    }
}
