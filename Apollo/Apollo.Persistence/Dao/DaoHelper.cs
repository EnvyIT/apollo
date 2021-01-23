using System;
using Apollo.Persistence.Util;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Apollo.Persistence.Attributes.Base;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Util.Logger;

namespace Apollo.Persistence.Dao
{
    public class DaoHelper : IDaoHelper
    {
        private readonly IApolloLogger<DaoHelper> Logger = LoggerFactory.CreateLogger<DaoHelper>();

        private readonly IConnectionFactory _connectionFactory;
        private readonly Func<DbCommand, Task<long>> _executeInsertIdentityAsync;

        public DaoHelper(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _executeInsertIdentityAsync = _connectionFactory.CreateIdentityExecutionStrategy();
        }

        public async Task<int> ExecuteAsync(string sql, params QueryParameter[] parameters)
        {
            await using var connection = await _connectionFactory.CreateConnectionAsync();
            await using var command = connection.CreateCommand();

            command.CommandText = sql;
            await command.PrepareAsync();

            SetParams(parameters, command);
            return await TryExecuteStatement(command.ExecuteNonQueryAsync, sql, parameters);
        }

        public async Task<int> ExecuteAsync(string sql)
        {
            await using var connection = await _connectionFactory.CreateConnectionAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            await command.PrepareAsync();
         
            return await TryExecuteStatement(command.ExecuteNonQueryAsync, sql);
        }

        public async Task<T> SelectSingleAsync<T>(string sql, RowMapper<T> rowMapper,
            params QueryParameter[] parameters)
        {
            return (await SelectAsync(sql, rowMapper, parameters)).SingleOrDefault();
        }

        public async Task<IEnumerable<T>> SelectAsync<T>(string sql, RowMapper<T> rowMapper,
            params QueryParameter[] parameters)
        {
            await using var connection = await _connectionFactory.CreateConnectionAsync();
            await using var command = connection.CreateCommand();

            command.CommandText = sql;
            await command.PrepareAsync();

            SetParams(parameters, command);

            await using var reader = await TryReaderAsync(command.ExecuteReaderAsync, sql, parameters);
            
            var resultSet = new List<T>();
            while (await reader.ReadAsync())
            {
                resultSet.Add(rowMapper(reader));
            }

            return resultSet;
        }


        public async Task<IEnumerable<long>> ExecuteInsertAsync<T>(IDictionary<string, IList<IList<QueryParameter>>> commands) where T : BaseEntity<T>, new()
        {
            await using var connection = await _connectionFactory.CreateConnectionAsync();
            await using var command = connection.CreateCommand();
            var ids = new List<long>();
            foreach (var (insertCommand, queryParameters) in commands)
            {
                command.CommandText = $"{insertCommand}";
                await command.PrepareAsync();
                foreach (var queryParameter in queryParameters)
                {
                    SetParams(queryParameter, command);
                    ids.Add(await TryExecuteStatement(() => _executeInsertIdentityAsync(command), insertCommand));
                }
            }

            return ids;
        }

        
        public async Task<int> ExecuteAsync<T>(IDictionary<string, IList<IList<QueryParameter>>> commands) where T : BaseEntity<T>, new()
        {
            await using var connection = await _connectionFactory.CreateConnectionAsync();
            await using var command = connection.CreateCommand();
            int count = 0;
            foreach (var (insertCommand, queryParameters) in commands)
            {
                command.CommandText = $"{insertCommand}";
                await command.PrepareAsync();
                foreach (var queryParameter in queryParameters)
                {
                    SetParams(queryParameter, command);
                    count +=  await TryExecuteStatement(command.ExecuteNonQueryAsync, insertCommand, queryParameter.ToArray());
                }
            }

            return count;
        }

        public async Task PerformTransaction(Func<Task> callback)
        {
            using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                TransactionScopeAsyncFlowOption.Enabled
            );
            await callback.Invoke();
            scope.Complete();
        }

        private static void SetParams(IEnumerable<QueryParameter> parameters, DbCommand command)
        {
            command.Parameters.Clear();
            foreach (var param in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = param.Name;
                parameter.Value = param.Value;
                command.Parameters.Add(parameter);
            }
        }

        private async Task<int> TryExecuteStatement(Func<Task<int>> command, string sql, params QueryParameter[] parameters)
        {
            try
            {
                return await command();
            }
            catch (DbException dbException)
            {
                if (parameters.Length == 0)
                {
                    Logger.Error(dbException, "{Query} failed", sql);
                }
                else
                {
                    Logger.Error(dbException, "{Query} failed with {parameters}", sql, parameters);
                }
                throw;
            }
        }

        private async Task<long> TryExecuteStatement(Func<Task<long>> command, string sql)
        {
            try
            {
                return await command();
            }
            catch (DbException dbException)
            {
             
                Logger.Error(dbException, "{Query} failed", sql);
                throw;
            }
        }

        private async Task<DbDataReader> TryReaderAsync(Func<Task<DbDataReader>> command, string sql, params QueryParameter[] parameters)
        {
            try
            {
                return await command();
            }
            catch (DbException dbException)
            {

                Logger.Error(dbException, "{Query} failed with {parameters}", sql, parameters);
                throw;
            }
            catch (ArgumentException argumentException)
            {
                Logger.Error(argumentException, "{Query} failed with {parameters}", sql, parameters);
                throw;
            }

        }

    }
}
