namespace PA.QuoteSubmission.SharedKernel.Interfaces
{
    /// <summary>
    /// Apply this marker interface only to aggregate root entities. 
    /// Repositories will only work with aggregate roots, not their children
    /// </summary>
    public interface IAggregateRoot {

        public abstract string Id { get; set; }
        /*void DispatchAndClearEvents();
        void TriggerCommand(CommandRequest<Entity> entity);
        Task<T> TriggerCommand<T>(CommandRequest<T> request) where T : Entity;
        */
    }
}
