using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.FluentEntity;
using Apollo.Persistence.FluentEntity.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.Util;
using Apollo.Util;
using Microsoft.Extensions.Configuration;

namespace Apollo.Api.Test
{
    public class DataSeeder : IDataSeeder
    {
        private const string ApolloTestDatabaseKey = "APOLLO_TEST";

        public IList<Seat> Seats { get; private set; }
        public IList<Row> Rows { get; private set; }
        public IList<Reservation> Reservations { get; private set; }
        public IList<CinemaHall> CinemaHalls { get; private set; }
        public IList<MovieActor> MovieActors { get; private set; }
        public IList<Actor> Actors { get; private set; }
        public IList<Movie> Movies { get; private set; }
        public IList<Genre> Genres { get; private set; }
        public IList<User> Users { get; private set; }
        public IList<Address> Addresses { get; private set; }
        public IList<City> Cities { get; private set; }
        public IList<Role> Roles { get; private set; }
        public IList<SeatReservation> SeatReservations { get; private set; }
        public IList<Schedule> Schedules { get; private set; }
        public IList<RowCategory> RowCategories { get; private set; }

        public IFluentEntityFrom FluentEntity { get; }

        public IEntityManager EntityManager { get; }

        public DataSeeder()
        {
            var appSettings = ConfigurationHelper.GetValues(ApolloTestDatabaseKey);
            var connectionFactory = new ConnectionFactory(appSettings[0]);
            EntityManager = EntityManagerFactory.CreateEntityManager(connectionFactory);
            FluentEntity = EntityManager.FluentEntity();
            InitSeedData();
        }

        private void InitSeedData()
        {
            CinemaHalls = CreateCinemaHalls();
            InitLayout(); // seat and row creation
            Reservations = CreateReservations();
            MovieActors = CreateMovieActors();
            Actors = CreateActors();
            Movies = CreateMovies();
            Genres = CreateGenres();
            Users = CreateUsers();
            Addresses = CreateAddresses();
            Cities = CreateCities();
            Roles = CreateRoles();
            SeatReservations = CreateSeatReservations();
            Schedules = CreateSchedules();
            RowCategories = CreateRowCategories();
        }

        public async Task SeedDatabaseAsync()
        {
            await Delete();

            await FluentEntity.InsertInto(Roles).ExecuteAsync();
            await FluentEntity.InsertInto(Cities).ExecuteAsync();
            await FluentEntity.InsertInto(Addresses).ExecuteAsync();
            await FluentEntity.InsertInto(Users).ExecuteAsync();

            await FluentEntity.InsertInto(Genres).ExecuteAsync();
            await FluentEntity.InsertInto(Movies).ExecuteAsync();
            await FluentEntity.InsertInto(Actors).ExecuteAsync();
            await FluentEntity.InsertInto(MovieActors).ExecuteAsync();

            await FluentEntity.InsertInto(CinemaHalls).ExecuteAsync();
            await FluentEntity.InsertInto(RowCategories).ExecuteAsync();
            await FluentEntity.InsertInto(Rows).ExecuteAsync();
            await FluentEntity.InsertInto(Seats).ExecuteAsync();

            await FluentEntity.InsertInto(Schedules).ExecuteAsync();
            await FluentEntity.InsertInto(Reservations).ExecuteAsync();
            await FluentEntity.InsertInto(SeatReservations).ExecuteAsync();
        }

        public async Task ResetDatabaseAsync()
        {
            await Delete();
        }

        private async Task Delete()
        {
            await FluentEntity.Delete<SeatReservation>().ExecuteAsync();
            await FluentEntity.Delete<Reservation>().ExecuteAsync();
            await FluentEntity.Delete<Ticket>().ExecuteAsync();
            await FluentEntity.Delete<Schedule>().ExecuteAsync();

            await FluentEntity.Delete<Seat>().ExecuteAsync();
            await FluentEntity.Delete<Row>().ExecuteAsync();
            await FluentEntity.Delete<RowCategory>().ExecuteAsync();
            await FluentEntity.Delete<CinemaHall>().ExecuteAsync();

            await FluentEntity.Delete<MovieActor>().ExecuteAsync();
            await FluentEntity.Delete<Actor>().ExecuteAsync();
            await FluentEntity.Delete<Movie>().ExecuteAsync();
            await FluentEntity.Delete<Genre>().ExecuteAsync();

            await FluentEntity.Delete<User>().ExecuteAsync();
            await FluentEntity.Delete<Address>().ExecuteAsync();
            await FluentEntity.Delete<City>().ExecuteAsync();
            await FluentEntity.Delete<Role>().ExecuteAsync();
        }


