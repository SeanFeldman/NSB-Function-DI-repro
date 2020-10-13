using AzureFunctions.ServiceCollectionWrapper;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(StartupWithWrapper))]

public class StartupWithWrapper : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var services = builder.Services;

        services.AddScoped<MyService>();
        services.AddScoped<IMyService>(sp => sp.GetRequiredService<MyService>());

        services.AddDbContext<MyDbContext>(options =>
        {
            var connectionString = builder.GetContext().Configuration.GetConnectionString("MyDbConnectionString");
            options.UseSqlServer(connectionString);
        });

        services.AddSingleton(new ServicesWrapper {ServiceCollection = services});
    }
}

public class ServicesWrapper
{
    public IServiceCollection ServiceCollection { get; set; }
}

public class MyService : IMyService
{
    public string SayHello() => "Hello!";
}

public interface IMyService
{
    string SayHello();
}