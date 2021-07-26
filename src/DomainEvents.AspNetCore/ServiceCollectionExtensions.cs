using Etherna.ExecContext;
using Etherna.ExecContext.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;

namespace Etherna.DomainEvents.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainEvents(
            this IServiceCollection services,
            IEnumerable<Type> eventHandlerTypes)
        {
            if (eventHandlerTypes is null)
                throw new ArgumentNullException(nameof(eventHandlerTypes));

            // External dependencies.
            services.AddExecutionContext();

            // Event handlers.
            foreach (var handlerType in eventHandlerTypes)
                services.TryAddScoped(handlerType);

            // Dispatcher.
            services.TryAddSingleton<IEventDispatcher>(sp =>
            {
                var executionContext = sp.GetRequiredService<IExecutionContext>();
                var dispatcher = new EventDispatcher(executionContext, sp);

                //subscrive handlers to dispatcher
                foreach (var handlerType in eventHandlerTypes)
                    dispatcher.AddHandler(handlerType);

                return dispatcher;
            });

            return services;
        }
    }
}
