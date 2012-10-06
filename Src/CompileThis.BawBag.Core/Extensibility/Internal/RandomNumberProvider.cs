namespace CompileThis.BawBag.Extensibility.Internal
{
    using System;

    internal class RandomNumberProvider : IRandomNumberProvider
    {
        private readonly Random _random;

        public RandomNumberProvider()
        {
            _random = new Random();
        }

        public int Next()
        {
            return _random.Next();
        }

        public int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }
    }
}
