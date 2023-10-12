using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Infrastructure.Model
{
    public class CoverageReferenceData
    {
        public string State { get; set; }
        public IEnumerable<CoverageRefData> Coverages { get; set; }
    }

    public class CoverageRefData
    {
        public string Name { get; set; }
        public string Limit { get; set; }
        public string Deductible { get; set; }
    }
}
