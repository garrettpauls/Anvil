using Anvil.Framework.MVVM;

using ReactiveUI;

namespace Anvil.Views.ConfigurationUI
{
    public sealed class MainConfigurationViewModel : RoutableViewModel
    {
        public MainConfigurationViewModel(IScreen hostScreen)
            : base("config/main", hostScreen)
        {
        }
    }
}