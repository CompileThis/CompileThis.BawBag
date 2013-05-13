namespace CompileThis.BawBag.Service
{
    using CompileThis.BawBag.Service.Runtime;
using System;

    using NLog;

    using Topshelf;

    internal class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionProcessor;
            HostFactory.Run(hostConfig =>
                {
                    hostConfig.SetServiceName("BawBag");
                    hostConfig.SetDisplayName("CompileThis BawBag");

                    hostConfig.RunAs("default", "default");

                    hostConfig.Service<BawBagService>(serviceConfig =>
                    {
                        serviceConfig.WhenStarted(service => service.Start());
                        serviceConfig.WhenStopped(service => service.Stop());

                        serviceConfig.ConstructUsing(hs => new BawBagService());
                    });
                });
        }

        private static void UnhandledExceptionProcessor(object sender, UnhandledExceptionEventArgs e)
        {
            Log.FatalException("Unhandled exception", e.ExceptionObject as Exception);
            Environment.Exit(256);
        }
    }
}
