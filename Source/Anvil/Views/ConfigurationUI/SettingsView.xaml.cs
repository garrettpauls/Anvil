using Anvil.Framework.MVVM;

namespace Anvil.Views.ConfigurationUI
{
    public partial class SettingsView : SettingsViewImpl
    {
        public SettingsView()
        {
            InitializeComponent();
        }
    }

    public abstract class SettingsViewImpl : View<SettingsViewModel>
    {
    }
}
