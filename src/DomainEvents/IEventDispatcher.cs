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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etherna.DomainEvents
{
    public interface IEventDispatcher
    {
        bool IsEventDispatchDisabled { get; }

        void AddHandler<THandler>()
            where THandler : IEventHandler;

        void AddHandler(Type handlerType);

        IDisposable DisableEventDispatch();

        Task DispatchAsync(IDomainEvent @event);

        Task DispatchAsync(IEnumerable<IDomainEvent> events);
    }
}
