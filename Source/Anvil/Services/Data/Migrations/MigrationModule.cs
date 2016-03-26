using Autofac;

namespace Anvil.Services.Data.Migrations
{
    public sealed class MigrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(MigrationModule).Assembly)
                   .InNamespaceOf<MigrationModule>()
                   .AssignableTo<IMigration>()
                   .As<IMigration>()
                   .InstancePerDependency();
        }
    }
}
