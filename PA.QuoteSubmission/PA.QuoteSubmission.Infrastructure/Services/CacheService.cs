using Dapr.Client;
using PA.QuoteSubmission.Infrastructure.Model;
using PA.QuoteSubmission.SharedKernel.Interfaces;
using System.Threading;

namespace PA.QuoteSubmission.Infrastructure.Services
{
    public abstract class CacheService<T> : ICacheService<T> where T : class
    {
        private readonly DaprClient _daprClient;
        private readonly CacheOptions cacheOptions;
        private readonly Dictionary<string, string> metadata;
        public CacheService(DaprClient _daprClient,CacheOptions cacheOptions) {
            this._daprClient= _daprClient;
            this.cacheOptions = cacheOptions;
            metadata = new Dictionary<string, string>();
            metadata.Add("metadata.ttlInSeconds", cacheOptions.TimeToLive);
        }
        public async void DeleteValue(string keyValue, CancellationToken cancellationToken = default)
        {
            await _daprClient.DeleteStateAsync(cacheOptions.StoreName, keyValue, cancellationToken: cancellationToken, metadata: metadata).ConfigureAwait(false);
        }

        public async Task<T> GetValue(string keyValue, CancellationToken cancellationToken = default)
        {
            return await _daprClient.GetStateAsync<T>(cacheOptions.StoreName, keyValue, cancellationToken: cancellationToken, metadata: metadata).ConfigureAwait(false);
        }

        public async void SaveValue(T value, string keyValue, CancellationToken cancellationToken = default)
        {
            await _daprClient.SaveStateAsync<T>(cacheOptions.StoreName, keyValue,value,cancellationToken: cancellationToken, metadata: metadata).ConfigureAwait(false);
        }

        public async void SaveValue(T value, string keyValue, string ttl, CancellationToken cancellationToken = default) {

            await _daprClient.SaveStateAsync<T>(cacheOptions.StoreName, keyValue, value, cancellationToken: cancellationToken, metadata: new Dictionary<string, string>() {
                {
                    "metadata.ttlInSeconds", ttl
                }
            }).ConfigureAwait(false);
        }

        public async Task SaveValueToPartitionAsync(T value, string keyValue, string partitionKey, CancellationToken cancellationToekn = default)
        {
            await _daprClient.SaveStateAsync<T>(cacheOptions.StoreName, keyValue, value, cancellationToken: default, metadata: new Dictionary<string, string>() {
                {
                    "metadata.partitionKey", partitionKey
                }
            });
        }
    }
}
