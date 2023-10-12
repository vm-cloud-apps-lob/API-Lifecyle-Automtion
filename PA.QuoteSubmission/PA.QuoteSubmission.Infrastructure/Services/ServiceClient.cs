using Dapr.Client;
using PA.QuoteSubmission.Infrastructure.Model;
using PA.QuoteSubmission.SharedKernel.Interfaces;
using Newtonsoft.Json;

namespace PA.QuoteSubmission.Infrastructure.Services
{
    internal class ServiceClient<TRequest, TResponse> : IServiceClient<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        private readonly DaprClient _daprClient;

        private readonly ServiceClientOptions clientOptions;
        public ServiceClient(DaprClient daprClient, ServiceClientOptions clientOptions) {
            this._daprClient = daprClient;
            this.clientOptions = clientOptions;
        }
        public async Task<TResponse> InvokeMethod(TRequest request, string requestUri, IDictionary<string, string>? parameters = null, CancellationToken cancellationToken = default)
        {
            try
            {
                string serializedRequest = JsonConvert.SerializeObject(request);
                var daprRequest = _daprClient.CreateInvokeMethodRequest(clientOptions.ApplicationId, requestUri, request);
                foreach (var header in clientOptions.Headers)
                {
                    daprRequest.Headers.Add(header.HeaderName, header.HeaderValue);
                }

                return await _daprClient.InvokeMethodAsync<TResponse>(daprRequest, cancellationToken);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
