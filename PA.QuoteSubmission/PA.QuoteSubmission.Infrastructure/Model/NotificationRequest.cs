using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Infrastructure.Model
{
    public class NotificationRequest
    {
        public IEnumerable<string> SenderList { get; set; }
        public string SubmissionId { get; set; }
        public string ProcessStatus { get; set; }
        public string Message { get; set; }
    }
    
}
