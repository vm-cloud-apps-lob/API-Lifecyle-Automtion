using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PA.QuoteSubmission.Core.SubmissionAggregate;
using PA.QuoteSubmission.Infrastructure.Model;
using PA.QuoteSubmission.Infrastructure.Services;
using PA.QuoteSubmission.Infrastructure.Workflow;
using PA.QuoteSubmission.SharedKernel.DomainObjects;
using PA.QuoteSubmission.Web.Models;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace PA.QuoteSubmission.Web.Controllers
{
    [Route("pa/quote/v1")]
    [ApiController]
    public class QuoteSubmissionController : ControllerBase
    {
        private readonly DaprClient daprClient;
        private readonly ILogger<QuoteSubmissionController> logger;
        private readonly SubmissionEventStoreService storeService;
        public QuoteSubmissionController(DaprClient daprClient, ILogger<QuoteSubmissionController> logger, SubmissionEventStoreService storeService)
        {
            this.daprClient = daprClient;
            this.logger = logger;
            this.storeService = storeService;
        }

        [Route("submission")]
        [HttpPost]
        public async Task<SubmissionSuccessResponse> QuoteSubmission(Submission submissionRequest) {

            var instanceId = Guid.NewGuid().ToString();
            submissionRequest.SubmissionId = instanceId;

#pragma warning disable CS0618 // Type or member is obsolete
            StartWorkflowResponse response = await daprClient.StartWorkflowAsync(
                "dapr",
                nameof(SubmissionWorkflow),
                instanceId,
                submissionRequest);

            var resp = await daprClient.WaitForWorkflowCompletionAsync(instanceId, "dapr");
#pragma warning restore CS0618 // Type or member is obsolete
            logger.LogInformation($"state: {JsonSerializer.Serialize(resp.RuntimeStatus)}");
            
            var success = new SubmissionSuccessResponse
            {
                SubmissionId = instanceId,
                Status = "Submitted Successfully"
            };

            return success;
        }
    }
}
