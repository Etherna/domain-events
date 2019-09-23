using System;
using System.Threading.Tasks;

namespace Digicando.DomainEvents
{
    public interface IEventHandler
    {
        Type HandledType { get; }

        Task HandleAsync(IDomainEvent @event);
    }

    public interface IEventHandler<TEvent> : IEventHandler where
        TEvent : class, IDomainEvent
    {
        Task HandleAsync(TEvent @event);
    }
}
