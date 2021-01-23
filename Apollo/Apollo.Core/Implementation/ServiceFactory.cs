using Apollo.Core.Interfaces;
using Apollo.Persistence.Util;
using Apollo.UnitOfWork;
using Apollo.UnitOfWork.Interfaces;
using Apollo.Util;

namespace Apollo.Core.Implementation
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly string _databaseJsonKey;

        private static IUnitOfWork _unitOfWork;
        private static ICheckoutService _checkoutService;
        private static IMovieService _movieService;
        private static IScheduleService _scheduleService;
        private static IUserService _userService;
        private static IInfrastructureService _infrastructureService;
        private static IReservationService _reservationService;

        public ServiceFactory(string databaseJsonKey)
        {
            _databaseJsonKey = databaseJsonKey;
        }

        public ICheckoutService CreateCheckoutService()
        {
            return SingletonHelper.GetInstance(ref _checkoutService,
                () => new CheckoutService(_unitOfWork, new PaymentFactory()));
        }

        public IMovieService CreateMovieService()
        {
            return SingletonHelper.GetInstance(ref _movieService, () => new MovieService(GetUnitOfWorkInstance()));
        }

        public IScheduleService CreateScheduleService()
        {
            return SingletonHelper.GetInstance(ref _scheduleService,
                () => new ScheduleService(GetUnitOfWorkInstance()));
        }

        public IUserService CreateUserService()
        {
            return SingletonHelper.GetInstance(ref _userService, () => new UserService(GetUnitOfWorkInstance()));
        }

        public IInfrastructureService CreateInfrastructureService()
        {
            return SingletonHelper.GetInstance(ref _infrastructureService,
                () => new InfrastructureService(GetUnitOfWorkInstance()));
        }

        public IReservationService CreateReservationService()
        {
            return SingletonHelper.GetInstance(ref _reservationService,
                () => new ReservationService(GetUnitOfWorkInstance(), CreateInfrastructureService()));
        }

        private IUnitOfWork GetUnitOfWorkInstance()
        {
            return SingletonHelper.GetInstance(ref _unitOfWork, () =>
            {
                var appSettings = ConfigurationHelper.GetValues(_databaseJsonKey);
                return new UnitOfWorkFactory().Create(new ConnectionFactory(appSettings[0]));
            });
        }
    }
}