namespace FarmManagement.Web.Events;


public interface IEventDispatcher
{
    Task DispatchAsync<T>(T domainEvent) where T : IDomainEvent;
}
