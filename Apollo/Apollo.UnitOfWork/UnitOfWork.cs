using Apollo.Persistence.FluentEntity;
using Apollo.Persistence.FluentEntity.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Transaction;
using Apollo.Persistence.Util;
using Apollo.Repository;
using Apollo.Repository.Interfaces;
using Apollo.UnitOfWork.Interfaces;

namespace Apollo.UnitOfWork
{
    public class UnitOfWork: IUnitOfWork
    {
        public IRepositoryInfrastructure RepositoryInfrastructure { get; }

        public IRepositoryMovie RepositoryMovie { get; }

        public IRepositorySchedule RepositorySchedule { get; }

        public IRepositoryTicket RepositoryTicket { get; }

        public IRepositoryUser RepositoryUser { get; }

        private readonly IEntityManager _entityManager;

        public UnitOfWork(IConnectionFactory connectionFactory)
        {
            var repositoryFactory = new RepositoryFactory(connectionFactory);

            _entityManager = EntityManagerFactory.CreateEntityManager(connectionFactory);

            RepositoryInfrastructure = repositoryFactory.CreateRepositoryInfrastructure();
            RepositoryMovie = repositoryFactory.CreateRepositoryMovie();
            RepositorySchedule = repositoryFactory.CreateRepositorySchedule();
            RepositoryTicket = repositoryFactory.CreateRepositoryTicket();
            RepositoryUser = repositoryFactory.CreateRepositoryUser();
        }

        public IFluentTransaction Transaction()
        {
            return _entityManager.FluentTransaction();
        }

    }
}
