using Dapr.Client;
using PA.QuoteSubmission.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Infrastructure.Services
{
    public class ReferenceDataService
    {
        private readonly RefDataOptions options;
        private readonly DaprClient daprClient;
        public ReferenceDataService(RefDataOptions options, DaprClient daprClient)
        {
            this.options = options;
            this.daprClient = daprClient;
        }
        
        public async Task<CoverageReferenceData> GetReferenceData(String state)
        {
            var request = this.options.Query;
            var param = this.options.ParameterName;
            var metadata = new Dictionary<string, string>();
            metadata.Add("query", request);
            metadata.Add("variable:"+param, state);
            var response = await daprClient.InvokeBindingAsync<string, CoverageReferenceData>("example.bindings.graphql", "query", "", new ReadOnlyDictionary<string, string>(metadata));
            return response;
        }
    }
}
