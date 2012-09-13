namespace CompileThis.BawBag.Service.Runtime
{
    public class BawBagService
    {
        private BawBagBot _bot;

        public void Start()
        {
            var config = new BawBagBotConfiguration
                {
                    Name = "BawBag",
                    Password = "B4wB4g",
                    Url = "http://debauchery.apphb.com",
                    Rooms = new[] { "BawBag", "Debauchery" }
                };

            _bot = new BawBagBot(config);

            _bot.Start().Wait();
        }

        public void Stop()
        {
            _bot.Stop().Wait();
        }
    }
}
