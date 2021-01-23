using System;
using Apollo.Domain.Entity;
using Apollo.Util.Logger;
using static Apollo.Util.ValidationHelper;

namespace Apollo.Core.Dto
{
    public class Mapper
    {
        private static readonly IApolloLogger<Mapper> Logger = LoggerFactory.CreateLogger<Mapper>();

        public static ActorDto Map(Actor actor)
        {
            ValidateNull( Logger.Here(), actor);
            return new ActorDto
            {
                Id = actor.Id,
                RowVersion = actor.RowVersion,
                LastName = actor.LastName,
                FirstName = actor.FirstName
            };
        }

        public static Actor Map(ActorDto movieActor)
        {
            ValidateNull( Logger.Here(), movieActor);
            return new Actor
            {
                Id = movieActor.Id,
                RowVersion = movieActor.RowVersion,
                FirstName = movieActor.FirstName,
                LastName = movieActor.LastName
            };
        }


        public static GenreDto Map(Genre genre)
        {
            ValidateNull( Logger.Here(), genre);
            return new GenreDto
            {
                Id = genre.Id,
                RowVersion = genre.RowVersion,
                Name = genre.Name
            };
        }

        public static Genre Map(GenreDto genreDto)
        {
            ValidateNull( Logger.Here(), genreDto);
            return new Genre
            {
                Id = genreDto.Id,
                RowVersion = genreDto.RowVersion,
                Name = genreDto.Name
            };
        }

        public static MovieActor Map(MovieActorDto movieActorDto)
        {
            ValidateNull( Logger.Here(), movieActorDto);
            return new MovieActor
            {
                Id = movieActorDto.Id,
                RowVersion = movieActorDto.RowVersion,
                Actor = movieActorDto.Actor == null ? null : Map(movieActorDto.Actor),
                Movie = movieActorDto.Movie == null ? null : Map(movieActorDto.Movie),
                ActorId = movieActorDto.ActorId,
                MovieId = movieActorDto.MovieId
            };
        }

        public static MovieDto Map(Movie movie)
        {
            ValidateNull( Logger.Here(), movie);
            return new MovieDto
            {
                Id = movie.Id,
                RowVersion = movie.RowVersion,
                Title = movie.Title,
                Trailer = movie.Trailer,
                Description = movie.Description,
                Image = movie.Image,
                Duration = movie.Duration,
                Genre = movie.Genre == null ? null : Map(movie.Genre),
                GenreId = movie.GenreId,
                Rating = movie.Rating
            };
        }

        public static MovieActorDto Map(MovieActor movieActor)
        {
            ValidateNull( Logger.Here(), movieActor);
            return new MovieActorDto
            {
                Id = movieActor.Id,
                RowVersion = movieActor.RowVersion,
                Actor = movieActor.Actor == null ? null : Map(movieActor.Actor),
                Movie = movieActor.Movie == null ? null : Map(movieActor.Movie),
                MovieId = movieActor.MovieId,
                ActorId = movieActor.ActorId
            };
        }

        public static Movie Map(MovieDto movieDto)
        {
            ValidateNull( Logger.Here(), movieDto);
            return new Movie
            {
                Id = movieDto.Id,
                RowVersion = movieDto.RowVersion,
                Title = movieDto.Title,
                Trailer = movieDto.Trailer,
                Description = movieDto.Description,
                Image = movieDto.Image ?? Array.Empty<byte>(),
                Duration = movieDto.Duration,
                GenreId = movieDto.GenreId,
                Genre = movieDto.Genre == null ? null : Map(movieDto.Genre),
                Rating = movieDto.Rating
            };
        }

        public static ScheduleDto Map(Schedule schedule)
        {
            ValidateNull( Logger.Here(), schedule);
            return new ScheduleDto
            {
                Id = schedule.Id,
                RowVersion = schedule.RowVersion,
                StartTime = schedule.StartTime,
                CinemaHall = schedule.CinemaHall == null ? null : Map(schedule.CinemaHall),
                CinemaHallId = schedule.CinemaHallId,
                Movie = schedule.Movie == null ? null : Map(schedule.Movie),
                MovieId = schedule.MovieId,
                Price = schedule.Price
            };
        }

        public static Schedule Map(ScheduleDto scheduleDto)
        {
            ValidateNull( Logger.Here(), scheduleDto);
            return new Schedule
            {
                Id = scheduleDto.Id,
                RowVersion = scheduleDto.RowVersion,
                StartTime = scheduleDto.StartTime,
                CinemaHall = scheduleDto.CinemaHall == null ? null :  Map(scheduleDto.CinemaHall),
                CinemaHallId = scheduleDto.CinemaHallId,
                Movie = scheduleDto.Movie == null ? null :  Map(scheduleDto.Movie),
                MovieId = scheduleDto.MovieId,
                Price = scheduleDto.Price
            };
        }

