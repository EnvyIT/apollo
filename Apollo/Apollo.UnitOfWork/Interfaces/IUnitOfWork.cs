using Apollo.Persistence.FluentEntity.Interfaces.Transaction;
using Apollo.Repository.Interfaces;

namespace Apollo.UnitOfWork.Interfaces
{
    public interface IUnitOfWork
    {
        public IRepositoryInfrastructure RepositoryInfrastructure { get; }

        public IRepositoryMovie RepositoryMovie { get; }

        public IRepositorySchedule RepositorySchedule { get; }

        public IRepositoryTicket RepositoryTicket { get; }

        public IRepositoryUser RepositoryUser { get; }

        IFluentTransaction Transaction();
    }
}
