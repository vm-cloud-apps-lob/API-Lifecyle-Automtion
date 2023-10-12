using PA.QuoteSubmission.Core.SubmissionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Infrastructure.Model
{
    public class EventMessage
    {
        public string EventName { get; set; }
        public string EventId { get; set; }
        public Submission Submission { get; set; }
    }
}
