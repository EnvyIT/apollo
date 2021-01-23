using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Util.Test
{
    public class SetExtensionTest
    {
        [Test]
        public void InsertAllInSet_ShouldReturnTrue()
        {
            var numbers =new List<int>{1, 2, 3, 4, 5};
            ISet<int> numberSet = new HashSet<int>(numbers.Count);
            var allNumbersAdded = numberSet.AddAll(numbers);
            allNumbersAdded.Should().BeTrue();
            numberSet.Count.Should().Be(numbers.Count);
            AssertAllIncludedInSet(numberSet, numbers);
        }

      

        [Test]
        public void DuplicateNumber_ShouldReturnFalse()
        {
            var numbers = new List<int> { 1, 1, 2, 3, 5, 8 };
            ISet<int> numberSet = new HashSet<int>(numbers.Count);
            var allNumbersAdded = numberSet.AddAll(numbers);
            allNumbersAdded.Should().BeFalse();
            numberSet.Count.Should().Be(numbers.Count - 1);
        }


        private static void AssertAllIncludedInSet(ISet<int> numberSet, List<int> numbers)
        {
            numbers.ForEach(number =>
            {
                numberSet.Contains(number).Should().BeTrue();
            });
        }
    }
}
