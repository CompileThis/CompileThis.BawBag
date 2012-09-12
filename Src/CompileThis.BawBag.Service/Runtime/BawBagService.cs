namespace CompileThis.BawBag.Service.Runtime
{
    public class BawBagService
    {
        private BawBagBot _bot;

        public async void Start()
        {
            var config = new BawBagBotConfiguration
                {
                    Name = "BawBag",
                    Password = "B4wB4g",
                    Url = "http://debauchery.apphb.com",
                    Rooms = new[] { "Debauchery", "BawBag" }
                };

            _bot = new BawBagBot(config);

            await _bot.Start();
        }

        public async void Stop()
        {
            await _bot.Stop();
        }
    }
}
