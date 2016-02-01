using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

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

        private LaunchGroupTreeNode mSelectedGroup;

        public MainConfigurationViewModel(IDataService dataService, IScreen hostScreen)
            : base("config/main", hostScreen)
        {
            dataService
                .LaunchGroups.Connect()
                .ObserveOnDispatcher()
                .TransformToTree(grp => grp.ParentGroupId ?? -1)
                .Transform(x => new LaunchGroupTreeNode(x, dataService))
                .Sort(SortExpressionComparer<LaunchGroupTreeNode>.Ascending(x => x.Model.Name))
                .Bind(out mLaunchGroups).DisposeMany()
                .Subscribe().TrackWith(Disposables);

            var firstGroup = mLaunchGroups.FirstOrDefault();
            if(firstGroup != null)
            {
                firstGroup.IsSelected = true;
            }
        }

        public ReadOnlyObservableCollection<LaunchGroupTreeNode> LaunchGroups => mLaunchGroups;

        public LaunchGroupTreeNode SelectedGroup
        {
            get { return mSelectedGroup; }
            set { this.RaiseAndSetIfChanged(ref mSelectedGroup, value); }
        }
    }

    public sealed class LaunchGroupTreeNode : DisposableViewModel
    {
        private readonly ReadOnlyObservableCollection<LaunchGroupTreeNode> mChildGroups;
        private readonly IDataService mDataService;
        private readonly Node<LaunchGroup, long> mNode;
        private bool mIsExpanded;
        private bool mIsSelected;

        public LaunchGroupTreeNode(Node<LaunchGroup, long> node, IDataService dataService)
        {
            mNode = node;
            mDataService = dataService;
            Model = node.Item;
            node.Children.Connect()
                .ObserveOnDispatcher()
                .Transform(x => new LaunchGroupTreeNode(x, dataService))
                .Sort(SortExpressionComparer<LaunchGroupTreeNode>.Ascending(x => x.Model.Name))
                .Bind(out mChildGroups).DisposeMany()
                .Subscribe().TrackWith(Disposables);

            Environment = new EnvironmentEditViewModel(
                dataService.GetEnvironmentVariablesFor(node.Item).Connect(),
                envVar => dataService.AddEnvironmentVariable(node.Item, envVar),
                envVar => dataService.DeleteEnvironmentVariable(node.Item, envVar));

            AddGroupCommand = ReactiveCommand.Create();
            AddGroupCommand.Subscribe(_AddGroup).TrackWith(Disposables);

            IsSelected = true;
            IsExpanded = true;
        }

        public ReactiveCommand<object> AddGroupCommand { get; }

        public ReadOnlyObservableCollection<LaunchGroupTreeNode> ChildGroups => mChildGroups;

        public EnvironmentEditViewModel Environment { get; }

        public bool IsExpanded
        {
            get { return mIsExpanded; }
            set { this.RaiseAndSetIfChanged(ref mIsExpanded, value); }
        }

        public bool IsSelected
        {
            get { return mIsSelected; }
            set { this.RaiseAndSetIfChanged(ref mIsSelected, value); }
        }

        public LaunchGroup Model { get; }

        private void _AddGroup(object _)
        {
            mDataService.AddLaunchGroupAsync(new LaunchGroup
            {
                Name = "New group",
                ParentGroupId = Model.Id
            });
        }
    }
}