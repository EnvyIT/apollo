namespace Apollo.Persistence.Dao.Interfaces
{
    public interface IDaoFactory
    {
        IActorDao CreateActorDao();

        IAddressDao CreateAddressDao();

        ICityDao CreateCityDao();

        ICinemaHallDao CreateCinemaHallDao();

        IGenreDao CreateGenreDao();

        IMovieDao CreateMovieDao();

        IMovieActorDao CreateMovieActorDao();

        IReservationDao CreateReservationDao();

        IRoleDao CreateRoleDao();

        IRowDao CreateRowDao();

        IRowCategoryDao CreateRowCategoryDao();

        IScheduleDao CreateScheduleDao();

        ISeatDao CreateSeatDao();

        ISeatReservationDao CreateSeatReservationDao();

        IUserDao CreateUserDao();
        ITicketDao CreateTicketDao();
    }
}
