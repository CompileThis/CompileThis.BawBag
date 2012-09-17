namespace CompileThis.BawBag.Service.Runtime
{
    public class BawBagService
    {
        private readonly BawBagBot _bot;

        public BawBagService()
        {
            _bot = new BawBagBot();
        }

        public void Start()
        {
            _bot.Start().Wait();
        }

        public void Stop()
        {
            _bot.Stop().Wait();
        }
    }
}
