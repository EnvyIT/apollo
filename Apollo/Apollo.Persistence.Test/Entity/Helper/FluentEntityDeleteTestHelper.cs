using System;
using System.Collections.Generic;
using Apollo.Persistence.FluentEntity.Interfaces;
using System.Threading.Tasks;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.Test.Entity.Mock;

namespace Apollo.Persistence.Test.Entity.Helper
{
    public class FluentEntityDeleteTestHelper
    {
        private readonly IFluentEntityFrom _fluentEntity;
        private readonly IList<GenreMock> _genreMocks;

        public FluentEntityDeleteTestHelper(IEntityManager entityManager)
        {
            if (entityManager == null) throw new ArgumentNullException(nameof(entityManager));
            _fluentEntity = entityManager.FluentEntity();
            _genreMocks = CreateGenreMocks();
        }

        public int GenreCount => _genreMocks.Count;

        private static IList<GenreMock> CreateGenreMocks()
        {
            return new List<GenreMock>
            {
                new GenreMock {Id = 1L, Name="Horror"},
                new GenreMock {Id = 2L, Name="Romance"},
                new GenreMock {Id = 3L, Name="Action"},
                new GenreMock {Id = 4L, Name="Comedy"},
                new GenreMock {Id = 5L, Name="Sci-Fi"},
                new GenreMock {Id = 6L, Name="Action-Adventure"},
                new GenreMock {Id = 7L, Name="Fantasy"},
                new GenreMock {Id = 8L, Name="Splatter"}
            };
        }

        public async Task SetupAsync()
        {
            await FillGenres();
        }

        public async Task TearDownAsync()
        {
            await _fluentEntity.Delete<GenreMock>().ExecuteAsync();
        }

        
        private async Task FillGenres()
        {
            await _fluentEntity.InsertInto(_genreMocks).ExecuteAsync();
        }

    }
}
