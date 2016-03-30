using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Anvil.Framework.ComponentModel;
using Anvil.Framework.MVVM;
using Anvil.Framework.Reactive;
using Anvil.Models;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

namespace Anvil.Views.ConfigurationUI
{
    public sealed class EnvironmentVariableViewModel : ReactiveObject
    {
        private bool mIsSelected;

        public EnvironmentVariableViewModel(EnvironmentVariable model)
        {
            Model = model;
        }

        public bool IsSelected
        {
            get { return mIsSelected; }
            set { this.RaiseAndSetIfChanged(ref mIsSelected, value); }
        }

        public EnvironmentVariable Model { get; }
    }

    public sealed class EnvironmentEditViewModel : DisposableViewModel
    {
        private readonly Func<EnvironmentVariable, Task> mAddEnvVarAsync;
        private readonly Func<EnvironmentVariable, Task> mDeleteEnvVarAsync;
        private readonly ObservableCollectionExtended<EnvironmentVariableViewModel> mEnvironmentVariables;
        private EnvironmentVariable mAddNewVariable = new EnvironmentVariable();

        public EnvironmentEditViewModel(
            IObservable<IChangeSet<EnvironmentVariable>> environmentVariables,
            Func<EnvironmentVariable, Task> addEnvVarAsync,
            Func<EnvironmentVariable, Task> deleteEnvVarAsync)
        {
            mAddEnvVarAsync = addEnvVarAsync;
            mDeleteEnvVarAsync = deleteEnvVarAsync;
            mEnvironmentVariables = new ObservableCollectionExtended<EnvironmentVariableViewModel>();

            environmentVariables
                .ObserveOnDispatcher()
                .Transform(x => new EnvironmentVariableViewModel(x))
                .DisposeMany()
                .Sort(SortExpressionComparer<EnvironmentVariableViewModel>.Ascending(x => x.Model.Key).ThenByAscending(x => x.Model.Value))
                .Bind(mEnvironmentVariables)
                .Subscribe().TrackWith(Disposables);

            AddCommand = ReactiveCommand.Create();
            AddCommand.Subscribe(_AddNew).TrackWith(Disposables);

            DeleteCommand = ReactiveCommandEx.Create<EnvironmentVariableViewModel>();
            DeleteCommand.Subscribe(_Delete).TrackWith(Disposables);

            var hasSelectedVariables = mEnvironmentVariables
                .ToObservableChangeSet()
                .AddKey(x => x.Model.Key)
                .TrueForAny(x => x.WhenAnyValue(y => y.IsSelected), x => x);

            CopySelectedCommand = ReactiveCommand.Create(hasSelectedVariables);
            CopySelectedCommand.Subscribe(_CopySelectedToClipboard).TrackWith(Disposables);

            PasteCommand = ReactiveCommand.Create();
            PasteCommand.Subscribe(_Paste).TrackWith(Disposables);
        }

        public ReactiveCommand<object> AddCommand { get; }

        public EnvironmentVariable AddNewVariable
        {
            get { return mAddNewVariable; }
            private set { this.RaiseAndSetIfChanged(ref mAddNewVariable, value); }
        }

        public ReactiveCommand<object> CopySelectedCommand { get; }

        public ReactiveCommand<EnvironmentVariableViewModel> DeleteCommand { get; }

        public IEnumerable<EnvironmentVariableViewModel> EnvironmentVariables => mEnvironmentVariables;

        public ReactiveCommand<object> PasteCommand { get; }

        private async void _AddNew(object _)
        {
            var envVar = AddNewVariable;
            AddNewVariable = new EnvironmentVariable();

            await mAddEnvVarAsync(envVar);
        }

        private void _CopySelectedToClipboard(object _)
        {
            var selectedItems = EnvironmentVariables.Where(x => x.IsSelected).ToArray();

            var text = new StringBuilder();
            foreach(var item in selectedItems)
            {
                // being naive and assuming Key won't include tabs or newlines, they shouldn't be addable via the UI
                text.AppendLine($"{item.Model.Key}\t{item.Model.Value}");
            }

            Clipboard.SetText(text.ToString());
        }

        private async void _Delete(EnvironmentVariableViewModel envVar)
        {
            await mDeleteEnvVarAsync(envVar.Model);
        }

        private async void _Paste(object _)
        {
            var text = Clipboard.GetText();
            var lines = text.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

            foreach(var line in lines)
            {
                var tabIndex = line.IndexOf('\t');
                string key, value;
                if(tabIndex < 0)
                {
                    key = line;
                    value = "";
                }
                else
                {
                    key = line.Substring(0, tabIndex);
                    value = line.Substring(tabIndex + 1);
                }

                var existingVar = mEnvironmentVariables.FirstOrDefault(x => key.Equals(x.Model.Key, StringComparison.OrdinalIgnoreCase));
                if(existingVar != null)
                {
                    existingVar.Model.Value = value;
                }
                else
                {
                    await mAddEnvVarAsync(new EnvironmentVariable
                    {
                        Key = key,
                        Value = value
                    });
                }
            }
        }
    }
}
