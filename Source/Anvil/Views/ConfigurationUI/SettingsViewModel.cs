using Anvil.Framework.MVVM;
using Anvil.Services;

using ReactiveUI;

namespace Anvil.Views.ConfigurationUI
{
    public sealed class SettingsViewModel : RoutableViewModel
    {
        public SettingsViewModel(IConfiguration configuration, IScreen hostScreen)
            : base("settings", hostScreen)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
    }
}
