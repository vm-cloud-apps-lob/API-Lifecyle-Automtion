using PA.QuoteSubmission.SharedKernel.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Core.SubmissionAggregate.Entities
{
    public class Agent : Entity
    {
        public string? AgentName { get; set; }
        public string? ContractNumber { get; set; }
        public string? ProducerSubCode { get; set; }
        public string? StateProducerId { get; set; }
        public string? ProducerRoleCode { get; set; }
    }
}
