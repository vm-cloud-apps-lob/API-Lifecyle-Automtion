using MediatR;

namespace PA.QuoteSubmission.SharedKernel.DomainObjects
{
    public abstract class CommandRequest<T> : IRequest<T> where T : Entity
    {
        public abstract string commandId { get; set; }
    }
}
