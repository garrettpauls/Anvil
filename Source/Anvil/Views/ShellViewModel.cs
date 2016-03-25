using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Anvil.Framework.ComponentModel;
using Anvil.Framework.Reactive;
using Anvil.Views.ConfigurationUI;

using Autofac.Extras.NLog;

using ReactiveUI;

using Squirrel;

namespace Anvil.Views
{
    public sealed class ShellViewModel : DisposableReactiveObject, IScreen
    {
        private readonly Func<SettingsViewModel> mSettingsViewModelFactory;
        private UpdateViewModel mUpdateViewModel;

        public ShellViewModel(RoutingState router, Func<SettingsViewModel> settingsViewModelFactory, Func<IUpdateManager> updateManagerFactory, ILogger log)
        {
            mSettingsViewModelFactory = settingsViewModelFactory;
            Router = router;

            var canShowSettings = Router.CurrentViewModel.Select(vm => !(vm is SettingsViewModel));
            ShowSettingsCommand = ReactiveCommand.Create(canShowSettings);
            ShowSettingsCommand.Subscribe(_ShowSettings).TrackWith(Disposables);

            this.ObservableForProperty(x => x.UpdateViewModel)
                .SelectMany(x => x.Value.WhenAnyValue(y => y.IsUpdateAvailable, y => y.IsUpdateCompleted))
                .Subscribe(_ =>
                {
                    this.RaisePropertyChanged(nameof(IsUpdateAvailable));
                    this.RaisePropertyChanged(nameof(IsUpdateCompleted));
                })
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

        public bool IsUpdateCompleted => UpdateViewModel?.IsUpdateCompleted ?? false;

        public RoutingState Router { get; }

        public ReactiveCommand<object> ShowSettingsCommand { get; }

        public UpdateViewModel UpdateViewModel
        {
            get { return mUpdateViewModel; }
            private set { this.RaiseAndSetIfChanged(ref mUpdateViewModel, value); }
        }

        private void _ShowSettings(object _)
        {
            Router.Navigate.Execute(mSettingsViewModelFactory());
        }
    }
}
