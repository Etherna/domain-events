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
