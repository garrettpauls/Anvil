using Anvil.Framework.MVVM;

namespace Anvil.Views.ConfigurationUI
{
    public partial class MainConfigurationView : MainConfigurationViewImpl
    {
        public MainConfigurationView()
        {
            InitializeComponent();
        }
    }

    public abstract class MainConfigurationViewImpl : View<MainConfigurationViewModel>
    {
        internal MainConfigurationViewImpl()
        {
        }
    }
}