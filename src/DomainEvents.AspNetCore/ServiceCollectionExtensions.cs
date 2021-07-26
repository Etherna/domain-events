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
