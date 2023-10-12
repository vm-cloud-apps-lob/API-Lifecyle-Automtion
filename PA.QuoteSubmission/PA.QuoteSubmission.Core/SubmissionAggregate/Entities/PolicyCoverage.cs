using PA.QuoteSubmission.SharedKernel.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Core.SubmissionAggregate.Entities
{
    public class PolicyCoverage : Entity
    {
        public string? CoverageCode { get; set; }
        public string? CoverageLimit { get; set; }
        public string? Deductible { get; set; }
    }
}
