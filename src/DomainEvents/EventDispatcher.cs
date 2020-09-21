//   Copyright 2020-present Etherna Sagl
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etherna.DomainEvents
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
            if (handlerType is null)
                throw new ArgumentNullException(nameof(handlerType));

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
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            var eventType = @event.GetType();
            if (!eventHandlerTypes.ContainsKey(eventType))
                return;

            // Create scope.
            using var scope = serviceProvider.CreateScope();

            // Process handlers.
            var handlerTypes = eventHandlerTypes[eventType];
            foreach (var handlerType in handlerTypes)
            {
                var handler = (IEventHandler)scope.ServiceProvider.GetService(handlerType);
                await handler.HandleAsync(@event).ConfigureAwait(false);
            }
        }

        public async Task DispatchAsync(IEnumerable<IDomainEvent> events)
        {
            if (events is null)
                throw new ArgumentNullException(nameof(events));

            foreach (var @event in events)
                await DispatchAsync(@event).ConfigureAwait(false);
        }
    }
}
