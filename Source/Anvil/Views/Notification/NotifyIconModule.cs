﻿using System;
using System.Windows.Forms;

using Anvil.Framework.ComponentModel;
using Anvil.Properties;

using Autofac;
using Autofac.Extras.NLog;

using Squirrel;

using Application = System.Windows.Application;

namespace Anvil.Views.Notification
{
    public sealed class NotifyIconManager : IStartable, IDisposable
    {
        private readonly DisposableTracker mDisposables = new DisposableTracker();
        private readonly NotifyIcon mIcon;
        private readonly ILogger mLog;
        private readonly UpdateProcessor mUpdateProcessor;

        public NotifyIconManager(Func<IUpdateManager> updateManager, ILogger log)
        {
            mLog = log;
            mIcon = new NotifyIcon();

            try
            {
                mUpdateProcessor = new UpdateProcessor(updateManager());
            }
            catch(Exception ex)
            {
                log.Error("Failed to initialize update manager.", ex);
                mUpdateProcessor = new UpdateProcessor();
            }
        }

        public void Dispose()
        {
            mDisposables.Dispose();

            mIcon.Visible = false;
            mIcon.Dispose();
        }

        public void Start()
        {
            mIcon.Icon = Resources.AnvilIcon;
            mIcon.ContextMenu = new ContextMenu(new[]
            {
                new MenuItem("&Open", _OpenMainWindow),
                mUpdateProcessor.MenuItem,
                new MenuItem("-"),
                new MenuItem("E&xit", _Exit)
            });

            mIcon.Visible = true;
        }

        private static void _Exit(object sender, EventArgs args)
        {
            Application.Current.Shutdown();
        }

        private static void _OpenMainWindow(object sender, EventArgs args)
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.Activate();
        }
    }
}