using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using domObj = PA.QuoteSubmission.SharedKernel.DomainObjects;

namespace PA.QuoteSubmission.Core.SubmissionAggregate.ValueObject
{
    public class Address: domObj.ValueObject
    {
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Street { get; set; }
        public string? State { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            var addressFields = new List<string>();
            addressFields.Add(this.AddressLine1 ?? string.Empty);
            addressFields.Add(this.AddressLine2 ?? string.Empty);
            addressFields.Add(this.Street ?? string.Empty);
            addressFields.Add(this.State ?? string.Empty);
            return addressFields;
        }
    }
}
