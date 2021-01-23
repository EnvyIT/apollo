using Apollo.Core.Implementation;
using Apollo.Core.Interfaces;
using Apollo.Util;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Apollo.Core.Test
{
    public class ServiceFactoryTest
    {

        private IServiceFactory _serviceFactory;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ConfigurationHelper.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.core.json")
                .Build();
        }

        [SetUp]
        public void Setup()
        {
            _serviceFactory = new ServiceFactory("Apollo_Test");
        }
        
        [Test]
        public void GetMovieService_ShouldReturnInstance()
        {
            var movieService = _serviceFactory.CreateMovieService();
            movieService.Should().NotBeNull();
        }

        [Test]
        public void GetMovieService_ShouldReturnSingleton()
        {
            var movieService = _serviceFactory.CreateMovieService();

            var anotherMovieService = _serviceFactory.CreateMovieService();

            movieService.Should().Be(anotherMovieService);
        }

        [Test]
        public void GetReservationService_ShouldReturnSingleton()
        {
            var reservationService = _serviceFactory.CreateReservationService();

            var anotherReservationService = _serviceFactory.CreateReservationService();

            reservationService.Should().Be(anotherReservationService);
        }

        [Test]
        public void GetInfrastructureService_ShouldReturnSingleton()
        {
            var infrastructureService = _serviceFactory.CreateInfrastructureService();

            var anotherInfrastructureService = _serviceFactory.CreateInfrastructureService();

            infrastructureService.Should().Be(anotherInfrastructureService);
        }

        [Test]
        public void GetUserService_ShouldReturnSingleton()
        {
            var userService = _serviceFactory.CreateUserService();

            var anotherUserService = _serviceFactory.CreateUserService();

            userService.Should().Be(anotherUserService);
        }

        [Test]
        public void GetScheduleService_ShouldReturnSingleton()
        {
            var scheduleService = _serviceFactory.CreateScheduleService();

            var anotherScheduleService = _serviceFactory.CreateScheduleService();

            scheduleService.Should().Be(anotherScheduleService);
        }

        [Test]
        public void GetCheckoutService_ShouldReturnSingleton()
        {
            var checkoutService = _serviceFactory.CreateCheckoutService();

            var anotherCheckoutService = _serviceFactory.CreateCheckoutService();

            checkoutService.Should().Be(anotherCheckoutService);
        }

    }
}
