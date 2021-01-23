using System;
using Apollo.Persistence.Util;
using Apollo.UnitOfWork.Interfaces;
using Apollo.Util;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Apollo.UnitOfWork.Test
{
    public class Tests
    {
        private IUnitOfWorkFactory _unitOfWorkFactory;
        private IConnectionFactory _connectionFactory;

        [OneTimeSetUp]
        public void Setup()
        {
            ConfigurationHelper.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json")
                .Build();
            var appSettings = ConfigurationHelper.GetValues("Apollo_Test");

            _connectionFactory = new ConnectionFactory(appSettings[0]);
            _unitOfWorkFactory = new UnitOfWorkFactory();
        }

        [Test]
        public void Test_Factory_ReturnInstance_WithoutError()
        {
            Action createInstance = () => _unitOfWorkFactory.Create(_connectionFactory);
            createInstance.Should().NotThrow();
        }

        [Test]
        public void Test_UnitOfWork_Repositories_Initialized()
        {
            var unitOfWork = _unitOfWorkFactory.Create(_connectionFactory);

            unitOfWork.RepositoryInfrastructure.Should().NotBeNull();
            unitOfWork.RepositoryMovie.Should().NotBeNull();
            unitOfWork.RepositorySchedule.Should().NotBeNull();
            unitOfWork.RepositoryTicket.Should().NotBeNull();
        }
    }
}