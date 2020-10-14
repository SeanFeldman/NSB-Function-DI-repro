using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System.Threading.Tasks;

public class AzureServiceBusTriggerFunction
{
    internal const string EndpointName = "ASBTriggerQueue";

    public AzureServiceBusTriggerFunction(FunctionEndpoint endpoint)
    {
        this.endpoint = endpoint;
    }

    [FunctionName(EndpointName)]
    public async Task Run(
        [ServiceBusTrigger(queueName: EndpointName)]
        Message message,
        ILogger logger,
        ExecutionContext executionContext)
    {
        await endpoint.Process(message, executionContext, logger);
    }

    private readonly FunctionEndpoint endpoint;
}