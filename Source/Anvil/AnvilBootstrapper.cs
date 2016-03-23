using System;
using System.Windows;
using System.Windows.Threading;

using Anvil.Framework;

using Autofac;
using Autofac.Extras.NLog;

using Splat;

using ILogger = Autofac.Extras.NLog.ILogger;

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

                _ConfigureLogging(lifetime);

                var app = lifetime.Resolve<App>();
                app.InitializeComponent();
                app.Run();
            }
        }

        private static void _Configure(ContainerBuilder builder)
        {
            builder.RegisterAssemblyModules(typeof(AnvilBootstrapper).Assembly);
            builder.RegisterModule<NLogModule>();
            builder.RegisterModule<SimpleNLogModule>();

            builder.RegisterType<App>().AsSelf().As<Application>().SingleInstance();
            builder.Register(context => AppDomain.CurrentDomain).As<AppDomain>().SingleInstance();
            builder.Register(context => Dispatcher.CurrentDispatcher).As<Dispatcher>().SingleInstance();
        }

        private static void _ConfigureLogging(IContainer lifetime)
        {
            var app = lifetime.Resolve<App>();
            var domain = lifetime.Resolve<AppDomain>();

            var log = lifetime.Resolve<ILogger>();

            app.DispatcherUnhandledException += (sender, args) => log.Error(args.Exception);
            domain.UnhandledException += (sender, args) => log.Error((Exception) args.ExceptionObject);
            domain.FirstChanceException += (sender, args) => log.Error(args.Exception);
        }

        [STAThread]
        public static void Main()
        {
            new AnvilBootstrapper().Run();
        }
    }
}