        private static IList<SeatReservation> CreateSeatReservations()
        {
            return new List<SeatReservation>
            {
                new SeatReservation {Id = 1L, ReservationId = 1L, SeatId = 245L},
                new SeatReservation {Id = 2L, ReservationId = 1L, SeatId = 246L},
                new SeatReservation {Id = 3L, ReservationId = 2L, SeatId = 1L},
                new SeatReservation {Id = 4L, ReservationId = 2L, SeatId = 2L},
                new SeatReservation {Id = 5L, ReservationId = 2L, SeatId = 3L},
                new SeatReservation {Id = 6L, ReservationId = 2L, SeatId = 4L},
                new SeatReservation {Id = 7L, ReservationId = 3L, SeatId = 1L},
                new SeatReservation {Id = 8L, ReservationId = 3L, SeatId = 2L},
                new SeatReservation {Id = 9L, ReservationId = 4L, SeatId = 1L},
                new SeatReservation {Id = 10L, ReservationId = 4L, SeatId = 2L},
                new SeatReservation {Id = 11L, ReservationId = 5L, SeatId = 87},
                new SeatReservation {Id = 12L, ReservationId = 6L, SeatId = 86},
                new SeatReservation {Id = 13L, ReservationId = 7L, SeatId = 253},
                new SeatReservation {Id = 14L, ReservationId = 7L, SeatId = 254},
            };
        }

        private static IList<Reservation> CreateReservations()
        {
            return new List<Reservation>
            {
                new Reservation {Id = 1L, ScheduleId = 1L, UserId = 5L, Deleted = false},
                new Reservation {Id = 2L, ScheduleId = 3L, UserId = 5L, Deleted = false},
                new Reservation {Id = 3L, ScheduleId = 23L, UserId = 5L, Deleted = false},
                new Reservation {Id = 4L, ScheduleId = 14L, UserId = 5L, Deleted = false},
                new Reservation {Id = 5L, ScheduleId = 13L, UserId = 5L, Deleted = false},
                new Reservation {Id = 6L, ScheduleId = 17L, UserId = 5L, Deleted = false},
                new Reservation {Id = 7L, ScheduleId = 19L, UserId = 5L, Deleted = true},
            };
        }


        private static IList<Genre> CreateGenres()
        {
            return new List<Genre>
            {
                new Genre {Id = 1L, Name = "Drama"},
                new Genre {Id = 2L, Name = "Crime"},
                new Genre {Id = 3L, Name = "Action"},
                new Genre {Id = 4L, Name = "Thriller"},
                new Genre {Id = 5L, Name = "Biography"},
                new Genre {Id = 6L, Name = "History"},
                new Genre {Id = 7L, Name = "Adventure"},
                new Genre {Id = 8L, Name = "Fantasy"},
                new Genre {Id = 9L, Name = "Western"},
                new Genre {Id = 10L, Name = "Romance"},
                new Genre {Id = 11L, Name = "Sci-Fi"},
                new Genre {Id = 12L, Name = "Mystery"},
                new Genre {Id = 13L, Name = "Comedy"},
                new Genre {Id = 14L, Name = "War"},
                new Genre {Id = 15L, Name = "Family"},
                new Genre {Id = 16L, Name = "Animation"},
                new Genre {Id = 17L, Name = "Musical"},
                new Genre {Id = 18L, Name = "Music"},
                new Genre {Id = 19L, Name = "Horror"},
                new Genre {Id = 20L, Name = "Film-Noir"},
                new Genre {Id = 21L, Name = "Sport"},
                new Genre {Id = 22L, Name = "Unused"}
            };
        }

