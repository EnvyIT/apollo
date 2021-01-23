using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Util.Test
{
    public class TypeExtensionTest
    {
        [Test]
        public void Test_ValidTypes_Should_BeTrue()
        {
            var source = "value";
            var result = source.GetType().IsAnyType(typeof(double), typeof(int), typeof(string));
            result.Should().BeTrue();
        }

        [Test]
        public void Test_InvalidTypes_Should_BeFalse()
        {
            var source = "value";
            var result = source.GetType().IsAnyType(typeof(double), typeof(int));
            result.Should().BeFalse();
        }

        [Test]
        public void Test_TypeObject_Should_BeFalse()
        {
            var source = "value";
            var result = source.GetType().IsAnyType(typeof(double), typeof(int), typeof(object));
            result.Should().BeFalse();
        }
    }
}
