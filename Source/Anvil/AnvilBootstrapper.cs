using System;
using System.Windows;

using Autofac;
using Autofac.Extras.NLog;

namespace Anvil
{
    public sealed class AnvilBootstrapper
    {
        private void _Configure(ContainerBuilder builder)
        {
            builder.RegisterAssemblyModules(typeof(AnvilBootstrapper).Assembly);
            builder.RegisterModule<NLogModule>();
            builder.RegisterType<App>().AsSelf().As<Application>();
        }

        public void Run()
        {
            var builder = new ContainerBuilder();
            _Configure(builder);

            using(var lifetime = builder.Build())
            {
                var app = lifetime.Resolve<App>();
                app.InitializeComponent();
                app.Run();
            }
        }

        [STAThread]
        public static void Main()
        {
            new AnvilBootstrapper().Run();
        }
    }
}