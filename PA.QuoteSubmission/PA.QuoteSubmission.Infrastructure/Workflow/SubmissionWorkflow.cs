using Dapr.Workflow;
using PA.QuoteSubmission.Core.SubmissionAggregate;
using PA.QuoteSubmission.Core.SubmissionAggregate.Entities;
using PA.QuoteSubmission.Core.SubmissionAggregate.ValueObject;
using PA.QuoteSubmission.Infrastructure.Activities;
using PA.QuoteSubmission.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PA.QuoteSubmission.Infrastructure.Workflow
{
    public class SubmissionWorkflow : Workflow<Submission, object>
    {
        public override async Task<object> RunAsync(WorkflowContext context, Submission input)
        {
            var id = context.InstanceId;
            input.Id = id;
            EventMessage saveEvent = new EventMessage 
            { 
                EventId = Guid.NewGuid().ToString(),
                EventName = "SaveSubmissionInput",
                Submission = input 
            };   
            await context.CallActivityAsync(nameof(SaveEventStoreActivity), saveEvent);

            context.SetCustomStatus("Submission Payload saved to event store");

            var clearanceResult = await context.CallActivityAsync<bool>(nameof(SubmissionClearanceActivity), input);

            if (clearanceResult)
            {
                AgentValidationRequest req = new AgentValidationRequest
                {
                    ContractNumber = input.Agent.ContractNumber,
                    ProducerRoleCode = input.Agent.ProducerRoleCode,
                    ProducerSubCode = input.Agent.ProducerSubCode,
                    StateProducerId = input.Agent.StateProducerId
                };

                var isAgentValid = await context.CallActivityAsync<bool>(nameof(AgentValidationActivity), req);

                if (isAgentValid)
                {
                    var stateCovRefData = await context.CallActivityAsync<IEnumerable<CoverageRefData>>(nameof(RetrieveAndCacheReferenceDataActivity), input.RiskState);
                    input = this.EnrichRefData(stateCovRefData, input);
                    var gwRequest = await context.CallActivityAsync<GuidewireSubmissionRequest>(nameof(GuidewireRequestMappingActivity), input);

                    var isSuccess = await context.CallActivityAsync<bool>(nameof(SubmitToGuidewareActivity),gwRequest);
                    if (isSuccess)
                    {
                        this.SendSuccessNotification(context, input.PrimaryInsured.Address, input.SubmissionId);
                    }
                    else
                    {
                        this.SendFailureNotification(context, "Failed to submit to Guidewire", input.PrimaryInsured.Address, input.SubmissionId);
                    }
                }
                else
                {
                    this.SendFailureNotification(context, "Invalid Agent details provided", input.PrimaryInsured.Address, input.SubmissionId);
                }
            }
            else
            {
                this.SendFailureNotification(context, "Failed in Submission Clearance", input.PrimaryInsured.Address, input.SubmissionId);
            }
            return Task.FromResult<object>(null);
        }

        private async void SendSuccessNotification(WorkflowContext contect, Address address, string submissionId)
        {
            string addressStr = JsonSerializer.Serialize(address);

            List<string> senderList = new List<string>();
            senderList.Add(addressStr);

            NotificationRequest request = new NotificationRequest
            {
                ProcessStatus = "SUCCESS",
                SubmissionId = submissionId,
                SenderList = senderList,
                Message = "Submission is completed"
            };
        }
        private async void SendFailureNotification(WorkflowContext context, string message, Address address, string submissionId)
        {
            string addressStr = JsonSerializer.Serialize(address);

            List<string> senderList = new List<string>();
            senderList.Add(addressStr);

            NotificationRequest request = new NotificationRequest
            {
                ProcessStatus = "FAILURE",
                SubmissionId = submissionId,
                SenderList = senderList,
                Message = message

            };
            await context.CallActivityAsync(nameof(SendNotificationActivity),request);
        }

        private Submission EnrichRefData(IEnumerable<CoverageRefData> covRefData, Submission submission)
        {
            return submission;
        }
    }
}
