using System;
using System.Collections.ObjectModel;

using Anvil.Framework.ComponentModel;
using Anvil.Framework.Reactive;
using Anvil.Models;
using Anvil.Services;

using ReactiveUI;

namespace Anvil.Views.Notification
{
    public sealed class TrayIconLaunchItemTreeNode : DisposableReactiveObject, ITrayIconMenuItem
    {
        private readonly LaunchItem mItem;
        private readonly ILaunchService mLaunchService;
        private string mName;

        public TrayIconLaunchItemTreeNode(LaunchItem item, ILaunchService launchService)
        {
            mItem = item;

            mLaunchService = launchService;
            item.WhenAnyValue(x => x.Name)
                .BindTo(this, x => x.Name)
                .TrackWith(Disposables);

            LaunchCommand = ReactiveCommand.Create();
            LaunchCommand.Subscribe(_Launch).TrackWith(Disposables);
        }

        public ReadOnlyObservableCollection<ITrayIconMenuItem> Children { get; } = new ReadOnlyObservableCollection<ITrayIconMenuItem>(new ObservableCollection<ITrayIconMenuItem>());

        public bool IsGroup { get; } = false;

        public IReactiveCommand<object> LaunchCommand { get; }

        public string Name
        {
            get { return mName; }
            private set { this.RaiseAndSetIfChanged(ref mName, value); }
        }

        private void _Launch(object _)
        {
            mLaunchService.Launch(mItem);
        }
    }
}
