using Dapr.Client;
using Dapr.Workflow;
using Microsoft.Extensions.Logging;
using PA.QuoteSubmission.Core.SubmissionAggregate;

namespace PA.QuoteSubmission.Infrastructure.Activities
{
    public class SubmissionClearanceActivity : WorkflowActivity<Submission, bool>
    {
        private readonly DaprClient daprClient;
        private readonly ILogger<SubmissionClearanceActivity> logger;
        public SubmissionClearanceActivity(DaprClient daprClient, ILogger<SubmissionClearanceActivity> logger)
        {
            this.daprClient = daprClient;
            this.logger = logger;
        }
        public override async Task<bool> RunAsync(WorkflowActivityContext context, Submission input)
        {
            throw new NotImplementedException();
           // var response = await daprClient.InvokeMethodGrpcAsync<Submission, SubmissionClearanceResponse>("sub-clearance-svc", "InvokeSubmissionClearance", input);
        }
    }
}
