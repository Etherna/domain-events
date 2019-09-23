using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Digicando.DomainEvents
{
    public class EventDispatcher : IEventDispatcher
    {
        // Fields.
        private readonly Dictionary<Type, List<Type>> eventHandlerTypes =
            new Dictionary<Type, List<Type>>(); //EventType -> HandlerType[]
        private readonly IServiceProvider serviceProvider;

        // Constructors.
        public EventDispatcher(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        // Methods.
        public void AddHandler<THandler>() where THandler : IEventHandler =>
            AddHandler(typeof(THandler));

        public void AddHandler(Type handlerType)
        {
            var eventType = handlerType.GetInterfaces()
                           .Where(i => i.IsGenericType)
                           .Single(i => i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                           .GenericTypeArguments[0];

            if (!eventHandlerTypes.ContainsKey(eventType))
                eventHandlerTypes[eventType] = new List<Type>();

            eventHandlerTypes[eventType].Add(handlerType);
        }

        public async Task DispatchAsync(IDomainEvent @event)
        {
            var eventType = @event.GetType();
            if (!eventHandlerTypes.ContainsKey(eventType))
                return;

            // Create scope.
            using (var scope = serviceProvider.CreateScope())
            {
                // Process handlers.
                var handlerTypes = eventHandlerTypes[eventType];
                foreach (var handlerType in handlerTypes)
                {
                    var handler = scope.ServiceProvider.GetService(handlerType) as IEventHandler;
                    await handler.HandleAsync(@event);
                }
            }
        }

        public async Task DispatchAsync(IEnumerable<IDomainEvent> events)
        {
            foreach (var @event in events)
                await DispatchAsync(@event);
        }
    }
}
