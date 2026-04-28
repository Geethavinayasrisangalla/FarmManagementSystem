namespace FarmManagement.Web.Events;


public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _provider;

    public EventDispatcher(IServiceProvider provider) => _provider = provider;

    public async Task DispatchAsync<T>(T domainEvent) where T : IDomainEvent
    {
        var handlers = _provider.GetServices<IEventHandler<T>>();
        foreach (var handler in handlers)
            await handler.HandleAsync(domainEvent);
    }
}
