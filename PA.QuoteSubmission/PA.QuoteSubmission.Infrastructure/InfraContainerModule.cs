using Autofac;
using Dapr.Client;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using PA.QuoteSubmission.Infrastructure.Model;
using PA.QuoteSubmission.Infrastructure.Services;
using PA.QuoteSubmission.SharedKernel;
using System.Reflection;

namespace PA.QuoteSubmission.Infrastructure
{
    public class InfraContainerModule : AssemblyScanModule
    {
        protected override Assembly Assembly => Assembly.GetExecutingAssembly();
        private readonly CacheOptions submissionCacheOptions;
        private readonly IConfigurationSection RefDataOptions;
        public InfraContainerModule(CacheOptions submissionCacheOptions, IConfigurationSection RefDataOptions)
        {
            this.submissionCacheOptions = submissionCacheOptions;
            this.RefDataOptions = RefDataOptions;
        }
        
         protected override void Load(Autofac.ContainerBuilder builder)
         {
            builder.Register(c => new SubmissionEventStoreService(c.Resolve<DaprClient>(), submissionCacheOptions));
            builder.Register(c => new ReferenceDataService(RefDataOptions.Get<RefDataOptions>(), c.Resolve<DaprClient>()));
        }

    }
}