        private static IList<Actor> CreateActors()
        {
            return new List<Actor>
            {
                new Actor {Id = 1L, FirstName = "Tim", LastName = "Robbins"},
                new Actor {Id = 2L, FirstName = "Morgan", LastName = "Freeman"},
                new Actor {Id = 3L, FirstName = "Bob", LastName = "Gunton"},
                new Actor {Id = 4L, FirstName = "William", LastName = "Sadler"},
                new Actor {Id = 5L, FirstName = "Clancy", LastName = "Brown"},
                new Actor {Id = 6L, FirstName = "Gil", LastName = "Bellows"},
                new Actor {Id = 7L, FirstName = "Mark", LastName = "Rolston"},
                new Actor {Id = 8L, FirstName = "James", LastName = "Whitmore"},
                new Actor {Id = 9L, FirstName = "Jeffrey", LastName = "DeMunn"},
                new Actor {Id = 10L, FirstName = "Larry", LastName = "Brandenburg"},
                new Actor {Id = 11L, FirstName = "Neil", LastName = "Giuntoli"},
                new Actor {Id = 12L, FirstName = "Brian", LastName = "Libby"},
                new Actor {Id = 13L, FirstName = "David", LastName = "Proval"},
                new Actor {Id = 14L, FirstName = "Joseph", LastName = "Ragno"},
                new Actor {Id = 15L, FirstName = "Jude", LastName = "Ciccolella"},
                new Actor {Id = 16L, FirstName = "Paul", LastName = "McCrane"},
                new Actor {Id = 17L, FirstName = "Renee", LastName = "Blaine"},
                new Actor {Id = 18L, FirstName = "Scott", LastName = "Mann"},
                new Actor {Id = 19L, FirstName = "John", LastName = "Horton"},
                new Actor {Id = 20L, FirstName = "Gordon", LastName = "Greene"},
                new Actor {Id = 21L, FirstName = "Alfonso", LastName = "Freeman"},
                new Actor {Id = 22L, FirstName = "V.J.", LastName = "Foster"},
                new Actor {Id = 23L, FirstName = "John E.", LastName = "Summers"},
                new Actor {Id = 24L, FirstName = "Frank", LastName = "Medrano"},
                new Actor {Id = 25L, FirstName = "Mack", LastName = "Miles"},
                new Actor {Id = 26L, FirstName = "Alan R.", LastName = "Kessler"},
                new Actor {Id = 27L, FirstName = "Morgan", LastName = "Lund"},
                new Actor {Id = 28L, FirstName = "Cornell", LastName = "Wallace"},
                new Actor {Id = 29L, FirstName = "Gary Lee", LastName = "Davis"},
                new Actor {Id = 30L, FirstName = "Neil", LastName = "Summers"},
                new Actor {Id = 31L, FirstName = "Unknown", LastName = "Author"}
            };
        }

        private static IList<MovieActor> CreateMovieActors()
        {
            return new List<MovieActor>
            {
                new MovieActor {Id = 1L, ActorId = 1L, MovieId = 1L},
                new MovieActor {Id = 2L, ActorId = 2L, MovieId = 1L},
                new MovieActor {Id = 3L, ActorId = 3L, MovieId = 1L},
                new MovieActor {Id = 4L, ActorId = 1L, MovieId = 2L},
                new MovieActor {Id = 5L, ActorId = 4L, MovieId = 2L},
                new MovieActor {Id = 6L, ActorId = 5L, MovieId = 2L},
                new MovieActor {Id = 7L, ActorId = 6L, MovieId = 2L},
                new MovieActor {Id = 8L, ActorId = 7L, MovieId = 3L},
                new MovieActor {Id = 9L, ActorId = 8L, MovieId = 3L},
                new MovieActor {Id = 10L, ActorId = 9L, MovieId = 3L},
                new MovieActor {Id = 11L, ActorId = 10L, MovieId = 3L},
                new MovieActor {Id = 12L, ActorId = 1L, MovieId = 3L},
                new MovieActor {Id = 13L, ActorId = 9L, MovieId = 3L},
                new MovieActor {Id = 14L, ActorId = 11L, MovieId = 3L},
                new MovieActor {Id = 15L, ActorId = 30L, MovieId = 4L},
                new MovieActor {Id = 16L, ActorId = 20L, MovieId = 4L},
                new MovieActor {Id = 17L, ActorId = 29L, MovieId = 4L},
                new MovieActor {Id = 18L, ActorId = 28L, MovieId = 4L},
                new MovieActor {Id = 19L, ActorId = 27L, MovieId = 4L},
                new MovieActor {Id = 20L, ActorId = 25L, MovieId = 5L},
                new MovieActor {Id = 21L, ActorId = 24L, MovieId = 5L},
                new MovieActor {Id = 22L, ActorId = 23L, MovieId = 5L},
                new MovieActor {Id = 23L, ActorId = 22L, MovieId = 5L},
                new MovieActor {Id = 24L, ActorId = 21L, MovieId = 5L},
                new MovieActor {Id = 25L, ActorId = 20L, MovieId = 5L},
                new MovieActor {Id = 26L, ActorId = 19L, MovieId = 6L},
                new MovieActor {Id = 27L, ActorId = 18L, MovieId = 6L},
                new MovieActor {Id = 28L, ActorId = 17L, MovieId = 6L},
                new MovieActor {Id = 29L, ActorId = 16L, MovieId = 6L},
                new MovieActor {Id = 30L, ActorId = 15L, MovieId = 6L},
                new MovieActor {Id = 31L, ActorId = 14L, MovieId = 6L},
                new MovieActor {Id = 32L, ActorId = 1L, MovieId = 6L},
                new MovieActor {Id = 33L, ActorId = 10L, MovieId = 6L},
                new MovieActor {Id = 34L, ActorId = 9L, MovieId = 6L},
                new MovieActor {Id = 35L, ActorId = 13L, MovieId = 7L},
                new MovieActor {Id = 36L, ActorId = 12L, MovieId = 7L},
                new MovieActor {Id = 37L, ActorId = 11L, MovieId = 8L},
                new MovieActor {Id = 38L, ActorId = 10L, MovieId = 8L},
                new MovieActor {Id = 39L, ActorId = 30L, MovieId = 9L},
                new MovieActor {Id = 40L, ActorId = 29L, MovieId = 9L},
                new MovieActor {Id = 41L, ActorId = 17L, MovieId = 9L},
                new MovieActor {Id = 42L, ActorId = 13L, MovieId = 10L},
                new MovieActor {Id = 43L, ActorId = 2L, MovieId = 10L},
            };
        }

