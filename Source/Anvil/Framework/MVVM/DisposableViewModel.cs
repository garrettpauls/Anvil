using System;

using Anvil.Framework.ComponentModel;

using ReactiveUI;

namespace Anvil.Framework.MVVM
{
    public abstract class DisposableViewModel : ReactiveObject, IDisposable
    {
        protected DisposableTracker Disposables { get; } = new DisposableTracker();

        public virtual void Dispose()
        {
            Disposables.Dispose();
        }
    }
}