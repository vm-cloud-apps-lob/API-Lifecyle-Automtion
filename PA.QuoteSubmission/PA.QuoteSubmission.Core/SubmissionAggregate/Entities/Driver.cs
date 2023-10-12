using PA.QuoteSubmission.SharedKernel.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Core.SubmissionAggregate.Entities
{
    public class Driver : Entity
    {
        public string? DriverId { get; set; }
        public string? DriverName { get; set; }
        public string? LicenseNumber { get; set; }
        public string? LicenseStateCode { get; set; }
        public string? LicenseStatus { get; set; }
    }
}
