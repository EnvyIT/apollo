using System.Threading.Tasks;

namespace Apollo.Persistence.FluentEntity.Interfaces.Shared
{
    public interface IFluentEntityCallExecute : ICallExecute
    {
        new Task<int> ExecuteAsync();
    }
}
