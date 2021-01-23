namespace Apollo.Core.Interfaces
{
    public interface IServiceFactory
    {
        ICheckoutService CreateCheckoutService();

        IMovieService CreateMovieService();

        IScheduleService CreateScheduleService();

        IUserService CreateUserService();
        IInfrastructureService CreateInfrastructureService();
        IReservationService CreateReservationService();
    }
}
