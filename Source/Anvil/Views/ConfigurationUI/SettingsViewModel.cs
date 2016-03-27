using Anvil.Framework.MVVM;
using Anvil.Models;
using Anvil.Services;

using ReactiveUI;

namespace Anvil.Views.ConfigurationUI
{
    public sealed class SettingsViewModel : RoutableViewModel
    {
        private readonly IConfigurationService mConfiguration;

        public SettingsViewModel(IConfigurationService configuration, IScreen hostScreen)
            : base("settings", hostScreen)
        {
            mConfiguration = configuration;
        }

        public bool CloseToSystemTray
        {
            get { return mConfiguration.GetValue(CommonConfigKeys.CloseToSystemTray, () => false); }
            set
            {
                mConfiguration.SetValue(CommonConfigKeys.CloseToSystemTray, value);
                this.RaisePropertyChanged();
            }
        }

        public bool IncludePreReleaseVersions
        {
            get { return mConfiguration.GetValue(CommonConfigKeys.IncludePreRelease, () => false); }
            set
            {
                mConfiguration.SetValue(CommonConfigKeys.IncludePreRelease, value);
                this.RaisePropertyChanged();
            }
        }
    }
}
