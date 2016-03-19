using Anvil.Framework.MVVM;
using Anvil.Models;

using ReactiveUI;

namespace Anvil.Views.ConfigurationUI
{
    public sealed class LaunchItemEditViewModel : RoutableViewModel
    {
        public LaunchItemEditViewModel(LaunchItem model, IScreen hostScreen)
            : base($"launchItem/edit/{model.Id}", hostScreen)
        {
            Model = model;
        }

        public LaunchItem Model { get; }
    }
}
