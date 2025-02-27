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
using System.Threading.Tasks;

namespace Etherna.DomainEvents
{
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
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
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
}
