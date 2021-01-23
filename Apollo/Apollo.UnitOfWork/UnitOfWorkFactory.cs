using Apollo.Persistence.Util;
using Apollo.UnitOfWork.Interfaces;

namespace Apollo.UnitOfWork
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private static readonly object Lock = new object();
        private static IUnitOfWork _unitOfWork;
        
        public IUnitOfWork Create(IConnectionFactory connectionFactory)
        {
            lock (Lock)
            {
                _unitOfWork ??= new UnitOfWork(connectionFactory);
            }
            return _unitOfWork;
        }
    }
}
