// Copyright 2020-present Etherna SA
// This file is part of DomainEvents.
// 
// DomainEvents is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// DomainEvents is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with DomainEvents.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.DomainEvents.Modifiers;
using Etherna.ExecContext;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etherna.DomainEvents
{
    public class EventDispatcher(
        IExecutionContext executionContext,
        IServiceProvider serviceProvider)
        : IEventDispatcher
    {
        // Fields.
        private readonly Dictionary<Type, List<Type>> eventHandlerTypes = new(); //EventType -> HandlerType[]

        // Properties.
        public bool IsEventDispatchDisabled =>
            EventDispatcherModifier.IsEventDispatchDisabled(executionContext);

        // Methods.
        public void AddHandler<THandler>() where THandler : IEventHandler =>
            AddHandler(typeof(THandler));

        public void AddHandler(Type handlerType)
        {
            ArgumentNullException.ThrowIfNull(handlerType, nameof(handlerType));

            var eventType = handlerType.GetInterfaces()
                           .Where(i => i.IsGenericType)
                           .Single(i => i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                           .GenericTypeArguments[0];

            if (!eventHandlerTypes.ContainsKey(eventType))
                eventHandlerTypes[eventType] = new List<Type>();

            eventHandlerTypes[eventType].Add(handlerType);
        }

        public IDisposable DisableEventDispatch() =>
            new EventDispatcherModifier(executionContext);

        public async Task DispatchAsync(IDomainEvent @event)
        {
            ArgumentNullException.ThrowIfNull(@event, nameof(@event));

            // Verify pre-conditions.
            if (EventDispatcherModifier.IsEventDispatchDisabled(executionContext))
                return;

            var eventType = @event.GetType();
            if (!eventHandlerTypes.TryGetValue(eventType, out var handlerTypes))
                return;

            // Create scope.
            using var scope = serviceProvider.CreateScope();
            foreach (var handlerType in handlerTypes)
            {
                var handler = (IEventHandler)scope.ServiceProvider.GetService(handlerType)!;
                await handler.HandleAsync(@event).ConfigureAwait(false);
            }
        }

        public async Task DispatchAsync(IEnumerable<IDomainEvent> events)
        {
            ArgumentNullException.ThrowIfNull(events, nameof(events));

            foreach (var @event in events)
                await DispatchAsync(@event).ConfigureAwait(false);
        }
    }
}
