using System.Threading.Tasks;
using AzureFunctions.ASBTrigger.DI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using NServiceBus;
using NServiceBus.Logging;

public class TriggerMessageHandler : IHandleMessages<TriggerMessage>
{
    private static readonly ILog Log = LogManager.GetLogger<TriggerMessageHandler>();
    private readonly IMyService myService;
    private readonly MyDbContext myDbContext;

    public TriggerMessageHandler(IMyService myService, MyDbContext myDbContext)
    {
        this.myService = myService;
        this.myDbContext = myDbContext;
    }

    public async Task Handle(TriggerMessage message, IMessageHandlerContext context)
    {
        Log.Warn($"Handling {nameof(TriggerMessage)} in {nameof(TriggerMessageHandler)}. Service says: {myService.SayHello()}");

        var any = await myDbContext.Users.AnyAsync();

        Log.Info($"Found any users: {any}");
    }
}