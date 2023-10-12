namespace PA.QuoteSubmission.SharedKernel.Interfaces
{
    public interface IPublishService<T> where T : class
    {
        Task Publish(T message, CancellationToken cancellationToken = default);
    }
}
