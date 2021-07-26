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
            if (context is null)
                throw new ArgumentNullException(nameof(context));
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
            if (context is null)
                throw new ArgumentNullException(nameof(context));
            if (context.Items is null)
                throw new ExecutionContextNotFoundException();

            if (!context.Items.ContainsKey(ModifierKey))
                return false;
            var requests = (ICollection<EventDispatcherModifier>)context.Items[ModifierKey]!;

            lock (((ICollection)requests).SyncRoot)
                return requests.Any();
        }
    }
}
