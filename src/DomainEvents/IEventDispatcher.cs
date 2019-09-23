using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Digicando.DomainEvents
{
    public interface IEventDispatcher
    {
        void AddHandler<THandler>()
            where THandler : IEventHandler;

        void AddHandler(Type handlerType);

        Task DispatchAsync(IDomainEvent @event);

        Task DispatchAsync(IEnumerable<IDomainEvent> events);
    }
}
