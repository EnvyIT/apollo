using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Apollo.Persistence.Util
{
    public class ConnectionFactory : IConnectionFactory
    {

        private readonly DbProviderFactory _dbProviderFactory;
        private readonly Func<DbCommand, Task<long>> _identityCommand;

        private const string SqlIdentityStatement = ";SELECT SCOPE_IDENTITY();";

        public string ConnectionString { get; }
        public ProviderType ProviderType { get; }
        public Func<DbCommand, Task<long>> CreateIdentityExecutionStrategy()
        {
            return _identityCommand;
        }

        public ConnectionFactory(string connectionString, ProviderType providerType = ProviderType.MySQL)
        {
            ConnectionString = connectionString;
            ProviderType = providerType;

            switch (providerType)
            {
                case ProviderType.MySQL:
                    _dbProviderFactory = MySqlClientFactory.Instance;
                    _identityCommand = MySqlIdentity;
                    break;
                case ProviderType.SQLServer:
                    _dbProviderFactory = SqlClientFactory.Instance;
                    _identityCommand = SqlIdentity;
                    break;
                default:
                    throw new ArgumentException($"No provider found for {providerType}.");
            }
        }

        private static async Task<long> SqlIdentity(DbCommand arg)
        {
            arg.CommandText += SqlIdentityStatement;
            var id = await  arg.ExecuteScalarAsync();
            return (long) id;
        }

        private static async Task<long> MySqlIdentity(DbCommand arg)
        {
           await arg.ExecuteNonQueryAsync();
           return ((MySqlCommand) arg).LastInsertedId;
        }


        public async Task<DbConnection> CreateConnectionAsync()
        {
            var connection = _dbProviderFactory.CreateConnection() ??
                             throw new ArgumentNullException($"{nameof(CreateConnectionAsync)} could not establish connection - {nameof(DbProviderFactory)} created null");
            connection.ConnectionString = this.ConnectionString;
            await connection.OpenAsync();
            return connection;
        }

    }
}
