using System;
using System.Windows;

using Autofac;

using Hardcodet.Wpf.TaskbarNotification;

namespace Anvil.Views.Notification
{
    public partial class TrayIcon : TaskbarIcon, IStartable, IDisposable
    {
        private readonly Func<TrayIconViewModel> mViewModelFactory;

        public TrayIcon(Func<TrayIconViewModel> viewModelFactory)
        {
            InitializeComponent();

            mViewModelFactory = viewModelFactory;
            Icon = Properties.Resources.AnvilIcon;
        }

        public new void Dispose()
        {
            Visibility = Visibility.Collapsed;
            (DataContext as IDisposable)?.Dispose();

            base.Dispose();
        }

        public void Start()
        {
            DataContext = mViewModelFactory();
            Visibility = Visibility.Visible;
        }
    }
}
