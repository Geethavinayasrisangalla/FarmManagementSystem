namespace FarmManagement.Web.Events;


public interface IEventHandler<T> where T : IDomainEvent
{
    Task HandleAsync(T domainEvent);
}
