using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

using Anvil.Framework.ComponentModel;
using Anvil.Models;
using Anvil.Services;
using Anvil.Services.Data;
using Anvil.Views.ConfigurationUI;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

namespace Anvil.Views.Notification
{
    public sealed class TrayIconLaunchGroupTreeNode : LaunchGroupTreeNode<TrayIconLaunchGroupTreeNode>, ITrayIconMenuItem
    {
        private readonly ReadOnlyObservableCollection<ITrayIconMenuItem> mChildren;

        private string mName;

        public TrayIconLaunchGroupTreeNode(Node<LaunchGroup, long> node, IDataService dataService, ILaunchService launchService)
            : base(node, dataService, launchService, x => new TrayIconLaunchGroupTreeNode(x, dataService, launchService))
        {
            node.Item
                .WhenAnyValue(x => x.Name)
                .BindTo(this, x => x.Name)
                .TrackWith(Disposables);

            var groups = MutableChildGroups
                .ToObservableChangeSet().Transform(x => (ITrayIconMenuItem) x);
            var items = MutableChildItems
                .ToObservableChangeSet()
                .Transform(x => (ITrayIconMenuItem) new TrayIconLaunchItemTreeNode(x, launchService)).DisposeMany();

            groups.Or(items)
                  .Sort(SortExpressionComparer<ITrayIconMenuItem>.Descending(x => x.IsGroup).ThenByAscending(x => x.Name))
                  .ObserveOnDispatcher()
                  .Bind(out mChildren)
                  .Subscribe().TrackWith(Disposables);
        }

        public ReadOnlyObservableCollection<ITrayIconMenuItem> Children => mChildren;

        public bool IsGroup { get; } = true;

        IReactiveCommand<object> ITrayIconMenuItem.LaunchCommand { get; } = ReactiveCommand.Create();

        public string Name
        {
            get { return mName; }
            private set { this.RaiseAndSetIfChanged(ref mName, value); }
        }
    }
}
