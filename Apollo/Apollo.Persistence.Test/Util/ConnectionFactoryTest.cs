using System;
using System.Data.Common;
using System.Threading.Tasks;
using Apollo.Persistence.Util;
using Apollo.Util;
using FluentAssertions;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace Apollo.Persistence.Test.Util
{
    public class ConnectionFactoryTest
    {
        private const string ConnectionString = "connection";

        [Test]
        public void CreateConnectionFactoryWithoutProviderType_ShouldReturnDefaultProviderMySql()
        {
            var connectionFactory = new ConnectionFactory(ConnectionString);
            connectionFactory.ConnectionString.Should().Be(ConnectionString);
            connectionFactory.ProviderType.Should().Be(ProviderType.MySQL);
        }

        [Test]
        public void CreateConnectionFactoryWithProviderType_ShouldReturnChosenProviderType()
        {
            const ProviderType mysqlProvider = ProviderType.SQLServer;
            var connectionFactory = new ConnectionFactory(ConnectionString, mysqlProvider);
            connectionFactory.ConnectionString.Should().Be(ConnectionString);
            connectionFactory.ProviderType.Should().Be(mysqlProvider);
        }

        [Test]
        public void CreateConnectionFactoryWithNonSupportedProvider_ShouldThrowArgumentException()
        {
            const ProviderType nonExistingProvider = (ProviderType) 1337;
            Action createFactory = () => new ConnectionFactory(ConnectionString, nonExistingProvider);
            createFactory.Should().ThrowExactly<ArgumentException>();
        }

        [Test] 
        public void CreateExecutionStrategy_ShouldReturnMysqlStrategy()
        {
            var connectionFactory = new ConnectionFactory(ConnectionString);
            var strategy = connectionFactory.CreateIdentityExecutionStrategy();
            strategy.Method.Name.Should().Be("MySqlIdentity");
        }


        [Test]
        public async Task CreateExecutionStrategy_ShouldExecuteMysqlStrategyCorrectly()
        {
            var connectionFactory = new ConnectionFactory(ConnectionString, ProviderType.SQLServer);
            var strategy = connectionFactory.CreateIdentityExecutionStrategy();
            var dbCommand = new MockMsSqlCommand(); 
            var result = await strategy.Invoke(dbCommand);
            dbCommand.CommandText.Should().Be(";SELECT SCOPE_IDENTITY();");
            result.Should().Be(0L);
        }

        [Test]
        public void CreateExecutionStrategy_ShouldReturnMSSqlStrategy()
        {
            const ProviderType mysqlProvider = ProviderType.SQLServer;
            var connectionFactory = new ConnectionFactory(ConnectionString, mysqlProvider);
            var strategy = connectionFactory.CreateIdentityExecutionStrategy();
            strategy.Method.Name.Should().Be("SqlIdentity");
        }

    }
}
