using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using ManagedIdentityDemo.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using System.Text.Json;


namespace ManagedIdentityDemo
{
    public class ManagedIdentityDemo
    {
        private readonly ILogger<ManagedIdentityDemo> _logger;

        public ManagedIdentityDemo(ILogger<ManagedIdentityDemo> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ManagedIdentityDemo))]
        public async Task Run(
            [ServiceBusTrigger("%servicebus_queue%", Connection = "servicebus")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            DefaultAzureCredential managedIdentity = new DefaultAzureCredential();
            string? environment = Environment.GetEnvironmentVariable("dataverse_url");

            _logger.LogInformation("Dataverse URL: {contentType}",environment);

            Account accountFromPayload = JsonSerializer.Deserialize<Account>(message.Body.ToString());

            string token = managedIdentity.GetTokenAsync(new TokenRequestContext(new[] { $"{environment}/.default" })).Result.Token;

            DataverseClient dataverseClient = new DataverseClient(token, environment);

            IOrganizationService serviceClient = dataverseClient.getOrganizationService();

            Entity accountToCreate = new Entity("account");
            accountToCreate["name"] = accountFromPayload?.AccountName;
            accountToCreate["emailaddress1"] = accountFromPayload?.Email;

            Guid accountId = serviceClient.Create(accountToCreate);

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
