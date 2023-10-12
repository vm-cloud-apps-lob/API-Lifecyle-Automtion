using Dapr.Workflow;
using PA.QuoteSubmission.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Infrastructure.Activities
{
    public class SubmitToGuidewareActivity : WorkflowActivity<GuidewireSubmissionRequest, bool>
    {
        public override Task<bool> RunAsync(WorkflowActivityContext context, GuidewireSubmissionRequest input)
        {
            throw new NotImplementedException();
        }
    }
}
