using Anvil.Framework.MVVM;

namespace Anvil.Views.ConfigurationUI
{
    public partial class LaunchItemEditView : LaunchItemEditViewImpl
    {
        public LaunchItemEditView()
        {
            InitializeComponent();
        }
    }

    public abstract class LaunchItemEditViewImpl : View<LaunchItemEditViewModel>
    {
    }
}
