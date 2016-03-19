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
    public sealed class LaunchGroupTreeNode : DisposableViewModel
    {
        private readonly ReadOnlyObservableCollection<LaunchGroupTreeNode> mChildGroups;
        private readonly ReadOnlyObservableCollection<LaunchItem> mChildItems;
        private readonly IDataService mDataService;
        private readonly IScreen mHostScreen;
        private readonly ILaunchService mLaunchService;
        private readonly Node<LaunchGroup, long> mNode;
        private bool mIsExpanded;
        private bool mIsSelected;

        public LaunchGroupTreeNode(Node<LaunchGroup, long> node, IDataService dataService, ILaunchService launchService, IScreen hostScreen)
        {
            mNode = node;
            mDataService = dataService;
            mLaunchService = launchService;
            mHostScreen = hostScreen;
            Model = node.Item;

            node.Children.Connect()
                .ObserveOnDispatcher()
                .Transform(x => new LaunchGroupTreeNode(x, dataService, launchService, hostScreen))
                .Sort(SortExpressionComparer<LaunchGroupTreeNode>.Ascending(x => x.Model.Name))
                .Bind(out mChildGroups).DisposeMany()
                .Subscribe().TrackWith(Disposables);

            dataService
                .LaunchItems.Connect()
                .ObserveOnDispatcher()
                .Filter(item => item.ParentGroupId == Model.Id)
                .Sort(SortExpressionComparer<LaunchItem>.Ascending(x => x.Name))
                .Bind(out mChildItems)
                .Subscribe().TrackWith(Disposables);

            Environment = new EnvironmentEditViewModel(
                dataService.GetEnvironmentVariablesFor(node.Item).Connect(),
                envVar => dataService.AddEnvironmentVariable(node.Item, envVar),
                envVar => dataService.DeleteEnvironmentVariable(node.Item, envVar));

            AddGroupCommand = ReactiveCommand.Create();
            AddGroupCommand.Subscribe(_AddGroup).TrackWith(Disposables);

            AddLauncherCommand = ReactiveCommand.Create();
            AddLauncherCommand.Subscribe(_AddLauncher).TrackWith(Disposables);

            DeleteCommand = ReactiveCommand.Create();
            DeleteCommand.Subscribe(_Delete).TrackWith(Disposables);

            DeleteLauncherCommand = ReactiveCommandEx.Create<LaunchItem>();
            DeleteLauncherCommand.Subscribe(_DeleteLauncher).TrackWith(Disposables);

            EditLauncherCommand = ReactiveCommandEx.Create<LaunchItem>();
            EditLauncherCommand.Subscribe(_EditLauncher).TrackWith(Disposables);

            LaunchCommand = ReactiveCommandEx.Create<LaunchItem>();
            LaunchCommand.Subscribe(_Launch).TrackWith(Disposables);

            IsSelected = true;
            IsExpanded = true;
        }

        public ReactiveCommand<object> AddGroupCommand { get; }

        public ReactiveCommand<object> AddLauncherCommand { get; }

        public ReadOnlyObservableCollection<LaunchGroupTreeNode> ChildGroups => mChildGroups;

        public ReadOnlyObservableCollection<LaunchItem> ChildItems => mChildItems;

        public ReactiveCommand<object> DeleteCommand { get; }

        public ReactiveCommand<LaunchItem> DeleteLauncherCommand { get; }

        public ReactiveCommand<LaunchItem> EditLauncherCommand { get; }

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

        public ReactiveCommand<LaunchItem> LaunchCommand { get; }

        public LaunchGroup Model { get; }

        private void _AddGroup(object _)
        {
            mDataService.AddLaunchGroupAsync(new LaunchGroup
            {
                Name = "New group",
                ParentGroupId = Model.Id
            });
        }

        private async void _AddLauncher(object _)
        {
            var item = new LaunchItem
            {
                Name = "New launcher",
                ParentGroupId = Model.Id
            };

            await mDataService.AddLaunchItemAsync(item);
            EditLauncherCommand.Execute(item);
        }

        private void _Delete(object _)
        {
            mDataService.RemoveLaunchGroupAsync(Model);
        }

        private void _DeleteLauncher(LaunchItem item)
        {
            mDataService.RemoveLaunchItemAsync(item);
        }

        private void _EditLauncher(LaunchItem item)
        {
            mHostScreen.Router.Navigate.Execute(new LaunchItemEditViewModel(item, mDataService, mLaunchService, mHostScreen));
        }

        private void _Launch(LaunchItem item)
        {
            mLaunchService.Launch(item);
        }
    }
}