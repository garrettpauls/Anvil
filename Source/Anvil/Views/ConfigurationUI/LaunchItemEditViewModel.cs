using System;

using Anvil.Framework.ComponentModel;
using Anvil.Framework.MVVM;
using Anvil.Models;
using Anvil.Services.Data;

using ReactiveUI;

namespace Anvil.Views.ConfigurationUI
{
    public sealed class LaunchItemEditViewModel : RoutableViewModel
    {
        public LaunchItemEditViewModel(LaunchItem model, IDataService dataService, IScreen hostScreen)
            : base($"launchItem/edit/{model.Id}", hostScreen)
        {
            Model = model;

            LaunchCommand = ReactiveCommand.Create();
            LaunchCommand.Subscribe(_Launch).TrackWith(Disposables);

            Environment = new EnvironmentEditViewModel(
                dataService.GetEnvironmentVariablesFor(Model).Connect(),
                envVar => dataService.AddEnvironmentVariable(Model, envVar),
                envVar => dataService.DeleteEnvironmentVariable(Model, envVar));
        }

        public EnvironmentEditViewModel Environment { get; }

        public ReactiveCommand<object> LaunchCommand { get; }

        public LaunchItem Model { get; }

        private void _Launch(object _)
        {
            throw new NotImplementedException();
        }
    }
}
