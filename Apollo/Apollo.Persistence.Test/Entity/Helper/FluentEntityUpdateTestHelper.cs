using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Persistence.FluentEntity.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.Test.Entity.Mock;

namespace Apollo.Persistence.Test.Entity.Helper
{
    public class FluentEntityUpdateTestHelper
    {
        private readonly IFluentEntityFrom _fluentEntity;
        private readonly IList<GenreMock> _genreMocks;
        private readonly IList<ActorMock> _actorMocks;
        private readonly IList<MovieMock> _movieMocks;
        private readonly IList<MovieActorMock> _movieActorMocks;
        private static DateTime _now;

        public FluentEntityUpdateTestHelper(IEntityManager entityManager)
        {
            if (entityManager == null) throw new ArgumentNullException(nameof(entityManager));
            _fluentEntity = entityManager.FluentEntity();
            _genreMocks = CreateGenreMocks();
            _actorMocks = CreateActorMocks();
            _movieActorMocks = CreateMovieActorMocks();
            _movieMocks = CreateMovieMocks();
            _now = DateTime.UtcNow;
        }

        public int GenreCount => _genreMocks.Count;

        public async Task SetupAsync()
        {
            await _fluentEntity.InsertInto(_genreMocks).ExecuteAsync();
            await _fluentEntity.InsertInto(_actorMocks).ExecuteAsync();
            await _fluentEntity.InsertInto(_movieMocks).ExecuteAsync();
            await _fluentEntity.InsertInto(_movieActorMocks).ExecuteAsync();
        }

        public async Task TearDownAsync()
        {
            await _fluentEntity.Delete<MovieActorMock>().ExecuteAsync();
            await _fluentEntity.Delete<MovieMock>().ExecuteAsync();
            await _fluentEntity.Delete<ActorMock>().ExecuteAsync();
            await _fluentEntity.Delete<GenreMock>().ExecuteAsync();
        }

        private static IList<GenreMock> CreateGenreMocks()
        {
            return new List<GenreMock>
            {
                new GenreMock {Id = 1L, Name="Horror" , RowVersion = _now},
                new GenreMock {Id = 2L, Name="Romance", RowVersion = _now},
                new GenreMock {Id = 3L, Name="Action", RowVersion = _now},
                new GenreMock {Id = 4L, Name="Comedy", RowVersion = _now},
                new GenreMock {Id = 5L, Name="Sci-Fi", RowVersion = _now},
                new GenreMock {Id = 6L, Name="Action-Adventure", RowVersion = _now},
                new GenreMock {Id = 7L, Name="Fantasy", RowVersion = _now},
                new GenreMock {Id = 8L, Name="Splatter", RowVersion = _now}
            };
        }

        private static IList<ActorMock> CreateActorMocks()
        {
            return new List<ActorMock>
            {
                new ActorMock {Id = 1L,  RowVersion = _now, LastName = "Jolie", FirstName = "Angelina"},
                new ActorMock {Id = 2L,  RowVersion = _now, LastName = "Pitt", FirstName = "Brad"},
                new ActorMock {Id = 3L,  RowVersion = _now, LastName = "Cloney", FirstName = "George"},
                new ActorMock {Id = 4L,  RowVersion = _now, LastName = "Diesel", FirstName = "Vin"},
                new ActorMock {Id = 5L,  RowVersion = _now, LastName = "Depp", FirstName = "Jhonny"},
                new ActorMock {Id = 6L,  RowVersion = _now, LastName = "Brosnan", FirstName = "Pierce"}
            };
        }

        private static IList<MovieActorMock> CreateMovieActorMocks()
        {
            return new List<MovieActorMock>
            {
                new MovieActorMock{Id = 1L, RowVersion = _now, ActorId = 1L, MovieId = 1L},
                new MovieActorMock{Id = 2L, RowVersion = _now, ActorId = 6L, MovieId = 2L},
                new MovieActorMock{Id = 3L, RowVersion = _now, ActorId = 5L, MovieId = 3L},
            };
        }

        private static IList<MovieMock> CreateMovieMocks()
        {
            return new List<MovieMock>
            {
                new MovieMock{Id = 1L, RowVersion = _now, Duration = 1200, Description = "Pretty good movie...", GenreId = 3L, Title = "Salt", Trailer = "youtube.com/salt", Image= new byte[5], Rating = 1},
                new MovieMock{Id = 2L, RowVersion = _now, Duration = 1337, Description = "A secret agent saves the world...", GenreId = 3L, Title = "007-Golden Eye", Trailer = "youtube.com/james007" , Image= new byte[5], Rating = 3},
                new MovieMock{Id = 3L, RowVersion = _now, Duration = 4242, Description = "Something with ships in the caribbean...", GenreId = 3L, Title = "Pirates of the Caribbean", Trailer = "youtube.com/potc" , Image= new byte[5], Rating = 2},
                new MovieMock{Id = 4L, RowVersion = _now, Duration = 0, Description = "Dummy Movie", GenreId = 3L, Title = "Dummy Title", Trailer = "dummy.com" , Image= new byte[5], Rating = 0},
            };
        }
        
    }
}
