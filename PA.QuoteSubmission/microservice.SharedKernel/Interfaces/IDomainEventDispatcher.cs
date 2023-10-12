using PA.QuoteSubmission.SharedKernel.DomainObjects;

namespace PA.QuoteSubmission.SharedKernel.Interfaces
{
    /// <summary>
    /// Domain Event Dispatcher Interface
    /// </summary>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// Publish the generated domain events and clear them from the entity
        /// </summary>
        /// <param name="entitiesWithEvents"></param>
        /// <returns></returns>
        Task DispatchAndClearEvents(IEnumerable<Entity> entitiesWithEvents);
        Task DispatchAndClearEvent(Entity entitiesWithEvent);
    }
}
