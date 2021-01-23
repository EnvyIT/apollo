using Apollo.Persistence.Dao;
using Apollo.Persistence.FluentEntity.Ado;
using Apollo.Persistence.FluentEntity.Interfaces;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.FluentEntity
{
    public static class EntityManagerFactory
    {
        public static IEntityManager CreateEntityManager(IConnectionFactory connectionFactory)
        {
            return new EntityManagerAdo(new DaoHelper(connectionFactory));
        }
    }
}