        private static IList<Movie> CreateMovies()
        {
            return new List<Movie>
            {
                new Movie
                {
                    Id = 1L, Rating = 3, Title = "The Kid",
                    Description =
                        @"The opening title reads: A comedy with a smile--and perhaps a tear. As she leaves the charity hospital and passes a church wedding, Edna deposits her new baby with a pleading note in a limousine and goes off to commit suicide. The limo is stolen by thieves who dump the baby by a garbage can. Charlie the Tramp finds the baby and makes a home for him. Five years later Edna has become an opera star but does charity work for slum youngsters in hope of finding her boy. A doctor called by Edna discovers the note with the truth about the Kid and reports it to the authorities who come to take him away from Charlie. Before he arrives at the Orphan Asylum Charlie steals him back and takes him to a flophouse. The proprietor reads of a reward for the Kid and takes him to Edna. Charlie is later awakened by a kind policeman who reunites him with the Kid at Edna's mansion.",
                    Duration = 125, GenreId = 1L, Image = new byte[] {12, 5, 1, 2, 0}
                },
                new Movie
                {
                    Id = 2L, Rating = 5, Title = "Sherlock Jr.",
                    Description =
                        "A meek and mild projectionist, who also cleans up after screenings, would like nothing better than to be a private detective. He becomes engaged to a pretty girl but a ladies man known as the Sheik vies for her affection. He gets rid of the projectionist by stealing a pocket watch belonging to the girl's father - which he pawns to buy her an expensive box of candy. He then slips the pawn ticket into the projectionist's pocket and subsequently is found by the police. He doesn't have much luck but in his dreams, he the debonair and renowned detective Sherlock Jr. who faces danger and solves the crime. In real life, the girl solves crimes quickly.",
                    Duration = 45, GenreId = 3L, Image = new byte[] {12, 5, 1, 2, 0},
                    Trailer = "https://www.imdb.com/video/vi2465709081"
                },
                new Movie
                {
                    Id = 3L, Rating = 2, Title = "The Gold Rush",
                    Description =
                        "A lone prospector ventures into Alaska looking for gold. He gets mixed up with some burly characters and falls in love with the beautiful Georgia. He tries to win her heart with his singular charm.",
                    Duration = 95, GenreId = 7L, Image = new byte[] {12, 5, 1, 2, 0},
                    Trailer = "https://www.imdb.com/video/vi70820889"
                },
                new Movie
                {
                    Id = 4L, Rating = 1, Title = "Metropolis",
                    Description =
                        "Sometime in the future, the city of Metropolis is home to a Utopian society where its wealthy residents live a carefree life. One of those is Freder Fredersen. One day, he spots a beautiful woman with a group of children, she and the children quickly disappear. Trying to follow her, he is horrified to find an underground world of workers who apparently run the machinery that keeps the Utopian world above ground functioning. One of the few people above ground who knows about the world below is Freder's father, John Fredersen, who is the founder and master of Metropolis. Freder learns that the woman is called Maria, who espouses the need to join the \"hands\" - the workers - to the \"head\" - those in power above - by a mediator who will act as the \"heart\". Freder wants to help the plight of the workers in their struggle for a better life. But when John learns of what Maria is advocating and that Freder has joined their cause, with the assistance of an old colleague. an inventor called ...",
                    Duration = 153, GenreId = 1L, Image = new byte[] {12, 5, 1, 2, 0},
                    Trailer = "https://www.imdb.com/video/vi1050609177"
                },
                new Movie
                {
                    Id = 5L, Rating = 1, Title = "The General",
                    Description =
                        "Johnnie loves his train (\"The General\") and Annabelle Lee. When the Civil War begins he is turned down for service because he's more valuable as an engineer. Annabelle thinks it's because he's a coward. Union spies capture The General with Annabelle on board. Johnnie must rescue both his loves.",
                    Duration = 67, GenreId = 3L, Image = new byte[] {12, 5, 1, 2, 0},
                    Trailer = "https://www.imdb.com/video/vi534623769"
                },
                new Movie
                {
                    Id = 6L, Rating = 2, Title = "The Circus",
                    Description =
                        "The Tramp finds himself at a circus where he is promptly chased around by the police who think he is a pickpocket. Running into the Bigtop, he is an accidental sensation with his hilarious efforts to elude the police. The circus owner immediately hires him, but discovers that the Tramp cannot be funny on purpose, so he takes advantage of the situation by making the Tramp a janitor who just happens to always be in the Bigtop at showtime. Unaware of this exploitation, the Tramp falls for the owner's lovely acrobatic stepdaughter, who is abused by her father. His chances seem good, until a dashing rival comes in and Charlie feels he has to compete with him.",
                    Duration = 72, GenreId = 13L, Image = new byte[] {12, 5, 1, 2, 0}
                },
                new Movie
                {
                    Id = 7L, Rating = 4, Title = "The Passion of Joan of Arc",
                    Description =
                        "Giovanna is taken to the Inquisition court. . After the accusation of blasphemy continues to pray in ecstasy . A friar thinks that Giovanna is a saint, but is taken away by the soldiers. Giovanna sees a cross in the shadow and feels comforted. She is not considered a daughter of God but a daughter of the devil and is sentenced to torture. Giovanna D 'Arco says that even if she dies she will not deny anything. The eyes are twisted by terror in front of the torture wheel and faint. Giovanna is taken to a bed where they are bleeding. Giovanna feels that she is about to die and asks to be buried in a consecrated area. Giovanna burns at the stake while devoted ladies cry.",
                    Duration = 114, GenreId = 5L, Image = new byte[] {12, 5, 1, 2, 0}
                },
                new Movie
                {
                    Id = 8L, Rating = 4, Title = "City Lights",
                    Description =
                        "A tramp falls in love with a beautiful blind girl. Her family is in financial trouble. The tramp's on-and-off friendship with a wealthy man allows him to be the girl's benefactor and suitor.",
                    Duration = 87, GenreId = 13L, Image = new byte[] {12, 5, 1, 2, 0}
                },
                new Movie
                {
                    Id = 9L, Rating = 1, Title = "M",
                    Description =
                        "In Germany, Hans Beckert is an unknown killer of girls. He whistles Edvard Grieg's 'In The Hall of the Mountain King', from the 'Peer Gynt' Suite I Op. 46 while attracting the little girls for death. The police force pressed by the Minister give its best effort trying unsuccessfully to arrest the serial killer. The organized crime has great losses due to the intense search and siege of the police and decides to chase the murderer, with the support of the beggars association. They catch Hans and briefly judge him.",
                    Duration = 99, GenreId = 2L, Image = new byte[] {12, 5, 1, 2, 0}
                },
                new Movie
                {
                    Id = 10L, Rating = 2, Title = "It Happened One Night",
                    Description =
                        "Ellie Andrews has just tied the knot with society aviator King Westley when she is whisked away to her father's yacht and out of King's clutches. Ellie jumps ship and eventually winds up on a bus headed back to her husband. Reluctantly she must accept the help of out-of- work reporter Peter Warne. Actually, Warne doesn't give her any choice: either she sticks with him until he gets her back to her husband, or he'll blow the whistle on Ellie to her father. Either way, Peter gets what (he thinks!) he wants .... a really juicy newspaper story.",
                    Duration = 105, GenreId = 13L, Image = new byte[] {12, 5, 1, 2, 0},
                    Trailer = "https://www.imdb.com/video/vi3410886681"
                },
                new Movie
                {
                    Id = 11L, Rating = 5, Title = "Unused",
                    Description =
                        "Ellie Andrews has just tied the knot with society aviator King Westley when she is whisked away to her father's yacht and out of King's clutches. Ellie jumps ship and eventually winds up on a bus headed back to her husband. Reluctantly she must accept the help of out-of- work reporter Peter Warne. Actually, Warne doesn't give her any choice: either she sticks with him until he gets her back to her husband, or he'll blow the whistle on Ellie to her father. Either way, Peter gets what (he thinks!) he wants .... a really juicy newspaper story.",
                    Duration = 105, GenreId = 13L, Image = new byte[] {12, 5, 1, 2, 0},
                    Trailer = "https://www.imdb.com/video/vi3410886681"
                },
            };
        }

