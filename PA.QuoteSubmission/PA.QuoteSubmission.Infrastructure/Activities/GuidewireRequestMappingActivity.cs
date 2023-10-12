using Dapr.Workflow;
using PA.QuoteSubmission.Core.SubmissionAggregate;
using PA.QuoteSubmission.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Infrastructure.Activities
{
    public class GuidewireRequestMappingActivity : WorkflowActivity<Submission, GuidewireSubmissionRequest>
    {
        public async override Task<GuidewireSubmissionRequest> RunAsync(WorkflowActivityContext context, Submission input)
        {
            return new GuidewireSubmissionRequest();
            
        }
    }
}
