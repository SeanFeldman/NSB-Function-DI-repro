using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.ServiceCollectionWrapper
{
    public class AzureServiceBusNativeTriggeredFunctionWithWrapper
    {
        private readonly ServicesWrapper wrapper;
        private readonly IServiceProvider serviceProvider;

        public AzureServiceBusNativeTriggeredFunctionWithWrapper(ServicesWrapper wrapper, IServiceProvider serviceProvider)
        {
            this.wrapper = wrapper;
            this.serviceProvider = serviceProvider;
        }

        [FunctionName(nameof(AzureServiceBusNativeTriggeredFunctionWithWrapper))]
        public async Task Run([ServiceBusTrigger("asbtriggerqueue", Connection = "AzureWebJobsServiceBus")] Message message, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {Encoding.UTF8.GetString(message.Body)}");

            // comment this out to fix the issue (will be using the injected service provider)
            var serviceProvider = wrapper.ServiceCollection.BuildServiceProvider();

            var myService = serviceProvider.GetService<IMyService>();
            log.LogInformation($"MyService says: {myService.SayHello()}");

            var myDbContext = serviceProvider.GetService<MyDbContext>();

            var any = await myDbContext.Users.AnyAsync();
            log.LogInformation($"Found any users: {any}");
        }
    }
}