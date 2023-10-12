using Dapr.Client;
using PA.QuoteSubmission.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Infrastructure.Services
{
    public class ReferenceDataCacheService : CacheService<CoverageReferenceData>
    {
        public ReferenceDataCacheService(DaprClient _daprClient, CacheOptions cacheOptions) : base(_daprClient, cacheOptions)
        {
        }
    }
}
