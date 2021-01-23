using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Import.Services;
using Apollo.Persistence.Util;
using Apollo.UnitOfWork;
using Apollo.UnitOfWork.Interfaces;
using Apollo.Util;
using Apollo.Util.Logger;
using Microsoft.Extensions.Configuration;


namespace Apollo.Import
{
    public class Program
    {
        private static readonly IApolloLogger<Program> Logger = LoggerFactory.CreateLogger<Program>();

        static async Task Main(string[] args)
        {
            ConfigurationHelper.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.import.json")
                .Build();

            await Import(new UnitOfWorkFactory().Create(
                new ConnectionFactory(ConfigurationHelper.GetValues("Apollo_Dev")[0]))
            );
        }

        private static async Task Import(IUnitOfWork unitOfWork)
        {
            var watch = Stopwatch.StartNew();

            await ImportMovies(unitOfWork);
            await ImportInfrastructure(unitOfWork);
            await ImportSchedules(unitOfWork);
            await ImportUser(unitOfWork);

            Logger.Info($"Import successfully finished after {watch.Elapsed.ToHumanReadable()}");
        }

        private static async Task ImportMovies(IUnitOfWork unitOfWork)
        {
            var movies = await unitOfWork.RepositoryMovie.GetMoviesAsync();
            if (movies.Any())
            {
                Logger.Info("Skipping movie import - already exist...");
                return;
            }

            Logger.Info("Starting import from IMDB-API...");
            movies = ImdbHttpService.GetImdbData(out var genres, out var actors, out var movieActors);
            var actorsSet = actors.Select(a => a.Value).ToHashSet();
            await Task.WhenAll(genres.Select(async genre => await unitOfWork.RepositoryMovie.AddGenreAsync(genre)));
            Logger.Info("Persisted genres");
            await Task.WhenAll(movies.Select(async movie => await unitOfWork.RepositoryMovie.AddMovieAsync(movie)));
            Logger.Info("Persisted movies");
            await Task.WhenAll(actorsSet.Select(async actor => await unitOfWork.RepositoryMovie.AddActorAsync(actor)));
            Logger.Info("Persisted actors");
            await Task.WhenAll(movieActors.Select(async movieActor =>
                await unitOfWork.RepositoryMovie.AddActorToMovieAsync(movieActor.MovieId, movieActor.ActorId)));
            Logger.Info("Persisted movie actors");
        }

        private static async Task ImportSchedules(IUnitOfWork unitOfWork)
        {
            var schedules = await unitOfWork.RepositorySchedule.GetSchedulesAsync();
            if (schedules.Any())
            {
                Logger.Info("Skipping schedule import - already exist...");
                return;
            }

            var movies = await unitOfWork.RepositoryMovie.GetMoviesAsync();
            var today = DateTime.UtcNow;
            var nextYear = today.AddYears(3);
            var from = today.GetBeginningOfMonth();
            var to = nextYear.GetEndOfMonth();
            await unitOfWork.RepositorySchedule.AddSchedulesForMoviesAsync(from, to, movies);
        }

        private static async Task ImportInfrastructure(IUnitOfWork unitOfWork)
        {
            var cinemaHalls = (await unitOfWork.RepositoryInfrastructure.GetCinemaHallsAsync()).ToList();
            if (cinemaHalls.Any())
            {
                Logger.Info("Skipping cinema infrastructure import - already exist...");
            }
            else
            {
                const string cinemaHallName = "Hall of Stars";
                const int sizeRow = 20;
                const int sizeColumn = 30;

                var cinemaHall = new CinemaHall
                    {Id = 1L, Label = cinemaHallName, SizeRow = sizeRow, SizeColumn = sizeColumn};
                await unitOfWork.RepositoryInfrastructure.AddCinemaHallAsync(cinemaHall);
                cinemaHalls.Add(cinemaHall);

                Logger.Info($"Persisted cinema hall \"{cinemaHallName}\"");
            }

            var categories = new[] {"Standard", "Premium", "Deluxe"};
            var categoryIds = (await ImportInfrastructureRowCategories(unitOfWork, categories)).ToList();

            foreach (var cinemaHall in cinemaHalls)
            {
                await ImportInfrastructureSeats(unitOfWork, cinemaHall.Id, categoryIds, cinemaHall.SizeRow,
                    cinemaHall.SizeColumn);
            }
        }

        private static async Task<IEnumerable<long>> ImportInfrastructureRowCategories(IUnitOfWork unitOfWork,
            string[] categories)
        {
            var rowCategories = (await unitOfWork.RepositoryInfrastructure.GetActiveRowCategoriesAsync()).ToList();
            if (rowCategories.Any())
            {
                Logger.Info("Skipping row category import - already exist...");
                return rowCategories.Select(category => category.Id);
            }

            var ids = new List<long>();

            for (var i = 0; i < categories.Length; i++)
            {
                ids.Add(await unitOfWork.RepositoryInfrastructure.AddRowCategoryAsync(new RowCategory
                {
                    Id = (long) i + 1,
                    Name = categories[i],
                    PriceFactor = 1 + i * .25
                }));
            }

            Logger.Info("Persisted row categories");
            return ids;
        }

