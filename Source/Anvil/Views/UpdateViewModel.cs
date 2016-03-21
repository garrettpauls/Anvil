using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Anvil.Framework;
using Anvil.Framework.ComponentModel;
using Anvil.Framework.MVVM;

using Autofac.Extras.NLog;

using ReactiveUI;

using Squirrel;

namespace Anvil.Views
{
    public sealed class UpdateViewModel : RoutableViewModel
    {
        private readonly ILogger mLog;
        private readonly IUpdateManager mUpdateManager;
        private bool mIsUpdateAvailable;

        private bool mIsUpdateCompleted;
        private string mReleaseNotes;
        private UpdateInfo mUpdateInfo;

        public UpdateViewModel(IUpdateManager updateManager, IScreen hostScreen, ILogger log) : base("update", hostScreen)
        {
            Guard.AgainstNullArgument(nameof(updateManager), updateManager);
            Guard.AgainstNullArgument(nameof(hostScreen), hostScreen);
            Guard.AgainstNullArgument(nameof(log), log);

            mLog = log;

            mUpdateManager = updateManager;

            ApplyUpdateCommand = ReactiveCommand.Create(this.WhenAnyValue(x => x.IsUpdateAvailable));
            ApplyUpdateCommand
                .Subscribe(_ApplyUpdate).TrackWith(Disposables);

            CheckForUpdatesCommand = ReactiveCommand.Create();
            CheckForUpdatesCommand
                .Subscribe(_CheckForUpdates).TrackWith(Disposables);
        }

        public ReactiveCommand<object> ApplyUpdateCommand { get; }

        public ReactiveCommand<object> CheckForUpdatesCommand { get; }

        public bool IsUpdateAvailable
        {
            get { return mIsUpdateAvailable; }
            private set { this.RaiseAndSetIfChanged(ref mIsUpdateAvailable, value); }
        }

        public bool IsUpdateCompleted
        {
            get { return mIsUpdateCompleted; }
            private set { this.RaiseAndSetIfChanged(ref mIsUpdateCompleted, value); }
        }

        public string ReleaseNotes
        {
            get { return mReleaseNotes; }
            private set { this.RaiseAndSetIfChanged(ref mReleaseNotes, value); }
        }

        private void _ApplyUpdate(object _)
        {
            IsUpdateAvailable = false;

            var update = mUpdateInfo;
            if(update == null)
            {
                return;
            }

            mUpdateManager
                .ApplyReleases(update)
                .ContinueWith(task =>
                {
                    if(task.Exception != null)
                    {
                        mLog.Error("Failed to apply updates", task.Exception);
                        return;
                    }

                    IsUpdateCompleted = true;
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void _CheckForUpdates(object _)
        {
            mLog.Info("Checking for updates...");

            mUpdateManager
                .CheckForUpdate()
                .ContinueWith(task =>
                {
                    if(task.Exception != null)
                    {
                        mLog.Error("Failed to check for updates", task.Exception);
                        return;
                    }

                    mUpdateInfo = task.Result;
                    IsUpdateAvailable =
                        mUpdateInfo != null &&
                        mUpdateInfo.FutureReleaseEntry != null && mUpdateInfo.CurrentlyInstalledVersion != null &&
                        mUpdateInfo.FutureReleaseEntry.Version != mUpdateInfo.CurrentlyInstalledVersion.Version;

                    if(IsUpdateAvailable)
                    {
                        ReleaseNotes = _CreateReleaseNotes(mUpdateInfo);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private static string _CreateReleaseNotes(UpdateInfo update)
        {
            var notes = update.FetchReleaseNotes();
            var orderedNotes = notes.OrderByDescending(note => note.Key.Version);

            var builder = new StringBuilder();

            foreach(var note in orderedNotes)
            {
                builder.AppendLine(note.Key.Version.ToString());
                builder.AppendLine();

                builder.AppendLine(note.Value);
                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}
