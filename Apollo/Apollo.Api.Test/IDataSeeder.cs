using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.FluentEntity.Interfaces;

namespace Apollo.Api.Test
{
    public interface IDataSeeder
    {
        /// <summary>
        /// If the database is empty it will be seeded.
        /// If the database is not empty, the whole database will be purged and re-seeded.
        /// </summary>
        /// <returns>Task</returns>
        Task SeedDatabaseAsync();

        Task ResetDatabaseAsync();

        public IList<Seat> Seats { get; }
        public IList<Row> Rows { get; }
        public IList<Reservation> Reservations { get; }
        public IList<CinemaHall> CinemaHalls { get; }
        public IList<MovieActor> MovieActors { get; }
        public IList<Actor> Actors { get; }
        public IList<Movie> Movies { get; }
        public IList<Genre> Genres { get; }
        public IList<User> Users { get; }
        public IList<Address> Addresses { get; }
        public IList<City> Cities { get; }
        public IList<Role> Roles { get; }
        public IList<SeatReservation> SeatReservations { get; }
        public IList<Schedule> Schedules { get; }
        public IList<RowCategory> RowCategories { get; }

        public IEntityManager EntityManager { get; }
    }
}
