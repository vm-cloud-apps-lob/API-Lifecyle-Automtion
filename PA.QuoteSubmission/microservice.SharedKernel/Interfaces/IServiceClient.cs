namespace PA.QuoteSubmission.SharedKernel.Interfaces
{
    public interface IServiceClient<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        Task<TResponse> InvokeMethod(TRequest request, string requestUri, IDictionary<string, string>? parameters = null, CancellationToken cancellationToken = default);
    }
}
