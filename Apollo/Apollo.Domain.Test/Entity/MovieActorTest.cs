using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class MovieActorTest : EntityTest<MovieActor>
    {
        private readonly string _attributeTableName = "movie_actor";
        private readonly string _attributeColumnMovieId = "movie_id";
        private readonly string _attributeColumnActorId = "actor_id";
        private readonly string _attributeColumnRefMovie = "movie";
        private readonly string _attributeColumnRefActor = "actor";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly long _movieId = 10L;
        private readonly long _actorId = 20L;
        private readonly Movie _movie = new Movie
        {
            Id = 30L,
            RowVersion = DateTime.UtcNow,
            Description = "Description",
            Duration = 15L,
            GenreId = 40L,
            Image = new byte[] { 1 },
            Title = "Movie",
            Trailer = "Trailer"
        };
        private readonly Actor _actor = new Actor { Id = 40L, RowVersion = DateTime.UtcNow, FirstName = "First", LastName = "Last" };

        private readonly long _cloneId = 10L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly long _cloneMovieId = 11L;
        private readonly long _cloneActorId = 21L;
        private readonly Movie _cloneMovie = new Movie
        {
            Id = 31L,
            RowVersion = DateTime.UtcNow,
            Description = "Description clone",
            Duration = 16L,
            GenreId = 41L,
            Image = new byte[] { 2 },
            Title = "Movie clone",
            Trailer = "Trailer clone"
        };
        private readonly Actor _cloneActor = new Actor { Id = 41L, RowVersion = DateTime.UtcNow, FirstName = "First clone", LastName = "Last clone" };

        protected override void SetProperties(MovieActor value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.MovieId = _movieId;
            value.ActorId = _actorId;
            value.Movie = _movie;
            value.Actor = _actor;
        }

        protected override void SetCloneProperties(MovieActor value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.MovieId = _cloneMovieId;
            value.ActorId = _cloneActorId;
            value.Movie = _cloneMovie;
            value.Actor = _cloneActor;
        }

        protected override void CheckProperties(MovieActor value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.MovieId.Should().Be(_movieId);
            value.ActorId.Should().Be(_actorId);
            value.Movie.Should().Be(_movie);
            value.Actor.Should().Be(_actor);
        }

        protected override void CheckClonedProperties(MovieActor value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.MovieId.Should().Be(_cloneMovieId);
            value.ActorId.Should().Be(_cloneActorId);
            value.Movie.Should().Be(_cloneMovie);
            value.Actor.Should().Be(_cloneActor);
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _movieId, _actorId);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.MovieId, _attributeColumnMovieId);
            Attribute_Column_Name_Should(_ => _.ActorId, _attributeColumnActorId);
            Attribute_ColumnRef_Name_Should(_ => _.Movie, _attributeColumnRefMovie);
            Attribute_ColumnRef_Name_Should(_ => _.Actor, _attributeColumnRefActor);
        }
    }
}