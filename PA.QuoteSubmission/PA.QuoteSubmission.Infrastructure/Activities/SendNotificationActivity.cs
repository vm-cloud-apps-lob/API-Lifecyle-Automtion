using Castle.Core.Logging;
using Dapr.Client;
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
    public class SendNotificationActivity : WorkflowActivity<NotificationRequest, bool>
    {
        private readonly PublishNotificationService service;
        private readonly ILogger<SendNotificationActivity> logger;
        public SendNotificationActivity(PublishNotificationService service, ILogger<SendNotificationActivity> logger) 
        { 
            this.service = service;
            this.logger = logger;
        }
        public override async Task<bool> RunAsync(WorkflowActivityContext context, NotificationRequest input)
        {
            logger.LogInformation($"{context.Name} Activity is started");

            await service.Publish(input);

            logger.LogInformation($"{context.Name} Activity is Completed");

            return true;
        }
    }
}
