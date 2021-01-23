using System;

namespace Apollo.Util
{
    public static class RandomGenerator
    {
        private static readonly Random _random;


        static RandomGenerator()
        {
            _random = new Random(new DateTime().Millisecond);
        }

        public static decimal GenerateRandomNumber(decimal min, decimal max)
        {
            return (decimal)_random.NextDouble() * (max - min) + min;
        }

        public static double GenerateRandomNumber(double min, double max)
        {
            return min + (_random.NextDouble() * (max - min));
        }

        public static int GenerateRandomNumber(int min, int max)
        {
            return _random.Next(min, max + 1);
        }
    }
}
