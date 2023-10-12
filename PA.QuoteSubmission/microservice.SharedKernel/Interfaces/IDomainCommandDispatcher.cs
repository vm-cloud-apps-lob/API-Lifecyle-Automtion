using PA.QuoteSubmission.SharedKernel.DomainObjects;

namespace PA.QuoteSubmission.SharedKernel.Interfaces
{
    public interface IDomainCommandDispatcher
    {
        void TriggerCommand<T>(CommandRequest<T> request) where T: Entity;
        Task<T> TriggerQuery<T>(CommandRequest<T> request) where T : Entity;
    }
}
