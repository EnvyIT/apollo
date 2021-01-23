using Apollo.Persistence.FluentEntity;
using Apollo.Persistence.FluentEntity.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.Test.Entity.Helper;
using Apollo.Persistence.Util;
using Apollo.Util;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Apollo.Persistence.Test.Entity
{
    public class FluentEntityBaseTest
    {
        protected IEntityManager _entityManager;
        protected IFluentEntityFrom _fluentEntity;
        protected FluentEntitySelectTestHelper _selectHelper;
        protected FluentEntityDeleteTestHelper _deleteHelper;
        protected FluentEntityUpdateTestHelper _updateHelper;
        protected FluentEntityInsertTestHelper _insertHelper;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ConfigurationHelper.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json")
                .Build();
            var appSettings = ConfigurationHelper.GetValues("Apollo_Test");
            var connectionFactory = new ConnectionFactory(appSettings[0]);
            _entityManager = EntityManagerFactory.CreateEntityManager(connectionFactory);
            _fluentEntity = _entityManager.FluentEntity();
            _selectHelper = new FluentEntitySelectTestHelper(_entityManager);
            _deleteHelper = new FluentEntityDeleteTestHelper(_entityManager);
            _updateHelper = new FluentEntityUpdateTestHelper(_entityManager);
            _insertHelper = new FluentEntityInsertTestHelper(_entityManager);
        }

    }
}
