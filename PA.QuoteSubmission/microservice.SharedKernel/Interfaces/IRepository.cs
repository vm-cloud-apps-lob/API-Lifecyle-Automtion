using PA.QuoteSubmission.SharedKernel.DomainObjects;

namespace PA.QuoteSubmission.SharedKernel.Interfaces
{
    /// <summary>
    /// This interface can be leveraged for implementing the CRUD operations on an Aggregate Root only.
    /// </summary>
    /// <typeparam name="T">The type of entity being operated on by this repository</typeparam>
    public interface IRepository<T> : IReadRepository<T> where T: AggregateRoot
    {
        /// <summary>
        /// Add an entity to the datasource
        /// </summary>
        /// <param name="entity">entity to add</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        void AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add list of entities to the datasource
        /// </summary>
        /// <param name="entities">list of entities to add</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        void AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update an entity
        /// </summary>
        /// <param name="entity">entity to update</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        void UpdateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update list of entities
        /// </summary>
        /// <param name="entities">list of entities to update</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        void UpdateRange(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <param name="entity">entity to delete</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        void DeleteAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete list of entities
        /// </summary>
        /// <param name="entities">list of entities to delete</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        void DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
   
    }
}
