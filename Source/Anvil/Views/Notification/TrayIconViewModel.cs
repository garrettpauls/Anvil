using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows;

using Anvil.Framework.ComponentModel;
using Anvil.Framework.MVVM;
using Anvil.Services;
using Anvil.Services.Data;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

namespace Anvil.Views.Notification
{
    public sealed class TrayIconViewModel : DisposableViewModel
    {
        private readonly ReadOnlyObservableCollection<TrayIconLaunchGroupTreeNode> mLaunchGroups;

        public TrayIconViewModel(IDataService dataService, ILaunchService launchService)
        {
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_Exit).TrackWith(Disposables);

            ShowMainWindowCommand = ReactiveCommand.Create();
            ShowMainWindowCommand.Subscribe(_ShowMainWindow).TrackWith(Disposables);

            dataService
                .LaunchGroups.Connect()
                .ObserveOnDispatcher()
                .TransformToTree(grp => grp.ParentGroupId ?? -1)
                .Transform(grp => new TrayIconLaunchGroupTreeNode(grp, dataService, launchService))
                .Sort(SortExpressionComparer<TrayIconLaunchGroupTreeNode>.Ascending(x => x.Model.Name))
                .Bind(out mLaunchGroups).DisposeMany()
                .Subscribe().TrackWith(Disposables);
        }

        public ReactiveCommand<object> ExitCommand { get; }

        public ReadOnlyObservableCollection<TrayIconLaunchGroupTreeNode> LaunchGroups => mLaunchGroups;

        public ReactiveCommand<object> ShowMainWindowCommand { get; }

        private static void _Exit(object _)
        {
            Application.Current.Shutdown();
        }

        private static void _ShowMainWindow(object _)
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.Activate();
        }
    }
}