using Castle.Core.Logging;
using Dapr.Client;
using DurableTask.Core;
using Microsoft.Extensions.Logging;
using PA.QuoteSubmission.Infrastructure.Model;
using PA.QuoteSubmission.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Infrastructure.Services
{
    public class PublishNotificationService : IPublishService<NotificationRequest>
    {
        private readonly DaprClient daprClient;
        private readonly ILogger<PublishNotificationService> logger;
        private readonly PublishOptions options;
        public PublishNotificationService(DaprClient daprClient, PublishOptions options, ILogger<PublishNotificationService> logger)
        {
            this.daprClient = daprClient;
            this.logger = logger;
            this.options = options;
        }

       

        public async Task Publish(NotificationRequest message, CancellationToken cancellationToken = default)
        {
            await daprClient.PublishEventAsync(options.PubSubName, options.TopicName, message);

            
        }
    }
}
