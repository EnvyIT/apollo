using System;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Util.Test
{
    public class SingletonHelperTest
    {
        [Test]
        public void Test_CreateInstance_NoCallback_ThrowException()
        {
            object instance = null;
            Action callback = () => SingletonHelper.GetInstance(ref instance, null);
            callback.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Test_CreateInstance_Return_NewInstance()
        {
            const string expected = "Instance";
            string instance = null;

            var result = SingletonHelper.GetInstance(ref instance, () => expected);

            result.Should().NotBeNull();
            result.Should().Be(expected);
        }

        [Test]
        public void Test_CreateInstance_ShouldNot_OverrideInstance()
        {
            const string expected = "Instance";
            string instance = null;

            SingletonHelper.GetInstance(ref instance, () => expected);
            var result = SingletonHelper.GetInstance(ref instance, () => "New Instance");

            result.Should().NotBeNull();
            result.Should().Be(expected);
        }
    }
}