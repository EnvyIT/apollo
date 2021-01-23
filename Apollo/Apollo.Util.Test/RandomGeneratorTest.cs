using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;


namespace Apollo.Util.Test
{
    public class RandomGeneratorTest
    {
        [Test]
        public void TestRandomGenerator_ShouldReturnCorrectDoubleNumber()
        {
            const double min = 0.5;
            const double max = 1.25;
            var randomNumber = RandomGenerator.GenerateRandomNumber(min, max);
            randomNumber.Should()
                .BeGreaterOrEqualTo(min)
                .And
                .BeLessOrEqualTo(max);
        }

        [Test]
        public void TestRandomGenerator_ShouldReturnCorrectDecimalNumber()
        {
            const decimal min = 1.3M;
            const decimal max = 12.5M;
            var randomNumber = RandomGenerator.GenerateRandomNumber(min, max);
            randomNumber.Should()
                .BeGreaterOrEqualTo(min)
                .And
                .BeLessOrEqualTo(max);
        }

        
        [Test]
        public void TestRandomGenerator_MultipleDecimalValues()
        {
            const decimal min = 1.3M;
            const decimal max = 12.5M;
            var numbers = new List<decimal>();
            for (var i = 0; i < 10000; ++i)
            {
                RandomGenerator.GenerateRandomNumber(min, max)
                    .Should()
                    .BeGreaterOrEqualTo(min)
                    .And
                    .BeLessOrEqualTo(max);
            }
        }

        [Test]
        public void TestRandomGenerator_MultipleIntegerValues()
        {
            const int min = 99;
            const int max = 422;
            var numbers = new List<decimal>();
            for (var i = 0; i < 10000; ++i)
            {
                RandomGenerator.GenerateRandomNumber(min, max)
                    .Should()
                    .BeGreaterOrEqualTo(min)
                    .And
                    .BeLessOrEqualTo(max);
            }

        }
    }
}