        private void InitLayout()
        {
            Rows = new List<Row>();
            Seats = new List<Seat>();
            var rowCategories = CreateRowCategories();
            var categoryIds = rowCategories
                .Where(c => !c.Name.ToLower().StartsWith("unused"))
                .Select(c => c.Id)
                .ToList();

            var cinemaHall = CinemaHalls.Last();
            Rows.Add(new Row
                {Id = 1L, CategoryId = rowCategories.First().Id, CinemaHallId = cinemaHall.Id, Number = 10});
            Rows.Add(new Row
                {Id = 2L, CategoryId = rowCategories.First().Id, CinemaHallId = cinemaHall.Id, Number = 11});
            Seats.Add(new Seat {Id = 1L, Number = 1, LayoutColumn = 0, LayoutRow = 0, RowId = 1L});
            Seats.Add(new Seat {Id = 2L, Number = 2, LayoutColumn = 0, LayoutRow = 1, RowId = 1L});
            Seats.Add(new Seat {Id = 3L, Number = 3, LayoutColumn = 0, LayoutRow = 2, RowId = 1L});
            Seats.Add(new Seat
                {Id = 4L, Number = 1, LayoutColumn = cinemaHall.SizeColumn - 1, LayoutRow = 0, RowId = 2L});
            Seats.Add(new Seat
                {Id = 5L, Number = 2, LayoutColumn = cinemaHall.SizeColumn - 1, LayoutRow = 1, RowId = 2L});
            Seats.Add(new Seat
                {Id = 6L, Number = 3, LayoutColumn = cinemaHall.SizeColumn - 1, LayoutRow = 2, RowId = 2L});
            Rows.Add(new Row
                {Id = 3L, CategoryId = rowCategories.First().Id, CinemaHallId = CinemaHalls.Last().Id, Number = 12});

            cinemaHall = CinemaHalls.First();
            const int sizeRow = 20;
            const int sizeColumn = 30;

            var corridorIndex1 = (int) (sizeColumn * .25);
            var corridorIndex2 = (int) (sizeColumn * .75);
            var rowFactor = (double) sizeRow / categoryIds.Count;

            var rowIdOffset = Rows.Count;
            var seatId = Seats.Count + 1;
            for (var row = 1; row < sizeRow; row += 2)
            {
                var rowNumber = row / 2 + 1;
                Rows.Add(new Row
                {
                    Id = rowNumber + rowIdOffset,
                    CinemaHallId = cinemaHall.Id,
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
                    Seats.Add(new Seat
                    {
                        Id = seatId++,
                        RowId = rowNumber + rowIdOffset,
                        Number = seatNumber,
                        LayoutRow = row,
                        LayoutColumn = column
                    });
                }
            }
        }

        private static IList<RowCategory> CreateRowCategories()
        {
            return new List<RowCategory>
            {
                new RowCategory {Id = 1L, Name = "Standard", PriceFactor = 1.0},
                new RowCategory {Id = 2L, Name = "Premium", PriceFactor = 1.25},
                new RowCategory {Id = 3L, Name = "Deluxe", PriceFactor = 1.5},
                new RowCategory {Id = 4L, Name = "Unused 1", PriceFactor = 1.75},
                new RowCategory {Id = 5L, Name = "Unused 2", PriceFactor = 1.75}
            };
        }

        private static IList<CinemaHall> CreateCinemaHalls()
        {
            return new List<CinemaHall>
            {
                new CinemaHall {Id = 1L, Label = "Hall of Stars", SizeRow = 20, SizeColumn = 10},
                new CinemaHall {Id = 2L, Label = "Hall of Dust", SizeRow = 20, SizeColumn = 10},
                new CinemaHall {Id = 3L, Label = "New Hall", SizeRow = 5, SizeColumn = 5},
                new CinemaHall {Id = 4L, Label = "Empty Hall 1", SizeRow = 5, SizeColumn = 5},
                new CinemaHall {Id = 5L, Label = "Empty Hall 2", SizeRow = 5, SizeColumn = 5}
            };
        }

        private static IList<Role> CreateRoles()
        {
            return new List<Role>
            {
                new Role {Id = 1L, Label = "Admin", MaxReservations = 20},
                new Role {Id = 2L, Label = "Standard", MaxReservations = 6},
                new Role {Id = 3L, Label = "Premium", MaxReservations = 10},
            };
        }

        private static IList<City> CreateCities()
        {
            return new List<City>
            {
                new City {Id = 1, PostalCode = "4234", Name = "Hagenberg"},
                new City {Id = 2, PostalCode = "4020", Name = "Linz"},
            };
        }

        private static IList<Address> CreateAddresses()
        {
            return new List<Address>
            {
                new Address {Id = 1, CityId = 1L, Street = "Campus", Number = 1},
                new Address {Id = 2, CityId = 1L, Street = "Campus", Number = 2},
                new Address {Id = 3, CityId = 2L, Street = "Hauptplatz", Number = 115},
                new Address {Id = 4, CityId = 2L, Street = "Wiener Strasse", Number = 120},
            };
        }

        private static IList<User> CreateUsers()
        {
            return new List<User>
            {
                new User
                {
                    Id = 1L, Uuid = "3941a6ae-cc82-4b60-8f95-9805db11e393", AddressId = 1L,
                    Email = "max.mustermann@apollo.com", FirstName = "Max", LastName = "Mustermann",
                    Phone = "066412364278", RoleId = 1L
                },
                new User
                {
                    Id = 2L, Uuid = "c95e0ea8-cc33-44b1-a8fb-3a95bb85f536", AddressId = 2L,
                    Email = "franz.mair@apollo.com", FirstName = "Franz", LastName = "Mair", Phone = "06649874583",
                    RoleId = 2L
                },
                new User
                {
                    Id = 3L, Uuid = "cd692bf7-dbb4-4d52-918c-181536d6caf8", AddressId = 3L,
                    Email = "susi.lustig@apollo.com", FirstName = "Susi", LastName = "Lustig", Phone = "06603645782",
                    RoleId = 2L
                },
                new User
                {
                    Id = 4L, Uuid = "561ec65a-5e0b-46b4-af01-e3c42acfe686", AddressId = 4L,
                    Email = "lisa.huber@apollo.com", FirstName = "Lisa", LastName = "Huber", Phone = "06692536784",
                    RoleId = 3L
                },
                new User
                {
                    Id = 5L, Uuid = "7398a331-f9d3-482b-9f88-9abb39e8bd44", AddressId = 1L, Email = "apollo@office.com",
                    FirstName = "Terminal", LastName = "1", Phone = "0664/1337421337", RoleId = 2L
                },
            };
        }

        private static IList<Schedule> CreateSchedules()
        {
            var utcNow = DateTime.UtcNow;
            var startTime = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 8, 0, 0);
            return new List<Schedule>
            {
                new Schedule {Id = 1L, StartTime = startTime, MovieId = 1L, CinemaHallId = 1L, Price = 11.22M},
                new Schedule
                    {Id = 2L, StartTime = startTime.AddHours(2.5), MovieId = 6L, CinemaHallId = 1L, Price = 8.13M},
                new Schedule
                    {Id = 3L, StartTime = startTime.AddHours(2.5), MovieId = 3L, CinemaHallId = 1L, Price = 10.07M},
                new Schedule
                    {Id = 4L, StartTime = startTime.AddHours(2.5), MovieId = 5L, CinemaHallId = 1L, Price = 9.43M},
                new Schedule
                    {Id = 5L, StartTime = startTime.AddHours(2.5), MovieId = 2L, CinemaHallId = 1L, Price = 12.38M},

                new Schedule
                {
                    Id = 6L, StartTime = startTime.AddDays(1).AddHours(2.5), MovieId = 4L, CinemaHallId = 1L,
                    Price = 12.40M
                },
                new Schedule
                {
                    Id = 7L, StartTime = startTime.AddDays(1).AddHours(2.5), MovieId = 2L, CinemaHallId = 1L,
                    Price = 12.47M
                },
                new Schedule
                {
                    Id = 8L, StartTime = startTime.AddDays(1).AddHours(2.5), MovieId = 1L, CinemaHallId = 1L,
                    Price = 8.73M
                },
                new Schedule
                {
                    Id = 9L, StartTime = startTime.AddDays(1).AddHours(2.5), MovieId = 2L, CinemaHallId = 1L,
                    Price = 7.18M
                },
                new Schedule
                {
                    Id = 10L, StartTime = startTime.AddDays(1).AddHours(2.5), MovieId = 8L, CinemaHallId = 1L,
                    Price = 11.99M
                },

                new Schedule
                {
                    Id = 11L, StartTime = startTime.AddDays(2).AddHours(2.5), MovieId = 9L, CinemaHallId = 2L,
                    Price = 12.27M
                },
                new Schedule
                {
                    Id = 12L, StartTime = startTime.AddDays(2).AddHours(2.5), MovieId = 10L, CinemaHallId = 2L,
                    Price = 9.78M
                },
                new Schedule
                {
                    Id = 13L, StartTime = startTime.AddDays(2).AddHours(2.5), MovieId = 10L, CinemaHallId = 2L,
                    Price = 7.65M
                },
                new Schedule
                {
                    Id = 14L, StartTime = startTime.AddDays(2).AddHours(2.5), MovieId = 3L, CinemaHallId = 2L,
                    Price = 14.27M
                },
                new Schedule
                {
                    Id = 15L, StartTime = startTime.AddDays(2).AddHours(2.5), MovieId = 3L, CinemaHallId = 2L,
                    Price = 12.24M
                },

                new Schedule
                {
                    Id = 16L, StartTime = startTime.AddDays(3).AddHours(2.5), MovieId = 2L, CinemaHallId = 1L,
                    Price = 10.38M
                },
                new Schedule
                {
                    Id = 17L, StartTime = startTime.AddDays(3).AddHours(2.5), MovieId = 4L, CinemaHallId = 1L,
                    Price = 11.96M
                },
                new Schedule
                {
                    Id = 18L, StartTime = startTime.AddDays(3).AddHours(2.5), MovieId = 5L, CinemaHallId = 1L,
                    Price = 12.31M
                },
                new Schedule
                {
                    Id = 19L, StartTime = startTime.AddDays(3).AddHours(2.5), MovieId = 7L, CinemaHallId = 1L,
                    Price = 14.53M
                },
                new Schedule
                {
                    Id = 20L, StartTime = startTime.AddDays(3).AddHours(2.5), MovieId = 8L, CinemaHallId = 1L,
                    Price = 13.22M
                },

                new Schedule
                {
                    Id = 21L, StartTime = startTime.AddDays(4).AddHours(2.5), MovieId = 1L, CinemaHallId = 1L,
                    Price = 9.50M
                },
                new Schedule
                {
                    Id = 22L, StartTime = startTime.AddDays(4).AddHours(2.5), MovieId = 3L, CinemaHallId = 1L,
                    Price = 7.35M
                },
                new Schedule
                {
                    Id = 23L, StartTime = startTime.AddDays(4).AddHours(2.5), MovieId = 3L, CinemaHallId = 1L,
                    Price = 10.01M
                },
                new Schedule
                {
                    Id = 24L, StartTime = startTime.AddDays(4).AddHours(2.5), MovieId = 7L, CinemaHallId = 1L,
                    Price = 12.5M
                },
                new Schedule
                {
                    Id = 25L, StartTime = startTime.AddDays(4).AddHours(2.5), MovieId = 4L, CinemaHallId = 1L,
                    Price = 9.99M
                },
                new Schedule
                {
                    Id = 26L, StartTime = startTime.AddDays(5).AddHours(2.5), MovieId = 4L, CinemaHallId = 2L, 
                    Price = 13.99M, 
                    Deleted = true
                },
            };
        }
    }
}