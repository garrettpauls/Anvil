using System;

using Anvil.Framework.ComponentModel;
using Anvil.Framework.Reactive;
using Anvil.Models;
using Anvil.Services;
using Anvil.Services.Data;

using DynamicData;

using ReactiveUI;

namespace Anvil.Views.ConfigurationUI
{
    public sealed class EditableLaunchGroupTreeNode : LaunchGroupTreeNode<EditableLaunchGroupTreeNode>
    {
        private readonly IDataService mDataService;
        private readonly IScreen mHostScreen;
        private bool mIsExpanded;
        private bool mIsSelected;

        public EditableLaunchGroupTreeNode(Node<LaunchGroup, long> node, IDataService dataService, ILaunchService launchService, IScreen hostScreen)
            : base(node, dataService, launchService, x => new EditableLaunchGroupTreeNode(x, dataService, launchService, hostScreen))
        {
            mDataService = dataService;
            mHostScreen = hostScreen;

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

            IsSelected = true;
            IsExpanded = true;
        }

        public ReactiveCommand<object> AddGroupCommand { get; }

        public ReactiveCommand<object> AddLauncherCommand { get; }

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
            mHostScreen.Router.Navigate.Execute(new LaunchItemEditViewModel(item, mDataService, LaunchService, mHostScreen));
        }
    }
}
