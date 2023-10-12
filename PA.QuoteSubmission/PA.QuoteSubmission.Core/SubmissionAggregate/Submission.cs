using PA.QuoteSubmission.Core.SubmissionAggregate.Entities;
using PA.QuoteSubmission.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Core.SubmissionAggregate
{
    public class Submission : IAggregateRoot
    {
        public string? SubmissionId { get; set; }
        public string? QuoteNumber { get; set; }
        public string? ProductName { get; set; }
        public string? RiskState { get; set; }
        public string? LobName { get; set; }
        public Agent? Agent { get; set; }
        public PrimaryInsured? PrimaryInsured { get; set; }
        public List<Vehicle>? Vehicle { get; set; }
        public List<PolicyCoverage>? PolicyCoverages { get; set; }
        public string? Id { get ; set ; }
    }
}
