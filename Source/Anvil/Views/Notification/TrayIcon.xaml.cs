using System;
using System.Windows;

using Autofac;

using Hardcodet.Wpf.TaskbarNotification;

namespace Anvil.Views.Notification
{
    public partial class TrayIcon : TaskbarIcon, IStartable, IDisposable
    {
        private readonly TrayIconViewModel mViewModel;

        public TrayIcon(TrayIconViewModel viewModel)
        {
            InitializeComponent();

            Icon = Properties.Resources.AnvilIcon;
            DataContext = mViewModel = viewModel;
        }

        public new void Dispose()
        {
            Visibility = Visibility.Collapsed;
            mViewModel.Dispose();

            base.Dispose();
        }

        public void Start()
        {
            Visibility = Visibility.Visible;
        }
    }
}
