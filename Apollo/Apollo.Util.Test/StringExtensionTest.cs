using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Util.Test
{
    public class StringExtensionTest
    {
        private const string HelloWorldPascalCase = "HelloWorld";
        private const string HelloWorldCamelCase = "helloWorld";
        private const string HelloWorldUpperCase = "HELLOWORLD";
        private const string HelloWorldLowerCase = "helloworld";
        private const string HelloWorldRandomUpperCaseLetters = "hEllOwORlD";
        private const string HelloEarth = "Hello Earth!";


        [Test]
        public void TwoStringSameCase_ShouldReturnEqual()
        {
            HelloWorldCamelCase
                .EqualsIgnoreCase(HelloWorldCamelCase)
                .Should()
                .BeTrue();
        }

        [Test]
        public void UpperCaseStringAndLowerCaseString_ShouldReturnEqual()
        {
            HelloWorldUpperCase
                .EqualsIgnoreCase(HelloWorldLowerCase)
                .Should()
                .BeTrue();
        }

        [Test]
        public void CamelCaseStringAndPascalCaseString_ShouldReturnEqual()
        {
            HelloWorldCamelCase
                .EqualsIgnoreCase(HelloWorldPascalCase)
                .Should()
                .BeTrue();
        }

        [Test]
        public void UpperCaseStringAndRandomUpperCaseLetterString_ShouldReturnEqual()
        {
            HelloWorldUpperCase
                .EqualsIgnoreCase(HelloWorldRandomUpperCaseLetters)
                .Should()
                .BeTrue();
        }

        [Test]
        public void TwoDifferentStringsInLowerCaseShouldReturnNotEqual()
        {
            HelloWorldPascalCase
                .EqualsIgnoreCase(HelloEarth)
                .Should()
                .BeFalse();
        }
    }
}
