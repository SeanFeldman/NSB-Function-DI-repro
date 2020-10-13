using System;
using AzureFunctions.ASBTrigger.DI;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NServiceBus;

[assembly: FunctionsStartup(typeof(Startup))]

public class Startup : FunctionsStartup
{
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        var context = builder.GetContext();

        builder.ConfigurationBuilder
            .AddUserSecrets<Startup>()
            .AddEnvironmentVariables();
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        var services = builder.Services;

        services.AddScoped(typeof(MyService));
        services.AddScoped<IMyService>(sp => sp.GetRequiredService<MyService>());

        services.AddDbContext<MyDbContext>(options =>
        {
            var connectionString = builder.GetContext().Configuration.GetConnectionString("MyDbConnectionString");
            options.UseSqlServer(connectionString);
        });

        services.AddSingleton(sp => new FunctionEndpoint(executionContext =>
        {
            var configuration = ServiceBusTriggeredEndpointConfiguration.FromAttributes();

            configuration.UseSerialization<NewtonsoftSerializer>();

            configuration.LogDiagnostics();

            var containerSettings =
                configuration.AdvancedConfiguration.UseContainer(new DefaultServiceProviderFactory());
            // var serviceCollection = containerSettings.ServiceCollection;
            containerSettings.ServiceCollection.Add(services);

            // serviceCollection.AddDbContext<MyDbContext>(delegate (DbContextOptionsBuilder options)
            // {
            //     var connectionString = configurationRoot.GetConnectionString("MyDbConnectionString");
            //     options.UseSqlServer(connectionString);
            // });
            //
            // serviceCollection.AddScoped(typeof(MyService));
            // serviceCollection.AddScoped<IMyService>(provider => provider.GetRequiredService<MyService>());

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