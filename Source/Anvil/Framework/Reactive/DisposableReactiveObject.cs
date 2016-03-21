using System;

using Anvil.Framework.ComponentModel;

using ReactiveUI;

namespace Anvil.Framework.Reactive
{
    public abstract class DisposableReactiveObject : ReactiveObject, IDisposable
    {
        protected DisposableTracker Disposables { get; } = new DisposableTracker();

        public void Dispose()
        {
            Disposables.Dispose();
            Dispose(true);
        }

        protected virtual void Dispose(bool isDisposing)
        {
        }
    }
}
