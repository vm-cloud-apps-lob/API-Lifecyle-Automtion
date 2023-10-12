namespace PA.QuoteSubmission.SharedKernel.Interfaces
{
    /// <summary>
    /// TBD - Restrict to aggregate or entity???
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICacheService<T> where T : class
    {
        void SaveValue(T value, string keyValue, CancellationToken cancellationToken = default);

        Task<T> GetValue(string keyValue, CancellationToken cancellationToken = default);

        void DeleteValue(string keyValue, CancellationToken cancellationToken = default);

        void SaveValue(T value, string keyValue, string ttl, CancellationToken cancellationToken = default);

        Task SaveValueToPartitionAsync(T value, string keyValue, string partitionKey, CancellationToken cancellationToekn = default);
    }
}