        private static async Task ImportInfrastructureSeats(IUnitOfWork unitOfWork, long cinemaHallId,
            IReadOnlyList<long> categoryIds, int sizeRow, int sizeColumn)
        {
            var rows = await unitOfWork.RepositoryInfrastructure.GetRowsFromCinemaHallAsync(cinemaHallId);
            if (rows.Any())
            {
                Logger.Info("Skipping row and seat import - already exist...");
                return;
            }

            var corridorIndex1 = (int) (sizeColumn * .25);
            var corridorIndex2 = (int) (sizeColumn * .75);
            var rowFactor = (double) sizeRow / categoryIds.Count;

            var seatId = 1L;
            for (var row = 1; row < sizeRow; row += 2)
            {
                var rowNumber = row / 2 + 1;
                var rowId = await unitOfWork.RepositoryInfrastructure.AddRowAsync(new Row
                {
                    Id = rowNumber,
                    CinemaHallId = cinemaHallId,
                    CategoryId = categoryIds[(int) (row / rowFactor)],
                    Number = rowNumber
                });

                for (var column = 0; column < sizeColumn; column++)
                {
                    if (column == corridorIndex1 || column == corridorIndex2)
                    {
                        continue;
                    }

                    var seatNumber = column + 1;
                    await unitOfWork.RepositoryInfrastructure.AddSeatAsync(new Seat
                    {
                        Id = seatId++,
                        RowId = rowId,
                        Number = seatNumber,
                        LayoutRow = row,
                        LayoutColumn = column
                    });
                }
            }

            Logger.Info("Persisted seats in cinema hall");
        }

        private static async Task ImportUser(IUnitOfWork unitOfWork)
        {
            var users = (await unitOfWork.RepositoryUser.GetUsersAsync()).ToList();
            if (users.Any())
            {
                Logger.Info("Skipping user import - already exist...");
                return;
            }

            await unitOfWork.RepositoryUser.AddRoleAsync(new Role {Id = 1L, Label = "Admin", MaxReservations = 20});
            await unitOfWork.RepositoryUser.AddRoleAsync(new Role {Id = 2L, Label = "Standard", MaxReservations = 6});
            await unitOfWork.RepositoryUser.AddRoleAsync(new Role {Id = 3L, Label = "Premium", MaxReservations = 10});

            await unitOfWork.RepositoryUser.AddCityAsync(new City { Id = 1L, PostalCode = "4234", Name = "Hagenberg" });
            await unitOfWork.RepositoryUser.AddCityAsync(new City { Id = 2L, PostalCode = "4020", Name = "Linz" });

            await unitOfWork.RepositoryUser.AddAddressAsync(new Address
                {Id = 1L, CityId = 1L, Street = "Campus", Number = 1});
            await unitOfWork.RepositoryUser.AddAddressAsync(new Address
                {Id = 2L, CityId = 1L, Street = "Campus", Number = 2});
            await unitOfWork.RepositoryUser.AddAddressAsync(new Address
                {Id = 3L, CityId = 2L, Street = "Hauptplatz", Number = 115});
            await unitOfWork.RepositoryUser.AddAddressAsync(new Address
                {Id = 4L, CityId = 2L, Street = "Wiener Strasse", Number = 120});

            await unitOfWork.RepositoryUser.AddUserAsync(new User
            {
                Id = 1L, Uuid = "3941a6ae-cc82-4b60-8f95-9805db11e393", RoleId = 1L, AddressId = 1L,
                FirstName = "Max", LastName = "Mustermann", Email = "max.mustermann@apollo.com", Phone = "066412364278"
            });
            await unitOfWork.RepositoryUser.AddUserAsync(new User
            {
                Id = 2L, Uuid = "c95e0ea8-cc33-44b1-a8fb-3a95bb85f536", RoleId = 2L, AddressId = 2L,
                FirstName = "Franz", LastName = "Mair", Email = "franz.mair@apollo.com", Phone = "06649874583"
            });
            await unitOfWork.RepositoryUser.AddUserAsync(new User
            {
                Id = 3L, Uuid = "cd692bf7-dbb4-4d52-918c-181536d6caf8", RoleId = 2L, AddressId = 3L,
                FirstName = "Susi", LastName = "Lustig", Email = "susi.lustig@apollo.com", Phone = "06603645782"
            });
            await unitOfWork.RepositoryUser.AddUserAsync(new User
            {
                Id = 4L, Uuid = "561ec65a-5e0b-46b4-af01-e3c42acfe686", RoleId = 3L, AddressId = 4L,
                FirstName = "Lisa", LastName = "Huber", Email = "lisa.huber@apollo.com", Phone = "06692536784"
            });

            await unitOfWork.RepositoryUser.AddUserAsync(new User
            {
                Id = 5L,
                Uuid = "7398a331-f9d3-482b-9f88-9abb39e8bd44",
                RoleId = 2L,
                AddressId = 1L,
                FirstName = "Terminal",
                LastName = "1",
                Email = "apollo@office.com",
                Phone = "0664/1337421337"
            });

            Logger.Info($"Persisted users");
        }
    }
}