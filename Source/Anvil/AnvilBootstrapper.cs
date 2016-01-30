using System;
using System.Windows;

using Anvil.Framework;

using Autofac;
using Autofac.Extras.NLog;

using Splat;

namespace Anvil
{
    public sealed class AnvilBootstrapper
    {
        public void Run()
        {
            var builder = new ContainerBuilder();
            _Configure(builder);

            using(var lifetime = builder.Build())
            {
                Locator.Current = Locator.CurrentMutable = new AutofacDependencyResolver(lifetime);

                var app = lifetime.Resolve<App>();
                app.InitializeComponent();
                app.Run();
            }
        }

        private static void _Configure(ContainerBuilder builder)
        {
            builder.RegisterAssemblyModules(typeof(AnvilBootstrapper).Assembly);
            builder.RegisterModule<NLogModule>();
            builder.RegisterType<App>().AsSelf().As<Application>();
        }

        [STAThread]
        public static void Main()
        {
            new AnvilBootstrapper().Run();
        }
    }
}