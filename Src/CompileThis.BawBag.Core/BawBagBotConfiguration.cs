namespace CompileThis.BawBag
{
    using System;
    using System.Configuration;

    public class BawBagBotConfiguration
    {
        public string JabbrUrl { get; set; }
        public string JabbrNick { get; set; }
        public string JabbrPassword { get; set; }

        public string[] Rooms { get; set; }
        public string[] Admins { get; set; }

        public string RavenUrl { get; set; }
        public string RavenDatabase { get; set; }

        public string PluginsDirectory { get; set; }

        public static BawBagBotConfiguration FromConfigFile()
        {
            return new BawBagBotConfiguration
                {
                    JabbrUrl = ConfigurationManager.AppSettings["BawBag/JabbrUrl"],
                    JabbrNick = ConfigurationManager.AppSettings["BawBag/JabbrNick"],
                    JabbrPassword = ConfigurationManager.AppSettings["BawBag/JabbrPassword"],
                    Rooms = ConfigurationManager.AppSettings["BawBag/Rooms"].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries),
                    Admins = ConfigurationManager.AppSettings["BawBag/Admins"].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries),
                    RavenUrl = ConfigurationManager.AppSettings["BawBag/RavenUrl"],
                    RavenDatabase = ConfigurationManager.AppSettings["BawBag/RavenDatabase"],
                    PluginsDirectory = ConfigurationManager.AppSettings["BawBag/PluginsDirectory"]
                };
        }
    }
}
