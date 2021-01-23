using System;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Util.Test
{
    public class ReflectionUtilsTest
    {
        [Test]
        public void ReflectionUtils_GetPropertyOfMockClass_ShouldReturnCorrectValueOfProperty()
        {
            const int expectedNumber = 1;
            var mockClass = new MockClass(expectedNumber);

            var number = ReflectionUtils.GetPropertyValue<int>(mockClass, nameof(MockClass.Number));
            number.Should().Be(expectedNumber);
        }


        [Test]
        public void ReflectionUtils_GetPropertyOfMockClass_ShouldThrowNullReferenceException()
        {
            const int expectedNumber = 1;
            var mockClass = new MockClass(expectedNumber); 
            Action getProperty = () =>  ReflectionUtils.GetPropertyValue<int>(mockClass, "AnotherProp");
            getProperty.Should().ThrowExactly<NullReferenceException>();
        }


        private class MockClass
        {
            public int Number { get; set; }

            public MockClass(int number) 
            {
                Number = number;
            }

        }
    }
}
