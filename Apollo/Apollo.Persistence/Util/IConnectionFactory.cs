using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Apollo.Persistence.Util
{
    public interface IConnectionFactory
    {
        Task<DbConnection> CreateConnectionAsync();

        ProviderType ProviderType { get; }
        Func<DbCommand, Task<long>> CreateIdentityExecutionStrategy();
    }
}
