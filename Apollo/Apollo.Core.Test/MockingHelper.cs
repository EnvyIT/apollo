using System;
using System.Threading.Tasks;
using Apollo.Core.Implementation;
using Apollo.Core.Interfaces;
using Apollo.Core.Test.Mocks;
using Apollo.Core.Types;
using Apollo.Persistence.FluentEntity.Interfaces.Transaction;
using Apollo.Repository.Interfaces;
using Apollo.UnitOfWork.Interfaces;
using Moq;

namespace Apollo.Core.Test
{
    public class MockingHelper
    {
        public Mock<IServiceFactory> ServiceFactory { get; }
        public Mock<IPaymentFactory> PaymentFactory { get; }
        public PaymentMock PaymentMock { get; }
        public Mock<IUnitOfWorkFactory> UnitOfWorkFactory { get; }
        public Mock<IUnitOfWork> UnitOfWork { get; }
        public Mock<IRepositoryInfrastructure> RepositoryInfrastructure { get; }
        public Mock<IRepositoryMovie> RepositoryMovie { get; }
        public Mock<IRepositorySchedule> RepositorySchedule { get; }
        public Mock<IRepositoryTicket> RepositoryTicket { get; }
        public Mock<IRepositoryUser> RepositoryUser { get; }
        public Mock<IFluentTransaction> FluentTransaction { get; }
        public Mock<IFluentTransactionCommit> FluentTransactionCommit { get; }

        public MockingHelper()
        {
            ServiceFactory = new Mock<IServiceFactory>();
            UnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
            UnitOfWork = new Mock<IUnitOfWork>();
            RepositoryInfrastructure = new Mock<IRepositoryInfrastructure>();
            RepositoryMovie = new Mock<IRepositoryMovie>();
            RepositorySchedule = new Mock<IRepositorySchedule>();
            RepositoryTicket = new Mock<IRepositoryTicket>();
            RepositoryUser = new Mock<IRepositoryUser>();

            FluentTransaction = new Mock<IFluentTransaction>();
            FluentTransactionCommit = new Mock<IFluentTransactionCommit>();

            FluentTransactionCommit = new Mock<IFluentTransactionCommit>();
            FluentTransaction.Setup(_ => _.PerformBlock(It.IsAny<Func<Task>>()))
                .Returns((Func<Task> block) =>
                {
                    block().Wait();
                    return FluentTransactionCommit.Object;
                });

            UnitOfWorkFactory.Setup(_ => _.Create(null)).Returns(UnitOfWork.Object);

            UnitOfWork.Setup(_ => _.RepositoryInfrastructure).Returns(RepositoryInfrastructure.Object);
            UnitOfWork.Setup(_ => _.RepositoryMovie).Returns(RepositoryMovie.Object);
            UnitOfWork.Setup(_ => _.RepositorySchedule).Returns(RepositorySchedule.Object);
            UnitOfWork.Setup(_ => _.RepositoryTicket).Returns(RepositoryTicket.Object);
            UnitOfWork.Setup(_ => _.RepositoryUser).Returns(RepositoryUser.Object);
            UnitOfWork.Setup(_ => _.Transaction()).Returns(FluentTransaction.Object);

            PaymentMock = new PaymentMock();
            PaymentFactory = new Mock<IPaymentFactory>();
            PaymentFactory.Setup(_ => _.CreatePayment(PaymentType.FhPay)).Returns(PaymentMock);

            ServiceFactory.Setup(_ => _.CreateCheckoutService())
                .Returns(new CheckoutService(UnitOfWork.Object, PaymentFactory.Object));
            ServiceFactory.Setup(_ => _.CreateMovieService()).Returns(new MovieService(UnitOfWork.Object));
            ServiceFactory.Setup(_ => _.CreateScheduleService()).Returns(new ScheduleService(UnitOfWork.Object));
            ServiceFactory.Setup(_ => _.CreateUserService()).Returns(new UserService(UnitOfWork.Object));
            ServiceFactory.Setup(_ => _.CreateInfrastructureService()).Returns(new InfrastructureService(UnitOfWork.Object));
            ServiceFactory.Setup(_ => _.CreateReservationService()).Returns(new ReservationService(UnitOfWork.Object, ServiceFactory.Object.CreateInfrastructureService()));
        }
    }
}