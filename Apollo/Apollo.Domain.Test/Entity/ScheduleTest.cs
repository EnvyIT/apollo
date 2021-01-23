using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class ScheduleTest : EntityTest<Schedule>
    {
        private readonly string _attributeTableName = "schedule";
        private readonly string _attributeColumnMovieId = "movie";
        private readonly string _attributeColumnCinemaHallId = "cinema_hall";
        private readonly string _attributeColumnPrice = "price";
        private readonly string _attributeColumnRefMovie = "movie";
        private readonly string _attributeColumnRefCinemaHall = "cinema_hall";
        private readonly string _attributeColumnStartTime = "start_time";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly long _movieId = 10;
        private readonly long _cinemaHallId = 20;
        private readonly decimal _price = new decimal(10.5);
        private readonly CinemaHall _cinemaHall = new CinemaHall { Id = 30, RowVersion = DateTime.UtcNow, Label = "Hall 15" };
        private readonly DateTime _startTime = DateTime.UtcNow;
        private readonly Movie _movie = new Movie
        {
            Id = 30,
            RowVersion = DateTime.UtcNow,
            Title = "Movie 1",
            Description = "Desc 1",
            Duration = 15,
            Trailer = "Trailer 1"
        };

        private readonly long _cloneId = 11L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly long _cloneMovieId = 11;
        private readonly long _cloneCinemaHallId = 21;
        private readonly decimal _clonePrice = new decimal(12.5);
        private readonly CinemaHall _cloneCinemaHall = new CinemaHall { Id = 31, RowVersion = DateTime.UtcNow, Label = "Hall 16" };
        private readonly DateTime _cloneStartTime = DateTime.UtcNow.AddDays(31);
        private readonly Movie _cloneMovie = new Movie
        {
            Id = 31,
            RowVersion = DateTime.UtcNow,
            Title = "Movie 2",
            Description = "Desc 2",
            Duration = 30,
            Trailer = "Trailer 2"
        };

        protected override void SetProperties(Schedule value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.MovieId = _movieId;
            value.CinemaHallId = _cinemaHallId;
            value.Price = _price;
            value.Movie = _movie;
            value.CinemaHall = _cinemaHall;
            value.StartTime = _startTime;
        }

        protected override void SetCloneProperties(Schedule value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.MovieId = _cloneMovieId;
            value.CinemaHallId = _cloneCinemaHallId;
            value.Price = _clonePrice;
            value.Movie = _cloneMovie;
            value.CinemaHall = _cloneCinemaHall;
            value.StartTime = _cloneStartTime;
        }

        protected override void CheckProperties(Schedule value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.MovieId.Should().Be(_movieId);
            value.CinemaHallId.Should().Be(_cinemaHallId);
            value.Price.Should().Be(_price);
            value.Movie.Should().Be(_movie);
            value.CinemaHall.Should().Be(_cinemaHall);
            value.StartTime.Should().Be(_startTime);
        }

        protected override void CheckClonedProperties(Schedule value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.MovieId.Should().Be(_cloneMovieId);
            value.CinemaHallId.Should().Be(_cloneCinemaHallId);
            value.Price.Should().Be(_clonePrice);
            value.Movie.Should().Be(_cloneMovie);
            value.CinemaHall.Should().Be(_cloneCinemaHall);
            value.StartTime.Should().Be(_cloneStartTime);
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _cinemaHallId, _movieId, _price, _startTime);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.CinemaHallId, _attributeColumnCinemaHallId);
            Attribute_Column_Name_Should(_ => _.MovieId, _attributeColumnMovieId);
            Attribute_Column_Name_Should(_ => _.Price, _attributeColumnPrice);
            Attribute_Column_Name_Should(_ => _.StartTime, _attributeColumnStartTime);
            Attribute_ColumnRef_Name_Should(_ => _.CinemaHall, _attributeColumnRefCinemaHall);
            Attribute_ColumnRef_Name_Should(_ => _.Movie, _attributeColumnRefMovie);
        }
    }
}