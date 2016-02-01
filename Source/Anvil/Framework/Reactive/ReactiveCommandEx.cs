using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

using ReactiveUI;

namespace Anvil.Framework.Reactive
{
    public static class ReactiveCommandEx
    {
        public static ReactiveCommand<TParameter> Create<TParameter>(
            IObservable<bool> canExecute = null,
            IScheduler scheduler = null)
            where TParameter : class
        {
            canExecute = canExecute ?? Observable.Return(true);
            return new ReactiveCommand<TParameter>(
                canExecute, x => Observable.Return(x as TParameter), scheduler);
        }
    }
}