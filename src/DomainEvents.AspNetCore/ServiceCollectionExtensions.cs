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

using Etherna.ExecContext;
using Etherna.ExecContext.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.DomainEvents.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static IServiceCollection AddDomainEvents(
            this IServiceCollection services,
            IEnumerable<Type> eventHandlerTypes)
        {
            ArgumentNullException.ThrowIfNull(eventHandlerTypes, nameof(eventHandlerTypes));

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

                //subscribe handlers to dispatcher
                foreach (var handlerType in eventHandlerTypes)
                    dispatcher.AddHandler(handlerType);

                return dispatcher;
            });

            return services;
        }
    }
}
