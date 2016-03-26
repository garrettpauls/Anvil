using System;
using System.Windows;

using Anvil.Services;

using Autofac;

using Hardcodet.Wpf.TaskbarNotification;

namespace Anvil.Views.Notification
{
    public partial class TrayIcon : TaskbarIcon, IStartable, IDisposable, INotificationService
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

        public void ShowError(string message, Exception exception)
        {
            ShowBalloonTip(message, exception.ToString(), BalloonIcon.Error);
        }

        public void Start()
        {
            DataContext = mViewModelFactory();
            Visibility = Visibility.Visible;
        }
    }
}
