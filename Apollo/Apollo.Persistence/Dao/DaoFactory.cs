using Apollo.Persistence.Dao.Ado;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao
{
    public class DaoFactory: IDaoFactory
    {
        private readonly IConnectionFactory _connectionFactory;

        public DaoFactory(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IActorDao CreateActorDao()
        {
            return new ActorDaoAdo(_connectionFactory);
        }

        public IAddressDao CreateAddressDao()
        {
            return new AddressDaoAdo(_connectionFactory);
        }

        public ICityDao CreateCityDao()
        {
            return new CityDaoAdo(_connectionFactory);
        }

        public ICinemaHallDao CreateCinemaHallDao()
        {
            return new CinemaHallDaoAdo(_connectionFactory);
        }

        public IGenreDao CreateGenreDao()
        {
            return new GenreDaoAdo(_connectionFactory);
        }

        public IMovieDao CreateMovieDao()
        {
            return new MovieDaoAdo(_connectionFactory);
        }

        public IMovieActorDao CreateMovieActorDao()
        {
            return new MovieActorDaoAdo(_connectionFactory);
        }

        public IReservationDao CreateReservationDao()
        {
            return new ReservationDaoAdo(_connectionFactory);
        }

        public IRoleDao CreateRoleDao()
        {
            return new RoleDaoAdo(_connectionFactory);
        }

        public IRowDao CreateRowDao()
        {
            return new RowDaoAdo(_connectionFactory);
        }

        public IRowCategoryDao CreateRowCategoryDao()
        {
            return new RowCategoryDaoAod(_connectionFactory);
        }

        public IScheduleDao CreateScheduleDao()
        {
            return new ScheduleDaoAdo(_connectionFactory);
        }

        public ISeatDao CreateSeatDao()
        {
            return new SeatDaoAdo(_connectionFactory);
        }

        public ISeatReservationDao CreateSeatReservationDao()
        {
            return new SeatReservationDaoAdo(_connectionFactory);
        }

        public IUserDao CreateUserDao()
        {
            return new UserDaoAdo(_connectionFactory);
        }

        public ITicketDao CreateTicketDao()
        {
            return new TicketDaoAdo(_connectionFactory);
        }
    }
}
