using Castle.Core.Logging;
using Dapr.Workflow;
using Microsoft.Extensions.Logging;
using PA.QuoteSubmission.Core.SubmissionAggregate;
using PA.QuoteSubmission.Infrastructure.Model;
using PA.QuoteSubmission.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Infrastructure.Activities
{
    public class SaveEventStoreActivity : WorkflowActivity<EventMessage, object>

    {
        private readonly SubmissionEventStoreService storeService;
        private readonly ILogger<SaveEventStoreActivity> logger;
        public SaveEventStoreActivity(SubmissionEventStoreService storeService, ILogger<SaveEventStoreActivity> logger) 
        {
            this.storeService = storeService;    
            this.logger = logger;
        }
        public override async Task<object> RunAsync(WorkflowActivityContext context, EventMessage input)
        {
            logger.LogInformation($"{context.Name} Activity is started");

            try
            {
                await storeService.SaveValueToPartitionAsync(input, input.EventId, input.Submission.QuoteNumber ?? input.EventId);
                logger.LogInformation($"{input.Submission.QuoteNumber} submission is saved to event store with event id : {input.EventId}");
            }
            catch(Exception e)
            {
                logger.LogError(e.Message);
                logger.LogError(e.StackTrace);
                logger.LogError(e.InnerException.Message);
            }
            return Task.FromResult<object>(null);
        }
    }
}