        public static RowDto Map(Row row)
        {
            ValidateNull( Logger.Here(), row);
            return new RowDto
            {
                Id = row.Id,
                RowVersion = row.RowVersion,
                Category = row.Category == null ? null : Map(row.Category),
                CategoryId = row.CategoryId,
                CinemaHall = row.CinemaHall == null ? null : Map(row.CinemaHall),
                CinemaHallId = row.CinemaHallId,
                Number = row.Number
            };
        }

        public static Row Map(RowDto rowDto)
        {
            ValidateNull( Logger.Here(), rowDto);
            return new Row
            {
                Id = rowDto.Id,
                RowVersion = rowDto.RowVersion,
                Category = rowDto.Category == null ? null : Map(rowDto.Category),
                CategoryId = rowDto.CategoryId,
                CinemaHall = rowDto.CinemaHall == null ? null : Map(rowDto.CinemaHall),
                CinemaHallId = rowDto.CinemaHallId,
                Number = rowDto.Number
            };
        }

        public static RowCategoryDto Map(RowCategory rowCategory)
        {
            ValidateNull( Logger.Here(), rowCategory);
            return new RowCategoryDto
            {
                Id = rowCategory.Id,
                RowVersion = rowCategory.RowVersion,
                Name = rowCategory.Name,
                PriceFactor = rowCategory.PriceFactor
            };
        }

        public static RowCategory Map(RowCategoryDto rowCategoryDto)
        {
            ValidateNull( Logger.Here(), rowCategoryDto);
            return new RowCategory
            {
                Id = rowCategoryDto.Id,
                RowVersion = rowCategoryDto.RowVersion,
                Name = rowCategoryDto.Name,
                PriceFactor = rowCategoryDto.PriceFactor
            };
        }

        public static CinemaHall Map(CinemaHallDto cinemaHallDto)
        {
            ValidateNull( Logger.Here(), cinemaHallDto);
            return new CinemaHall
            {
                Id = cinemaHallDto.Id,
                RowVersion = cinemaHallDto.RowVersion,
                Label = cinemaHallDto.Label,
                SizeColumn = cinemaHallDto.SizeColumn,
                SizeRow =  cinemaHallDto.SizeRow
            };
        }

        public static CinemaHallDto Map(CinemaHall cinemaHall)
        {
            ValidateNull( Logger.Here(), cinemaHall);
            return new CinemaHallDto
            {
                Id = cinemaHall.Id,
                RowVersion = cinemaHall.RowVersion,
                Label = cinemaHall.Label,
                SizeRow = cinemaHall.SizeRow,
                SizeColumn = cinemaHall.SizeColumn
            };
        }

        public static Seat Map(SeatDto seatDto)
        {
            ValidateNull( Logger.Here(), seatDto);
            return new Seat
            {
                Id = seatDto.Id,
                RowVersion = seatDto.RowVersion,
                LayoutColumn = seatDto.LayoutColumn,
                LayoutRow = seatDto.LayoutRow,
                Locked = seatDto.State == SeatState.Locked,
                Number = seatDto.Number,
                Row = seatDto.Row == null ? null : Map(seatDto.Row),
                RowId = seatDto.RowId
            };
        }

        public static SeatDto Map(Seat seat)
        {
            ValidateNull( Logger.Here(), seat);
            return new SeatDto
            {
                Id = seat.Id,
                RowVersion = seat.RowVersion,
                LayoutColumn = seat.LayoutColumn,
                LayoutRow = seat.LayoutRow,
                State = seat.Locked ? SeatState.Locked : SeatState.Free, 
                Number = seat.Number,
                Row = seat.Row == null ? null : Map(seat.Row),
                RowId = seat.RowId
            };
        }

        public static UserDto Map(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return new UserDto
            {
                Id = user.Id,
                RowVersion = user.RowVersion,
                Uuid = user.Uuid,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address == null ? null : Map(user.Address),
                Role = user.Role == null ? null : Map(user.Role),
                RoleId = user.RoleId
            };
        }

        public static UserDto Map(User user, Address address, Role role)
        {
            var result = Map(user);
            result.Role = Map(role);
            result.Address = Map(address);

            return result;
        }

