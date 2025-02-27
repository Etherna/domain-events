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
using Etherna.ExecContext.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.DomainEvents.Modifiers
{
    public class EventDispatcherModifier : IDisposable
    {
        // Consts.
        private const string ModifierKey = "EventDispatcherModifier";

        // Fields.
        private bool disposed;
        private readonly ICollection<EventDispatcherModifier> requests;

        // Constructor.
        public EventDispatcherModifier(IExecutionContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            if (context.Items is null)
                throw new ExecutionContextNotFoundException();

            if (!context.Items.ContainsKey(ModifierKey))
                context.Items.Add(ModifierKey, new List<EventDispatcherModifier>());

            requests = (ICollection<EventDispatcherModifier>)context.Items[ModifierKey]!;

            lock (((ICollection)requests).SyncRoot)
                requests.Add(this);
        }


        // Dispose.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                lock (((ICollection)requests).SyncRoot)
                    requests.Remove(this);
            }

            disposed = true;
        }

        // Static methods.
        public static bool IsEventDispatchDisabled(IExecutionContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            if (context.Items is null)
                throw new ExecutionContextNotFoundException();

            if (!context.Items.TryGetValue(ModifierKey, out var modRequests))
                return false;
            var requests = (ICollection<EventDispatcherModifier>)modRequests!;

            lock (((ICollection)requests).SyncRoot)
                return requests.Count != 0;
        }
    }
}
