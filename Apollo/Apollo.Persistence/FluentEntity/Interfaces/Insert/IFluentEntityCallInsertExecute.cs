using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;

namespace Apollo.Persistence.FluentEntity.Interfaces.Insert
{
   public interface IFluentEntityCallInsertExecute : ICallExecute
    {
        new Task<IEnumerable<long>> ExecuteAsync();
    }
}
