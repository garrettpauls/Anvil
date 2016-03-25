using System;
using System.ComponentModel;
using System.Windows;

using Anvil.Services;
using Anvil.Views.ConfigurationUI;

using MahApps.Metro.Controls;

using ReactiveUI;

namespace Anvil.Views
{
    public partial class Shell : MetroWindow, IViewFor<ShellViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(ShellViewModel), typeof(Shell), new PropertyMetadata(default(ShellViewModel)));

        private readonly IConfiguration mConfiguration;

        public Shell(ShellViewModel viewModel, RoutingState router, Func<MainConfigurationViewModel> initialViewFactory, IConfiguration configuration)
        {
            mConfiguration = configuration;
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
            if(mConfiguration.CloseToSystemTray)
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
}
