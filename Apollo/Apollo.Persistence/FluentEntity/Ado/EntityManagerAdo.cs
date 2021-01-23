using System;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.FluentEntity.Interfaces.Transaction;

namespace Apollo.Persistence.FluentEntity.Ado
{
    public class EntityManagerAdo : IEntityManager
    {
        private readonly IDaoHelper _daoHelper;

        public EntityManagerAdo(IDaoHelper daoHelper)
        {
            _daoHelper = daoHelper ?? throw new ArgumentNullException(nameof(daoHelper));
        }

        public IFluentEntityFrom FluentEntity()
        {
            return new FluentEntityAdo(_daoHelper);
        }

        public IFluentTransaction FluentTransaction()
        {
            return new FluentTransactionAdo(_daoHelper);
        }
    }
}
