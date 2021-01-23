using Apollo.Persistence.Util;
using Apollo.Repository.Implementation;
using Apollo.Repository.Interfaces;

namespace Apollo.Repository
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IConnectionFactory _connectionFactory;

        public RepositoryFactory(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IRepositoryInfrastructure CreateRepositoryInfrastructure()
        {
            return new RepositoryInfrastructure(_connectionFactory);
        }

        public IRepositoryMovie CreateRepositoryMovie()
        {
            return new RepositoryMovie(_connectionFactory);
        }

        public IRepositorySchedule CreateRepositorySchedule()
        {
            return new RepositorySchedule(_connectionFactory);
        }

        public IRepositoryTicket CreateRepositoryTicket()
        {
            return new RepositoryTicket(_connectionFactory);
        }

        public IRepositoryUser CreateRepositoryUser()
        {
            return new RepositoryUser(_connectionFactory);
        }
    }
}
