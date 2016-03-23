using System;
using System.Windows;

using Anvil.Framework.ComponentModel;
using Anvil.Framework.MVVM;

using ReactiveUI;

namespace Anvil.Views.Notification
{
    public sealed class TrayIconViewModel : DisposableViewModel
    {
        public TrayIconViewModel()
        {
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_Exit).TrackWith(Disposables);

            ShowMainWindowCommand = ReactiveCommand.Create();
            ShowMainWindowCommand.Subscribe(_ShowMainWindow).TrackWith(Disposables);
        }

        public ReactiveCommand<object> ExitCommand { get; }

        public ReactiveCommand<object> ShowMainWindowCommand { get; }

        private void _Exit(object _)
        {
            Application.Current.Shutdown();
        }

        private static void _ShowMainWindow(object _)
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.Activate();
        }
    }
}
