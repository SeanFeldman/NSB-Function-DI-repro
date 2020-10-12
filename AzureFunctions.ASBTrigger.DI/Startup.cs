using System;
using AzureFunctions.ASBTrigger.DI;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus;

[assembly: FunctionsStartup(typeof(Startup))]

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var services = builder.Services;

        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json")
            .AddUserSecrets<Startup>()
            .AddEnvironmentVariables()
            .Build();
        // services.AddSingleton<IConfiguration>(configurationRoot);

        // services.AddScoped(typeof(MyService));
        // services.AddScoped<IMyService>(sp => sp.GetRequiredService<MyService>());

        // services.AddDbContext<MyDbContext>(delegate(DbContextOptionsBuilder options)
        // {
        //     var connectionString = configurationRoot.GetConnectionString("MyDbConnectionString");
        //     options.UseSqlServer(connectionString);
        // });

        services.AddSingleton(sp => new FunctionEndpoint(executionContext =>
        {
            var configuration = ServiceBusTriggeredEndpointConfiguration.FromAttributes();

            configuration.UseSerialization<NewtonsoftSerializer>();

            configuration.LogDiagnostics();

            var containerSettings = configuration.AdvancedConfiguration.UseContainer(new DefaultServiceProviderFactory());
            var serviceCollection = containerSettings.ServiceCollection;

            serviceCollection.AddDbContext<MyDbContext>(delegate (DbContextOptionsBuilder options)
            {
                var connectionString = configurationRoot.GetConnectionString("MyDbConnectionString");
                options.UseSqlServer(connectionString);
            });

            serviceCollection.AddScoped(typeof(MyService));
            serviceCollection.AddScoped<IMyService>(provider => provider.GetRequiredService<MyService>());

            return configuration;
        }));
    }
}

public class MyService : IMyService
{
    public string SayHello() => "Hello!";
}

public interface IMyService
{
    string SayHello();
}