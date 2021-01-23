using System;
using System.Collections.Generic;
using System.Linq;
using Apollo.Core.Exception;
using Apollo.Util.Logger;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Util.Test
{
    public class ValidationHelperTest
    {
        private readonly IApolloLogger<ValidationHelperTest> _logger =
            LoggerFactory.CreateLogger<ValidationHelperTest>();


        [Test]
        public void Test_ValidateNull()
        {
            MockUser mockUser = null;
            Action validate = () => ValidationHelper.ValidateNull(_logger.Here(), mockUser);
            validate.Should().ThrowExactly<ArgumentNullException>();

            validate = () => ValidationHelper.ValidateNull( _logger.Here(), "test");
            validate.Should().NotThrow();
        }

        [Test]
        public void Test_ValidateUpdate()
        {
            const int updatedRows = 0;
            Action validate = () =>
                ValidationHelper.ValidateUpdate<ValidationHelperTest, MockUser>(_logger.Here(), updatedRows);
            validate.Should().ThrowExactly<PersistenceException>();

            validate = () => ValidationHelper.ValidateUpdate<ValidationHelperTest, MockUser>(_logger.Here(), 1);
            validate.Should().NotThrow();
        }

        [Test]
        public void Test_ValidateId()
        {
            const int id = 0;
            Action validate = () => ValidationHelper.ValidateId<ValidationHelperTest, MockUser>(_logger.Here(), id);
            validate.Should().ThrowExactly<PersistenceException>();

            validate = () => ValidationHelper.ValidateId<ValidationHelperTest, MockUser>(_logger.Here(), 15L);
            validate.Should().NotThrow();
        }

        [Test]
        public void Test_ValidateLessOrEquals()
        {
            Action validate = () => ValidationHelper.ValidateLessOrEquals(_logger.Here(), 2, 1);
            validate.Should().ThrowExactly<ArgumentOutOfRangeException>();

            validate = () => ValidationHelper.ValidateLessOrEquals(_logger.Here(), 1, 1);
            validate.Should().NotThrow();

            validate = () => ValidationHelper.ValidateLessOrEquals(_logger.Here(), 1, 2);
            validate.Should().NotThrow();
        }

        [Test]
        public void Test_ValidateEnumerableIsNotNullOrEmpty()
        {
            Action validate = () => ValidationHelper.ValidateEnumerableIsNotNullOrEmpty(_logger.Here(), (IEnumerable<string>) null, "property");
            validate.Should().ThrowExactly<ArgumentException>();

            validate = () => ValidationHelper.ValidateEnumerableIsNotNullOrEmpty(_logger.Here(),Enumerable.Empty<string>(), "property");
            validate.Should().ThrowExactly<ArgumentException>();

            validate = () => ValidationHelper.ValidateEnumerableIsNotNullOrEmpty(_logger.Here(),new[] {"test"}, "property");
            validate.Should().NotThrow();
        }

        private class MockUser
        {
            //just a mock for testing
        }
    }
}