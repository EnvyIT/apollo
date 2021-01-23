using Apollo.Persistence.Util;

namespace Apollo.UnitOfWork.Interfaces
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(IConnectionFactory connectionFactory);
    }
}
