using Anvil.Framework.MVVM;

using Autofac;

using ReactiveUI;

namespace Anvil.Views
{
    public sealed class ViewModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = new[] {typeof(ViewModule).Assembly};

            builder.RegisterAssemblyTypes(assemblies)
                   .InNamespaceOf<ViewModule>()
                   .AssignableTo<IView>()
                   .AsClosedTypesOf(typeof(View<>))
                   .AsClosedTypesOf(typeof(IViewFor<>))
                   .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(assemblies)
                   .InNamespaceOf<ViewModule>()
                   .AssignableTo<IRoutableViewModel>()
                   .AsSelf()
                   .InstancePerLifetimeScope();

            builder.RegisterType<Shell>()
                   .AsSelf()
                   .InstancePerLifetimeScope();

            builder.RegisterType<ShellViewModel>()
                   .AsSelf()
                   .As<IScreen>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<RoutingState>()
                   .AsSelf()
                   .InstancePerLifetimeScope();
        }
    }
}