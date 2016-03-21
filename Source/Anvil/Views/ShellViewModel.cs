using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Anvil.Framework.ComponentModel;
using Anvil.Framework.Reactive;

using Autofac.Extras.NLog;

using ReactiveUI;

using Squirrel;

namespace Anvil.Views
{
    public sealed class ShellViewModel : DisposableReactiveObject, IScreen
    {
        private UpdateViewModel mUpdateViewModel;

        public ShellViewModel(RoutingState router, Func<IUpdateManager> updateManagerFactory, ILogger log)
        {
            Router = router;

            this.ObservableForProperty(x => x.UpdateViewModel)
                .SelectMany(x => x.Value.WhenAnyValue(y => y.IsUpdateAvailable))
                .Subscribe(_ => this.RaisePropertyChanged(nameof(IsUpdateAvailable)))
                .TrackWith(Disposables);

            Task.Factory
                .StartNew(updateManagerFactory)
                .ContinueWith(task =>
                {
                    if(task.Exception != null)
                    {
                        log.Error("Failed to create update manager", task.Exception);
                    }
                    else
                    {
                        UpdateViewModel = new UpdateViewModel(task.Result, this, log);
                        UpdateViewModel.CheckForUpdatesCommand.ExecuteAsyncTask();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public bool IsUpdateAvailable => UpdateViewModel?.IsUpdateAvailable ?? false;

        public RoutingState Router { get; }

        public UpdateViewModel UpdateViewModel
        {
            get { return mUpdateViewModel; }
            private set { this.RaiseAndSetIfChanged(ref mUpdateViewModel, value); }
        }
    }
}
