namespace CompileThis.BawBag.Service
{
    using System;

    using Topshelf;

    using CompileThis.BawBag.Service.Runtime;

    class Program
    {
        static void Main(string[] args)
        {
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

            Console.ReadLine();
        }
    }
}
