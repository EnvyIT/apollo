using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apollo.Persistence.FluentEntity.Interfaces.Select
{
    public interface IFluentEntityCall<T>
    {
        Task<IEnumerable<T>> QueryAsync();

        Task<T> QuerySingleAsync();
      
    }
}
