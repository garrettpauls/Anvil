using System;
using System.Threading.Tasks;
using System.Windows.Forms;

using Squirrel;

namespace Anvil.Views.Notification
{
    public sealed class UpdateProcessor
    {
        private readonly IUpdateManager mUpdateManager;
        private bool mIsProcessing;
        private UpdateStatus mUpdateStatus = UpdateStatus.Unchecked;

        public UpdateProcessor(IUpdateManager updateManager)
        {
            mUpdateManager = updateManager;

            MenuItem = new MenuItem("&Check for updates...", _UpdateClicked);
        }

        public UpdateProcessor()
        {
            MenuItem = new MenuItem("Automatic updates not available");
            MenuItem.Enabled = false;
        }

        public MenuItem MenuItem { get; }

        private void _CheckForUpdates()
        {
            mIsProcessing = true;
            mUpdateStatus = UpdateStatus.CheckingForUpdate;

            MenuItem.Text = "Checking for updates...";
            MenuItem.Enabled = false;

            mUpdateManager
                .CheckForUpdate()
                .ContinueWith(task =>
                {
                    var update = task.Result;
                    var isUpdateAvailable = update.FutureReleaseEntry.Version != update.CurrentlyInstalledVersion.Version;

                    if(isUpdateAvailable)
                    {
                        mUpdateStatus = UpdateStatus.UpdateAvailable;
                        MenuItem.Enabled = true;
                        MenuItem.Text = $"Update to {update.FutureReleaseEntry.Version}";
                    }
                    else
                    {
                        mUpdateStatus = UpdateStatus.NoUpdateAvailable;
                        MenuItem.Text = "No updates available";
                    }

                    mIsProcessing = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void _UpdateApplication()
        {
            mIsProcessing = true;
            mUpdateStatus = UpdateStatus.Updating;
            MenuItem.Text = "Applying update...";
            MenuItem.Enabled = false;

            mUpdateManager
                .UpdateApp()
                .ContinueWith(task =>
                {
                    MenuItem.Text = "Restart Anvil to complete update";
                    MenuItem.Enabled = false;

                    mUpdateStatus = UpdateStatus.UpdatePending;
                    mIsProcessing = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void _UpdateClicked(object sender, EventArgs e)
        {
            if(mIsProcessing)
            {
                return;
            }

            switch(mUpdateStatus)
            {
                case UpdateStatus.Unchecked:
                    _CheckForUpdates();
                    break;
                case UpdateStatus.CheckingForUpdate:
                    break;
                case UpdateStatus.NoUpdateAvailable:
                    break;
                case UpdateStatus.UpdateAvailable:
                    _UpdateApplication();
                    break;
                case UpdateStatus.Updating:
                    break;
                case UpdateStatus.UpdatePending:
                    break;
            }
        }

        private enum UpdateStatus
        {
            Unchecked,
            CheckingForUpdate,
            UpdateAvailable,
            Updating,
            NoUpdateAvailable,
            UpdatePending
        }
    }
}
