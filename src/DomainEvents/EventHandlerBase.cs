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
    public abstract class EventHandlerBase<TEvent> : IEventHandler<TEvent> where
        TEvent : class, IDomainEvent
    {
        public Type HandledType => typeof(TEvent);

        public abstract Task HandleAsync(TEvent @event);

        public Task HandleAsync(IDomainEvent @event)
        {
            if (@event is not TEvent)
                throw new ArgumentException($"Event must be of type {typeof(TEvent)}");

            return HandleAsync((TEvent)@event);
        }
    }
}
