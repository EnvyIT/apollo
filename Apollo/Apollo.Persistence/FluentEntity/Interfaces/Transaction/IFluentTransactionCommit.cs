using System.Threading.Tasks;

namespace Apollo.Persistence.FluentEntity.Interfaces.Transaction
{
    public interface IFluentTransactionCommit
    {
        Task Commit();
    }
}
