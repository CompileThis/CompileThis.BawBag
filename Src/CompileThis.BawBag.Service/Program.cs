namespace CompileThis.BawBag.Service
{
    using CompileThis.BawBag.Service.Runtime;

    using Topshelf;

    internal class Program
    {
        public static void Main()
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
        }
    }
}
