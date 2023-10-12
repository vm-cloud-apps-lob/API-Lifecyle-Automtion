using PA.QuoteSubmission.SharedKernel.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Core.SubmissionAggregate.Entities
{
    public class Vehicle : Entity
    {
        public string? VehicleId { get; set; }
        public string? Model { get; set; }
        public string? Vin { get; set; }
        public string? Year { get; set; }
        public List<Driver>? Driver { get; set; }
        public List<VehicleCoverage>? VehicleCoverages { get; set; }
    }
}
