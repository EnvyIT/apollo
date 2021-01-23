using System;
using Apollo.Util.Logger;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace Apollo.Util.Test
{
    public class LoggerTest
    {
        private Mock<ILogger> _mockLogger;
        private ILogger _logger;
        private ApolloLogger<LoggerTest> _apolloLogger;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger>();
            _logger = _mockLogger.Object;
            _apolloLogger = new ApolloLogger<LoggerTest>(_logger);
            _mockLogger.Setup(_ => _.ForContext(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<bool>()))
                .Returns(_logger);
            _mockLogger.Setup(_ => _.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
                .Returns(_logger);
        }

        [Test]
        public void CreateLogger_ShouldReturnInstance()
        {
            var logger = LoggerFactory.CreateLogger<LoggerTest>();
            logger.Should().NotBeNull();
        }


        [Test]
        public void Info_ShouldBeCalled()
        {
            var exception = new NullReferenceException();
            const string message = "Entity was null!";
            _mockLogger.Setup(_ => _.Information(exception, message));

            _apolloLogger.Here().Info(exception, message);
            _mockLogger.Verify(_ => _.Information(exception, message, It.IsAny<object[]>()), Times.Once);
            _mockLogger.Verify(_ => _.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()),
                Times.Exactly(3));
        }


        [Test]
        public void Debug_ShouldBeCalled()
        {
            var exception = new NullReferenceException();
            const string message = "Entity was null!";

            _apolloLogger.Here().Debug(exception, message);
            _mockLogger.Verify(_ => _.Debug(exception, message, It.IsAny<object[]>()), Times.Once);
            _mockLogger.Verify(_ => _.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()),
                Times.Exactly(3));
        }


        [Test]
        public void Warning_ShouldBeCalled()
        {
            var exception = new NullReferenceException();
            const string message = "Entity was null!";

            _apolloLogger.Here().Warning(exception, message);
            _mockLogger.Verify(_ => _.Warning(exception, message, It.IsAny<object[]>()), Times.Once);
            _mockLogger.Verify(_ => _.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()),
                Times.Exactly(3));
        }


        [Test]
        public void Error_ShouldBeCalled()
        {
            var exception = new NullReferenceException();
            const string message = "Entity was null!";

            _apolloLogger.Here().Error(exception, message);
            _mockLogger.Verify(_ => _.Error(exception, message, It.IsAny<object[]>()), Times.Once);
            _mockLogger.Verify(_ => _.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()),
                Times.Exactly(3));
        }

        [Test]
        public void Verbose_ShouldBeCalled()
        {
            var exception = new NullReferenceException();
            const string message = "Entity was null!";

            _apolloLogger.Here().Verbose(exception, message);
            _mockLogger.Verify(_ => _.Verbose(exception, message, It.IsAny<object[]>()), Times.Once);
            _mockLogger.Verify(_ => _.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()),
                Times.Exactly(3));
        }

        [Test]
        public void Fatal_ShouldBeCalled()
        {
            var exception = new NullReferenceException();
            const string message = "Entity was null!";

            _apolloLogger.Here().Fatal(exception, message);
            _mockLogger.Verify(_ => _.Fatal(exception, message, It.IsAny<object[]>()), Times.Once);
            _mockLogger.Verify(_ => _.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()),
                Times.Exactly(3));
        }


        [Test]
        public void InfoMessage_ShouldBeCalled()
        {
            const string message = "Entity was null!";

            _apolloLogger.Here().Info(message);
            _mockLogger.Verify(_ => _.Information(message, It.IsAny<object[]>()), Times.Once);
            _mockLogger.Verify(_ => _.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()),
                Times.Exactly(3));
        }


        [Test]
        public void DebugMessage_ShouldBeCalled()
        {
            const string message = "Entity was null!";

            _apolloLogger.Here().Debug(message);
            _mockLogger.Verify(_ => _.Debug(message, It.IsAny<object[]>()), Times.Once);
            _mockLogger.Verify(_ => _.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()),
                Times.Exactly(3));
        }


        [Test]
        public void WarningMessage_ShouldBeCalled()
        {
            const string message = "Entity was null!";

            _apolloLogger.Here().Warning(message);
            _mockLogger.Verify(_ => _.Warning(message, It.IsAny<object[]>()), Times.Once);
            _mockLogger.Verify(_ => _.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()),
                Times.Exactly(3));
        }


        [Test]
        public void ErrorMessage_ShouldBeCalled()
        {
            const string message = "Entity was null!";

            _apolloLogger.Here().Error(message);
            _mockLogger.Verify(_ => _.Error(message, It.IsAny<object[]>()), Times.Once);
            _mockLogger.Verify(_ => _.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()),
                Times.Exactly(3));
        }

        [Test]
        public void VerboseMessage_ShouldBeCalled()
        {
            const string message = "Entity was null!";

            _apolloLogger.Here().Verbose(message);
            _mockLogger.Verify(_ => _.Verbose(message, It.IsAny<object[]>()), Times.Once);
            _mockLogger.Verify(_ => _.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()),
                Times.Exactly(3));
        }

        [Test]
        public void FatalMessage_ShouldBeCalled()
        {
            const string message = "Entity was null!";

            _apolloLogger.Here().Fatal(message);
            _mockLogger.Verify(_ => _.Fatal(message, It.IsAny<object[]>()), Times.Once);
            _mockLogger.Verify(_ => _.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()),
                Times.Exactly(3));
        }
    }
}