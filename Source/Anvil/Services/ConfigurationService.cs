using Anvil.Properties;

namespace Anvil.Services
{
    public interface IConfigurationService : IService
    {
        bool IncludePreReleaseVersions { get; }

        string UpdateUrl { get; }
    }

    public sealed class ConfigurationService : IConfigurationService
    {
        private const string DEFAULT_UPDATE_URL = "https://github.com/garrettpauls/Anvil/";
        private readonly Settings mSettings;

        public ConfigurationService()
        {
            mSettings = Settings.Default;
        }

        public bool IncludePreReleaseVersions => mSettings.IncludePreRelease;

        public string UpdateUrl => (mSettings.UpdateUrl == "" ? null : mSettings.UpdateUrl) ?? DEFAULT_UPDATE_URL;
    }
}
