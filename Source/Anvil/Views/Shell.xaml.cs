using System;
using System.ComponentModel;
using System.Windows;

using Anvil.Views.ConfigurationUI;

using ReactiveUI;

namespace Anvil.Views
{
    public partial class Shell : Window, IViewFor<ShellViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(ShellViewModel), typeof(Shell), new PropertyMetadata(default(ShellViewModel)));

        public Shell(ShellViewModel viewModel, RoutingState router, Func<MainConfigurationViewModel> initialViewFactory)
        {
            InitializeComponent();

            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
            ViewModel = viewModel;

            router.Navigate.Execute(initialViewFactory());
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ShellViewModel) value; }
        }

        public ShellViewModel ViewModel
        {
            get { return (ShellViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private void _HandleClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