        public static AddressDto Map(Address address)
        {
            ValidateNull( Logger.Here(), address);

            return new AddressDto
            {
                Id = address.Id,
                RowVersion = address.RowVersion,
                CityRowVersion = address.City.RowVersion,
                CityId = address.CityId,
                PostalCode = address.City.PostalCode,
                City = address.City.Name,
                Street = address.Street,
                Number = address.Number
            };
        }

        public static RoleDto Map(Role role)
        {
            ValidateNull(Logger.Here(), role);

            return new RoleDto
            {
                Id = role.Id,
                RowVersion = role.RowVersion,
                Label = role.Label,
                MaxReservations = role.MaxReservations
            };
        }

        public static User Map(UserDto user)
        {
            ValidateNull( Logger.Here(), user);

            return new User
            {
                Id = user.Id,
                RowVersion = user.RowVersion,
                Uuid = user.Uuid,
                Deleted = false,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                RoleId = user.RoleId,
                Role = user.Role == null
                    ? null
                    : new Role
                    {
                        Id = user.Role.Id,
                        RowVersion = user.Role.RowVersion,
                        Label = user.Role.Label,
                        MaxReservations = user.Role.MaxReservations
                    },
                Address = user.Address == null
                    ? null
                    : new Address
                    {
                        Id = user.Address.Id,
                        RowVersion = user.Address.RowVersion,
                        Street = user.Address.Street,
                        Number = user.Address.Number,
                        City = new City
                        {
                            Id = user.Address.CityId,
                            RowVersion = user.Address.CityRowVersion,
                            PostalCode = user.Address.PostalCode,
                            Name = user.Address.City
                        }
                    }
            };
        }

        public static Ticket Map(TicketDto ticketDto)
        {
            ValidateNull( Logger.Here(), ticketDto);
            return new Ticket
            {
                Id = ticketDto.Id,
                RowVersion = ticketDto.RowVersion,
                Printed = ticketDto.Printed
            };
        }

        public static TicketDto Map(Ticket ticket)
        {
            ValidateNull( Logger.Here(), ticket);
            return new TicketDto
            {
                Id = ticket.Id,
                RowVersion = ticket.RowVersion,
                Printed = ticket.Printed
            };
        }

        public static Reservation Map(ReservationDto reservationDto)
        {
            ValidateNull( Logger.Here(), reservationDto);
            return new Reservation
            {
                Id = reservationDto.Id,
                RowVersion = reservationDto.RowVersion,
                Schedule = reservationDto.Schedule == null ? null : Map(reservationDto.Schedule),
                ScheduleId = reservationDto.ScheduleId,
                Ticket = reservationDto.Ticket == null ? null : Map(reservationDto.Ticket),
                TicketId = reservationDto.TicketId,
                User = reservationDto.User == null ? null : Map(reservationDto.User),
                UserId = reservationDto.UserId
            };
        }

        public static ReservationDto Map(Reservation reservation)
        {
            ValidateNull( Logger.Here(), reservation);
            return new ReservationDto
            {
                Id = reservation.Id,
                RowVersion = reservation.RowVersion,
                Schedule = reservation.Schedule == null ? null : Map(reservation.Schedule),
                ScheduleId = reservation.ScheduleId,
                Ticket = reservation.Ticket == null ? null : Map(reservation.Ticket),
                TicketId = reservation.TicketId,
                User = reservation.User == null ? null : Map(reservation.User),
                UserId = reservation.UserId
            };
        }

        public static SeatReservation Map(SeatReservationDto seatReservationDto)
        {
            ValidateNull( Logger.Here(), seatReservationDto);
            return new SeatReservation
            {
                Id = seatReservationDto.Id,
                RowVersion = seatReservationDto.RowVersion,
                Reservation = seatReservationDto.Reservation == null ? null : Map(seatReservationDto.Reservation),
                Seat = seatReservationDto.Seat == null ? null : Map(seatReservationDto.Seat),
                ReservationId = seatReservationDto.ReservationId,
                SeatId = seatReservationDto.SeatId
            };
        }

        public static SeatReservationDto Map(SeatReservation seatReservation)
        {
            ValidateNull( Logger.Here(), seatReservation);
            return new SeatReservationDto
            {
                Id = seatReservation.Id,
                RowVersion = seatReservation.RowVersion,
                Reservation = seatReservation.Reservation == null ? null : Map(seatReservation.Reservation),
                Seat = seatReservation.Seat == null ? null : Map(seatReservation.Seat),
                ReservationId = seatReservation.ReservationId,
                SeatId = seatReservation.SeatId
            };
        }
    }
}