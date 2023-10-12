using Castle.Core.Logging;
using Dapr.Workflow;
using Microsoft.Extensions.Logging;
using PA.QuoteSubmission.Infrastructure.Model;
using PA.QuoteSubmission.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Infrastructure.Activities
{
    public class RetrieveAndCacheReferenceDataActivity : WorkflowActivity<string, IEnumerable<CoverageRefData>>
    {
        private readonly ILogger<RetrieveAndCacheReferenceDataActivity> logger;
        private readonly ReferenceDataService refDataService;
        private readonly ReferenceDataCacheService refDataCacheService;
        //private readonly 
        public RetrieveAndCacheReferenceDataActivity(
            ILogger<RetrieveAndCacheReferenceDataActivity> logger,
            ReferenceDataService refDataService,
            ReferenceDataCacheService refDataCacheService
            ) 
        {
            this.logger = logger;
            this.refDataService = refDataService;
            this.refDataCacheService = refDataCacheService;
        }
        public override async Task<IEnumerable<CoverageRefData>> RunAsync(WorkflowActivityContext context, string state)
        {
            logger.LogInformation($"{context.Name} Activity is started");
            var data = await refDataCacheService.GetValue(state);
            if (data == null)
            {
               data = await refDataService.GetReferenceData(state);
               refDataCacheService.SaveValue(data, state);
            }
            logger.LogInformation($"{context.Name} Activity is completed");
            //
            return data.Coverages;
        }
    }
}
