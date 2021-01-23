namespace Apollo.Repository.Interfaces
{
    public interface IRepositoryFactory
    {
        IRepositoryInfrastructure CreateRepositoryInfrastructure();

        IRepositoryMovie CreateRepositoryMovie();

        IRepositorySchedule CreateRepositorySchedule();

        IRepositoryTicket CreateRepositoryTicket();

        IRepositoryUser CreateRepositoryUser();
    }
}
