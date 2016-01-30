using System;
using System.Collections.ObjectModel;

using Anvil.Framework.ComponentModel;
using Anvil.Framework.MVVM;
using Anvil.Models;
using Anvil.Services.Data;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

namespace Anvil.Views.ConfigurationUI
{
    public sealed class MainConfigurationViewModel : RoutableViewModel
    {
        private readonly ReadOnlyObservableCollection<LaunchGroupTreeNode> mLaunchGroups;

        public MainConfigurationViewModel(IDataService dataService, IScreen hostScreen)
            : base("config/main", hostScreen)
        {
            dataService
                .LaunchGroups.Connect()
                .TransformToTree(grp => grp.ParentGroupId ?? -1)
                .Transform(x => new LaunchGroupTreeNode(x))
                .Sort(SortExpressionComparer<LaunchGroupTreeNode>.Ascending(x => x.Model.Name))
                .Bind(out mLaunchGroups).DisposeMany()
                .Subscribe().TrackWith(Disposables);
        }

        public ReadOnlyObservableCollection<LaunchGroupTreeNode> LaunchGroups => mLaunchGroups;
    }

    public sealed class LaunchGroupTreeNode : DisposableViewModel
    {
        private readonly ReadOnlyObservableCollection<LaunchGroupTreeNode> mChildGroups;
        private readonly Node<LaunchGroup, long> mNode;

        public LaunchGroupTreeNode(Node<LaunchGroup, long> node)
        {
            mNode = node;
            Model = node.Item;
            node.Children.Connect()
                .Transform(x => new LaunchGroupTreeNode(x))
                .Sort(SortExpressionComparer<LaunchGroupTreeNode>.Ascending(x => x.Model.Name))
                .Bind(out mChildGroups).DisposeMany()
                .Subscribe().TrackWith(Disposables);
        }

        public ReadOnlyObservableCollection<LaunchGroupTreeNode> ChildGroups => mChildGroups;

        public LaunchGroup Model { get; }
    }
}