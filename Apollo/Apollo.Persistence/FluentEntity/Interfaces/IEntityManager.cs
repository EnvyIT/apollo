using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.FluentEntity.Interfaces.Transaction;

namespace Apollo.Persistence.FluentEntity.Interfaces
{
    public interface IEntityManager
    {
        IFluentEntityFrom FluentEntity();

        IFluentTransaction FluentTransaction();
    }
}
