using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Infrastructure.Model
{
    public class AgentValidationRequest
    {
        public string ContractNumber { get; set; }
        public string ProducerSubCode { get; set; }
        public string StateProducerId { get; set; }
        public string ProducerRoleCode { get; set; }
    }
}
