namespace PA.QuoteSubmission.Infrastructure.Model
{
    public class ServiceClientOptions
    {
        public string ApplicationId;

        public string Path;

        public Header[] Headers;
    }

    public class Header { 
        public string HeaderName { get; set; }
        public string HeaderValue { get; set; }
    }
}
