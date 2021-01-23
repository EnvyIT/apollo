using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Apollo.Util.Test
{
    public class ConfigurationHelperTest
    {

        [OneTimeSetUp]
        public void SetupConfiguration()
        {
            ConfigurationHelper.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("config.test.json")
                .Build();
        }


        [Test]
        public void ConfigurationHelper_ShouldReturnValueIfKeyExists()
        {
            const string expectedAPIKey = "AMG123-ON8bR-AGSF#16-12345";
            var actualResult = ConfigurationHelper.GetValues("TEST_KEY");
            actualResult.Should().NotBeEmpty();
            actualResult.Length.Should().Be(1);
            actualResult[0].Should().Be(expectedAPIKey);
        }

        [Test]
        public void ConfigurationHelper_ShouldThrowFileNotFoundExceptionIfFileDoesNotExist()
        {
            Action actor = () => ConfigurationHelper.GetValues("API_KEY");
            actor.Should().Throw<KeyNotFoundException>();
        }
    }
}