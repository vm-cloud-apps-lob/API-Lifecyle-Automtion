using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using domObj = PA.QuoteSubmission.SharedKernel.DomainObjects;
namespace PA.QuoteSubmission.Core.SubmissionAggregate.ValueObject
{
    public class DriverLicenseInfo : domObj.ValueObject
    {
        public string? LicenseNumber { get; set; }
        public string? LicenseStateCode { get; set; }
        public string? LicenseStatus { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            var licFields = new List<string>();
            licFields.Add(LicenseNumber ?? string.Empty);
            return licFields;
        }
    }
}
