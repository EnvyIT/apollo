using System;
using System.Collections.Generic;
using System.Linq;
using Apollo.Domain.Entity;
using Apollo.Persistence.FluentEntity.Interfaces;
using System.Threading.Tasks;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.Test.Entity.Mock;

namespace Apollo.Persistence.Test.Entity.Helper
{
    public class FluentEntitySelectTestHelper
    {
        private readonly IFluentEntityFrom _fluentEntity;
        private readonly IList<GenreMock> _genreMocks;
        private readonly IList<ActorMock> _actorMocks;
        private readonly IList<MovieMock> _movieMocks;
        private readonly IList<MovieActorMock> _movieActorMocks;

        public int GenreCount => _genreMocks.Count;

        public FluentEntitySelectTestHelper(IEntityManager entityManager)
        {
            if (entityManager == null) throw new ArgumentNullException(nameof(entityManager));
            _fluentEntity = entityManager.FluentEntity();
            _genreMocks = CreateGenreMocks();
            _movieMocks = CreateMovieMocks();
            _actorMocks = CreateActorMocks();
            _movieActorMocks = CreateMovieActorMocks();
        }

        private static IList<GenreMock> CreateGenreMocks()
        {
            return Enumerable.Range(1, 5).Select(index => new GenreMock
            {
                Id = index,
                RowVersion = DateTime.UtcNow,
                Name = $"Genre{index}"
            }).ToList();
        }

        private static IList<ActorMock> CreateActorMocks()
        {
            return Enumerable.Range(1, 10).Select(index => new ActorMock
            {
                Id = index,
                RowVersion = DateTime.UtcNow,
                FirstName = $"Firstname{index}",
                LastName = $"Lastname{index}"
            }).ToList();
        }

        private IList<MovieActorMock> CreateMovieActorMocks()
        {
            const int actorCount = 6;
            var data = new List<MovieActorMock>();
            for (var i = 1; i <= _movieMocks.Count; i++)
            {
                for (var j = 0; j < actorCount; j++)
                {
                    data.Add(new MovieActorMock
                    {
                        Id = (i - 1) * actorCount + j + 1,
                        RowVersion = DateTime.UtcNow,
                        MovieId = i,
                        ActorId = (i - 1 + j) % actorCount + 1
                    });
                }
            }
            return data;
        }

        private IList<MovieMock> CreateMovieMocks()
        {
            return Enumerable.Range(1, 10).Select(index => new MovieMock
            {
                Id = index,
                RowVersion = DateTime.UtcNow,
                Description = $"Description{index}",
                Duration = index,
                GenreId = (index - 1) % GenreCount + 1,
                Image = new byte[] { },
                Title = $"Movie{index}",
                Trailer = index % 2 == 0 ? $"Trailer{index}" : null,
                Rating = index % 6
            }).ToList();
        }

        public async Task SetupAsync()
        {
            await FillGenres();
            await FillActors();
            await FillMovies();
            await FillMovieActor();
        }

        public async Task TearDownAsync()
        {
            await _fluentEntity.Delete<MovieActorMock>().ExecuteAsync();
            await _fluentEntity.Delete<ActorMock>().ExecuteAsync();
            await _fluentEntity.Delete<MovieMock>().ExecuteAsync();
            await _fluentEntity.Delete<Genre>().ExecuteAsync();
        }

        private async Task FillGenres()
        {
            await _fluentEntity.InsertInto(_genreMocks).ExecuteAsync();
        }

        private async Task FillActors()
        {
          
            await _fluentEntity.InsertInto(_actorMocks).ExecuteAsync();
        }

        private async Task FillMovies()
        {
            await _fluentEntity.InsertInto(_movieMocks).ExecuteAsync();
        }

        private async Task FillMovieActor()
        {
            
            await _fluentEntity.InsertInto(_movieActorMocks).ExecuteAsync();
        }
    }
}
