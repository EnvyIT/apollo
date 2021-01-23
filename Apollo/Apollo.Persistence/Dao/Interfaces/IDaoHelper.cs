using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Persistence.Dao.Interfaces
{
    public interface IDaoHelper
    {
        Task<int> ExecuteAsync(string sql, params QueryParameter[] parameters);
        Task<int> ExecuteAsync(string sql);

        Task<T> SelectSingleAsync<T>(string sql, RowMapper<T> rowMapper, params QueryParameter[] parameters);
        Task<IEnumerable<T>> SelectAsync<T>(string sql, RowMapper<T> rowMapper, params QueryParameter[] parameters);
        Task PerformTransaction(Func<Task> callback);
        Task<int> ExecuteAsync<T>(IDictionary<string, IList<IList<QueryParameter>>> commands) where T : BaseEntity<T>, new();
        Task<IEnumerable<long>> ExecuteInsertAsync<T>(IDictionary<string, IList<IList<QueryParameter>>> commands) where T: BaseEntity<T>, new();
    }
}
