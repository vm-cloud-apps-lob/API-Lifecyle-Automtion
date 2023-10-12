using MediatR;
using PA.QuoteSubmission.SharedKernel.DomainObjects;
using PA.QuoteSubmission.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;

namespace PA.QuoteSubmission.SharedKernel.Implementation
{
    public class DomainCommandDispatcher : IDomainCommandDispatcher
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        public DomainCommandDispatcher(IMediator mediator, ILogger<DomainCommandDispatcher> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        public async void TriggerCommand<T>(CommandRequest<T> request) where T : Entity
        {
            await _mediator.Send(request);
            _logger.LogInformation("Command with commandID: {s} is processed successfully", request.commandId);
        }
        public async Task<T> TriggerQuery<T>(CommandRequest<T> request) where T : Entity
        {
            _logger.LogInformation("Retrieving query from Repository");
            return await _mediator.Send(request);
        }
    }
}
