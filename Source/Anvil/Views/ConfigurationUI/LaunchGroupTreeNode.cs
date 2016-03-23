using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

using Anvil.Framework.ComponentModel;
using Anvil.Framework.MVVM;
using Anvil.Framework.Reactive;
using Anvil.Models;
using Anvil.Services;
using Anvil.Services.Data;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

namespace Anvil.Views.ConfigurationUI
{
    public abstract class LaunchGroupTreeNode<TSelf> : DisposableViewModel
        where TSelf : LaunchGroupTreeNode<TSelf>
    {
        private readonly ObservableCollectionExtended<TSelf> mChildGroups = new ObservableCollectionExtended<TSelf>();
        private readonly ObservableCollectionExtended<LaunchItem> mChildItems = new ObservableCollectionExtended<LaunchItem>();

        public LaunchGroupTreeNode(
            Node<LaunchGroup, long> node,
            IDataService dataService,
            ILaunchService launchService,
            Func<Node<LaunchGroup, long>, TSelf> createChildNode)
        {
            Model = node.Item;

            LaunchService = launchService;

            LaunchCommand = ReactiveCommandEx.Create<LaunchItem>();
            LaunchCommand.Subscribe(_Launch).TrackWith(Disposables);

            node.Children.Connect()
                .ObserveOnDispatcher()
                .Transform(createChildNode)
                .Sort(SortExpressionComparer<TSelf>.Ascending(x => x.Model.Name))
                .Bind(mChildGroups).DisposeMany()
                .Subscribe().TrackWith(Disposables);

            dataService
                .LaunchItems.Connect()
                .ObserveOnDispatcher()
                .Filter(item => item.ParentGroupId == Model.Id)
                .Sort(SortExpressionComparer<LaunchItem>.Ascending(x => x.Name))
                .Bind(mChildItems)
                .Subscribe().TrackWith(Disposables);

            ChildGroups = new ReadOnlyObservableCollection<TSelf>(mChildGroups);
            ChildItems = new ReadOnlyObservableCollection<LaunchItem>(mChildItems);
        }

        public ReadOnlyObservableCollection<TSelf> ChildGroups { get; }

        public ReadOnlyObservableCollection<LaunchItem> ChildItems { get; }

        public ReactiveCommand<LaunchItem> LaunchCommand { get; }

        protected ILaunchService LaunchService { get; }

        public LaunchGroup Model { get; }

        protected ObservableCollectionExtended<TSelf> MutableChildGroups => mChildGroups;

        protected ObservableCollectionExtended<LaunchItem> MutableChildItems => mChildItems;

        private void _Launch(LaunchItem item)
        {
            LaunchService.Launch(item);
        }
    }
}
