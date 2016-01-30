using Autofac;

namespace Anvil.Services
{
    public sealed class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = new[] {typeof(ServiceModule).Assembly};
            builder.RegisterAssemblyTypes(assemblies)
                   .InNamespaceOf<ServiceModule>()
                   .AssignableTo<IService>()
                   .AsImplementedInterfaces()
                   .OnActivating(async args =>
                   {
                       var initializable = args.Instance as IInitializableService;
                       if(initializable != null)
                       {
                           await initializable.InitializeAsync();
                       }
                   });
        }
    }
}