using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

using Anvil.Framework.ComponentModel;
using Anvil.Framework.MVVM;
using Anvil.Models;
using Anvil.Services;
using Anvil.Services.Data;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

namespace Anvil.Views.ConfigurationUI
{
    public sealed class MainConfigurationViewModel : RoutableViewModel
    {
        private readonly IDataService mDataService;
        private readonly ReadOnlyObservableCollection<EditableLaunchGroupTreeNode> mLaunchGroups;

        private EditableLaunchGroupTreeNode mSelectedGroup;

        public MainConfigurationViewModel(IDataService dataService, ILaunchService launchService, IScreen hostScreen)
            : base("config/main", hostScreen)
        {
            mDataService = dataService;

            AddGroupCommand = ReactiveCommand.Create();
            AddGroupCommand.Subscribe(_AddRootGroup).TrackWith(Disposables);

            dataService
                .LaunchGroups.Connect()
                .ObserveOnDispatcher()
                .TransformToTree(grp => grp.ParentGroupId ?? -1)
                .Transform(x => new EditableLaunchGroupTreeNode(x, dataService, launchService, hostScreen))
                .Sort(SortExpressionComparer<EditableLaunchGroupTreeNode>.Ascending(x => x.Model.Name))
                .Bind(out mLaunchGroups).DisposeMany()
                .Subscribe().TrackWith(Disposables);

            var firstGroup = mLaunchGroups.FirstOrDefault();
            if(firstGroup != null)
            {
                firstGroup.IsSelected = true;
            }
        }

        public ReactiveCommand<object> AddGroupCommand { get; }

        public ReadOnlyObservableCollection<EditableLaunchGroupTreeNode> LaunchGroups => mLaunchGroups;

        public EditableLaunchGroupTreeNode SelectedGroup
        {
            get { return mSelectedGroup; }
            set { this.RaiseAndSetIfChanged(ref mSelectedGroup, value); }
        }

        private async void _AddRootGroup(object _)
        {
            var grp = new LaunchGroup
            {
                Name = "New group",
                ParentGroupId = null
            };

            await mDataService.AddLaunchGroupAsync(grp);
        }
    }
}
