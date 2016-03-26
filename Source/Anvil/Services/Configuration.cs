using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Anvil.Framework.ComponentModel;
using Anvil.Framework.Reactive;
using Anvil.Properties;

using ReactiveUI;

namespace Anvil.Services
{
    public interface IConfiguration : IService
    {
        bool CloseToSystemTray { get; set; }

        bool IncludePreReleaseVersions { get; set; }

        string UpdateUrl { get; }
    }

    public sealed class Configuration : DisposableReactiveObject, IConfiguration, IInitializableService
    {
        private const string DEFAULT_UPDATE_URL = "https://github.com/garrettpauls/Anvil";
        private readonly Settings mSettings;

        public Configuration()
        {
            mSettings = Settings.Default;
            Observable
                .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    handler => mSettings.PropertyChanged += handler,
                    handler => mSettings.PropertyChanged -= handler)
                .Subscribe(x => mSettings.Save())
                .TrackWith(Disposables);
        }

        public bool CloseToSystemTray
        {
            get { return mSettings.CloseToSystemTray; }
            set
            {
                if(CloseToSystemTray != value)
                {
                    mSettings.CloseToSystemTray = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IncludePreReleaseVersions
        {
            get { return mSettings.IncludePreRelease; }
            set
            {
                if(IncludePreReleaseVersions != value)
                {
                    mSettings.IncludePreRelease = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string UpdateUrl => (mSettings.UpdateUrl == "" ? null : mSettings.UpdateUrl) ?? DEFAULT_UPDATE_URL;

        public Task InitializeAsync()
        {
            return Task.Factory.StartNew(mSettings.Upgrade);
        }
    }
}
