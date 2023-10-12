using MediatR;
using PA.QuoteSubmission.SharedKernel.DomainObjects;
using PA.QuoteSubmission.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;

namespace PA.QuoteSubmission.SharedKernel.Implementation
{
    /// <summary>
    /// Domain Event Dispatcher implementation. Initiates the notification (mediatr) for all relevant processes 
    /// </summary>
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        public DomainEventDispatcher(IMediator mediator, ILogger<DomainEventDispatcher> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task DispatchAndClearEvents(IEnumerable<Entity> entitiesWithEvents)
        {
            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.DomainEvents.ToArray();
                entity.ClearDomainEvents();
                foreach (var domainEvent in events)
                {
                    await _mediator.Publish(domainEvent).ConfigureAwait(false);
                }
            }
        }

        public async Task DispatchAndClearEvent(Entity entitiyWithEvent)
        {

            var events = entitiyWithEvent.DomainEvents.ToArray();
            entitiyWithEvent.ClearDomainEvents();
            foreach (var domainEvent in events)
            {
                await _mediator.Publish(domainEvent).ConfigureAwait(false);
            }

        }
    }
}
