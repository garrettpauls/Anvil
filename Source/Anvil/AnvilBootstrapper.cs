using System;
using System.Diagnostics;
using System.IO;
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
            _CreateDefaultLogDirectory();

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

        private static void _CreateDefaultLogDirectory()
        {
            // NLog doesn't reliably create the logs directory when it's missing
            // so we force it to exist now.
            var defaultLogDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

            try
            {
                Directory.CreateDirectory(defaultLogDir);
            }
            catch(Exception ex)
            {
                var message = $"Failed to create default log directory {defaultLogDir}: {ex.Message}\r\n\r\n{ex}";

                Debug.WriteLine(message);
                Console.Error.WriteLine(message);
            }
        }

        [STAThread]
        public static void Main()
        {
            new AnvilBootstrapper().Run();
        }
    }
}
