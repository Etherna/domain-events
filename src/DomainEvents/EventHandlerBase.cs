using System;
using System.Threading.Tasks;

namespace Digicando.DomainEvents
{
    public abstract class EventHandlerBase<TEvent> : IEventHandler<TEvent> where
        TEvent : class, IDomainEvent
    {
        public Type HandledType => typeof(TEvent);

        public abstract Task HandleAsync(TEvent @event);

        public Task HandleAsync(IDomainEvent @event)
        {
            if (!(@event is TEvent))
                throw new ArgumentException($"Event must be of type {typeof(TEvent)}");

            return HandleAsync(@event as TEvent);
        }
    }
}
