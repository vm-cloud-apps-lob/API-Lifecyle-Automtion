using PA.QuoteSubmission.Core.SubmissionAggregate.ValueObject;
using PA.QuoteSubmission.SharedKernel.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Core.SubmissionAggregate.Entities
{
    public class PrimaryInsured : Entity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Address? Address { get; set; }
        public DriverLicenseInfo? DriverLicenseInfo { get; set; }
    }
}
