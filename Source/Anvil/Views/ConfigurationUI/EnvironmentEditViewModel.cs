using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Anvil.Framework.ComponentModel;
using Anvil.Framework.MVVM;
using Anvil.Framework.Reactive;
using Anvil.Models;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

namespace Anvil.Views.ConfigurationUI
{
    public sealed class EnvironmentEditViewModel : DisposableViewModel
    {
        private readonly Func<EnvironmentVariable, Task> mAddEnvVarAsync;
        private readonly Func<EnvironmentVariable, Task> mDeleteEnvVarAsync;
        private readonly ReadOnlyObservableCollection<EnvironmentVariable> mEnvironmentVariables;
        private EnvironmentVariable mAddNewVariable = new EnvironmentVariable();

        public EnvironmentEditViewModel(
            IObservable<IChangeSet<EnvironmentVariable>> environmentVariables,
            Func<EnvironmentVariable, Task> addEnvVarAsync,
            Func<EnvironmentVariable, Task> deleteEnvVarAsync)
        {
            mAddEnvVarAsync = addEnvVarAsync;
            mDeleteEnvVarAsync = deleteEnvVarAsync;

            environmentVariables
                .ObserveOnDispatcher()
                .Sort(SortExpressionComparer<EnvironmentVariable>.Ascending(x => x.Key).ThenByAscending(x => x.Value))
                .Bind(out mEnvironmentVariables)
                .Subscribe().TrackWith(Disposables);

            AddCommand = ReactiveCommand.Create();
            AddCommand.Subscribe(_AddNew).TrackWith(Disposables);

            DeleteCommand = ReactiveCommandEx.Create<EnvironmentVariable>();
            DeleteCommand.Subscribe(_Delete).TrackWith(Disposables);
        }

        public ReactiveCommand<object> AddCommand { get; }

        public EnvironmentVariable AddNewVariable
        {
            get { return mAddNewVariable; }
            private set { this.RaiseAndSetIfChanged(ref mAddNewVariable, value); }
        }

        public ReactiveCommand<EnvironmentVariable> DeleteCommand { get; }

        public ReadOnlyObservableCollection<EnvironmentVariable> EnvironmentVariables => mEnvironmentVariables;

        private async void _AddNew(object _)
        {
            var envVar = AddNewVariable;
            AddNewVariable = new EnvironmentVariable();

            await mAddEnvVarAsync(envVar);
        }

        private async void _Delete(EnvironmentVariable envVar)
        {
            await mDeleteEnvVarAsync(envVar);
        }
    }
}