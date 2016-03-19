using Autofac;

using Squirrel;

namespace Anvil.Services
{
    public sealed class ServiceModule : Module
    {
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
                    return UpdateManager.GitHubUpdateManager(
                        config.UpdateUrl,
                        prerelease: config.IncludePreReleaseVersions).Result;
                })
                .As<IUpdateManager>()
                .SingleInstance();
        }
    }
}
