using PA.QuoteSubmission.SharedKernel.DomainObjects;

namespace PA.QuoteSubmission.SharedKernel.Interfaces
{
    /// <summary>
    /// Interface can be used for querying the data from the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadRepository<T> where T: AggregateRoot
    {
        Task<T> ReadById(string id, string partitionKey, CancellationToken cancellationToken = default);

        Task<List<T>> GetByStoredProcedure(string spName, string partitionKey, dynamic[] parameters, CancellationToken cancellationToken = default);
    }
}
