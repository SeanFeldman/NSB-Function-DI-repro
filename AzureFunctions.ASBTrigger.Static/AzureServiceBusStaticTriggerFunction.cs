using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System.Threading.Tasks;
using AzureFunctions.ASBTrigger.DI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class AzureServiceBusStaticTriggerFunction
{
    private const string EndpointName = "ASBTriggerQueue";

    [FunctionName(EndpointName)]
    public async Task Run(
        [ServiceBusTrigger(queueName: EndpointName)]
        Message message,
        ILogger logger,
        ExecutionContext executionContext)
    {
        await endpoint.Process(message, executionContext, logger);
    }

    private static readonly FunctionEndpoint endpoint = new FunctionEndpoint(executionContext =>
    {
        var configuration = ServiceBusTriggeredEndpointConfiguration.FromAttributes();

        configuration.UseSerialization<NewtonsoftSerializer>();

        configuration.LogDiagnostics();

        var containerSettings = configuration.AdvancedConfiguration.UseContainer(new DefaultServiceProviderFactory());
        var serviceCollection = containerSettings.ServiceCollection;

        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json")
            .AddUserSecrets<AzureServiceBusStaticTriggerFunction>()
            .AddEnvironmentVariables()
            .Build();

        serviceCollection.AddDbContext<MyDbContext>(delegate (DbContextOptionsBuilder options)
        {
            var connectionString = configurationRoot.GetConnectionString("MyDbConnectionString");
            options.UseSqlServer(connectionString);
        });

        serviceCollection.AddScoped(typeof(MyService));
        serviceCollection.AddScoped<IMyService>(provider => provider.GetRequiredService<MyService>());

        return configuration;
    });
}

public class MyService : IMyService
{
    public string SayHello() => "Hello!";
}

public interface IMyService
{
    string SayHello();
}