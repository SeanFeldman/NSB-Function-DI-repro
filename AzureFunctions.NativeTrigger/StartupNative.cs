using System;
using AzureFunctions.NativeTrigger;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(StartupNative))]

public class StartupNative : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var services = builder.Services;

        services.AddDbContext<MyDbContext>(delegate(DbContextOptionsBuilder options)
        {
            var connectionString = builder.GetContext().Configuration.GetConnectionString("MyDbConnectionString");
            options.UseSqlServer(connectionString);
        });
    }
}