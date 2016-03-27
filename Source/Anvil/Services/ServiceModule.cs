using Anvil.Models;

using Autofac;

using Squirrel;

namespace Anvil.Services
{
    public sealed class ServiceModule : Module
    {
        private const string DEFAULT_UPDATE_URL = "https://github.com/garrettpauls/Anvil";

        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = new[] {typeof(ServiceModule).Assembly};
            builder
                .RegisterAssemblyTypes(assemblies)
                .InNamespaceOf<ServiceModule>()
                .AssignableTo<IService>()
                .AsImplementedInterfaces()
                .SingleInstance()
                .OnActivating(async args =>
                {
                    var initializable = args.Instance as IInitializableService;
                    if(initializable != null)
                    {
                        await initializable.InitializeAsync();
                    }
                });

            builder
                .Register(context =>
                {
                    var config = context.Resolve<IConfigurationService>();
                    var updateUrl = config.GetValue(CommonConfigKeys.UpdateUrl, () => DEFAULT_UPDATE_URL);
                    var includePreRelease = config.GetValue(CommonConfigKeys.IncludePreRelease, () => false);

                    return UpdateManager.GitHubUpdateManager(
                        updateUrl,
                        prerelease: includePreRelease).Result;
                })
                .As<IUpdateManager>()
                .SingleInstance();
        }
    }
}
