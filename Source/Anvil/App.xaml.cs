﻿using System;
using System.Windows;

using Anvil.Views;

using Autofac.Extras.NLog;

namespace Anvil
{
    public partial class App : Application
    {
        private readonly ILogger mLog;
        private readonly Func<MainWindow> mMainWindowFactory;

        public App(Func<MainWindow> mainWindowFactory, ILogger log)
        {
            mMainWindowFactory = mainWindowFactory;
            mLog = log;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            mLog.Info("Starting application...");

            MainWindow = mMainWindowFactory();
            MainWindow.Show();
        }
    }
}