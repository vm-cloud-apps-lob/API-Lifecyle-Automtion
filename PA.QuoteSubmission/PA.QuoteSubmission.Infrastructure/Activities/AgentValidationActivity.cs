using Dapr.Workflow;
using PA.QuoteSubmission.Core.SubmissionAggregate.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Infrastructure.Activities
{
    public class AgentValidationActivity : WorkflowActivity<Agent, bool>
    {
        public async override Task<bool> RunAsync(WorkflowActivityContext context, Agent input)
        {
            //throw new NotImplementedException();
            return true;
        }
    }
}
