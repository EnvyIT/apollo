using System;
using System.Linq;
using NUnit.Framework;
using System.Threading.Tasks;
using Apollo.Persistence.Test.Entity.Mock;
using FluentAssertions;

namespace Apollo.Persistence.Test.Entity
{
    public class FluentEntitySelectTest : FluentEntityBaseTest
    {
        [SetUp]
        public async Task Setup()
        {
            await _selectHelper.SetupAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _selectHelper.TearDownAsync();
        }

        [Test]
        public async Task SelectAll_Genres_ShouldReturn_5()
        {
            var result = await _fluentEntity.SelectAll<GenreMock>().QueryAsync();
            result.Should().HaveCount(_selectHelper.GenreCount);
        }

        [Test]
        public async Task SelectAll_Genres_Limit_2_ShouldReturn_2()
        {
            const int limit = 2;
            var result = await _fluentEntity
                .SelectAll<GenreMock>()
                .Limit(limit)
                .QueryAsync();
            result.Should().HaveCount(limit);
        }

        [Test]
        public async Task SelectAll_Genres_Limit_1_QuerySingle_Should_NotThrow()
        {
            Func<Task> query = async () => await _fluentEntity
                    .SelectAll<GenreMock>()
                    .Limit(1)
                    .QuerySingleAsync();
            await query.Should().NotThrowAsync();
        }

        [Test]
        public async Task SelectAll_Genres_NoLimit_QuerySingle_Should_Throw()
        {
            Func<Task> query = async () => await _fluentEntity
                .SelectAll<GenreMock>()
                .QuerySingleAsync();
            await query.Should().ThrowAsync<InvalidOperationException>();
        }

        [Test]
        public async Task Select_Columns_Correctly()
        {
            var result = await _fluentEntity
                .Select<MovieMock>()
                .Column(_ => _.Id)
                .Column(_ => _.Title)
                .Column(_ => _.Rating)
                .Limit(1)
                .QuerySingleAsync();

            result.Id.Should().Be(1);
            result.Title.Should().Be("Movie1");
            result.Description.Should().BeNullOrEmpty();
            result.Duration.Should().Be(0);
            result.Image.Should().BeNullOrEmpty();
            result.Trailer.Should().BeNullOrEmpty();
            result.GenreId.Should().Be(0);
            result.Rating.Should().Be(1);
        }

        [Test]
        public async Task Select_MultiUse_AutoReset_Should_ResetCorrectly()
        {
            var select = _fluentEntity.Select<MovieMock>();
            await select
                .Column(_ => _.Id)
                .Column(_ => _.Title)
                .Limit(1)
                .QuerySingleAsync();

            var result = await select
                .Column(_ => _.Id)
                .Column(_ => _.Description)
                .Limit(1)
                .QuerySingleAsync();

            result.Id.Should().Be(1);
            result.Title.Should().BeNullOrEmpty();
            result.Description.Should().Be("Description1");
        }

        [Test]
        public async Task Select_MultiUse_NoAutoReset_Should_NotReset()
        {
            const int limit = 1;
            var select = _fluentEntity.SelectAll<MovieMock>(false).Limit(limit);

            var result1 = await select.QueryAsync();
            var result2 = await select.QueryAsync();

            result1.Should().HaveCount(limit);
            result2.Should().HaveCount(limit);
        }

        [Test]
        public async Task Select_Where_GreaterAndLower()
        {
            const int lower = 3;
            const int upper = 6;
            var result = (await _fluentEntity
                .SelectAll<MovieMock>()
                .Where(_ => _.Id)
                .GreaterThan(lower)
                .And(_ => _.Id)
                .LowerThan(upper)
                .QueryAsync()).ToList();

            result.Where(_ => _.Id <= lower).Should().HaveCount(0);
            result.Where(_ => _.Id >= upper).Should().HaveCount(0);
            result.Where(_ => _.Id > lower && _.Id < upper).Should().HaveCount(2);
        }

        [Test]
        public async Task Select_Where_Equal_1()
        {
            const int id = 3;
            var result = (await _fluentEntity
                .SelectAll<MovieMock>()
                .Where(_ => _.Id)
                .Equal(id)
                .QueryAsync()).ToList();

            result.Should().HaveCount(1);
        }

        [Test]
        public async Task Select_Where_In()
        {
            var result = (await _fluentEntity
                .SelectAll<MovieMock>()
                .Where(_ => _.Id)
                .In(new long[] { 1, 2, 3 })
                .QueryAsync()).ToList();

            result.Should().HaveCount(3);
        }

        [Test]
        public async Task Select_Where_MultipleConditions_And()
        {
            var result = (await _fluentEntity
                .SelectAll<MovieMock>()
                .Where(_ => _.Id)
                .GreaterThan(3)
                .And(_ => _.Id)
                .LowerThan(6)
                .And(_ => _.Duration)
                .Equal(5)
                .QueryAsync()).ToList();

            result.Should().HaveCount(1);
        }

        [Test]
        public async Task Select_Where_MultipleConditions_Or()
        {
            var result = (await _fluentEntity
                .SelectAll<MovieMock>()
                .Where(_ => _.Id)
                .GreaterThan(3)
                .And(_ => _.Id)
                .LowerThan(6)
                .Or(_ => _.Duration)
                .Equal(7)
                .QueryAsync()).ToList();

            result.Should().HaveCount(3);
        }

        [Test]
        public async Task Select_OrderBy_GenreName_Ascending()
        {
            var result = (await _fluentEntity
                .SelectAll<GenreMock>()
                .Order()
                .ByAscending(_ => _.Name)
                .QueryAsync())
                .ToList();

            result.Should().ContainInOrder(result.OrderBy(_ => _.Name));
        }

        [Test]
        public async Task Select_OrderBy_GenreName_Descending()
        {
            var result = (await _fluentEntity
                    .SelectAll<GenreMock>()
                    .Order()
                    .ByDescending(_ => _.Name)
                    .QueryAsync())
                .ToList();

            result.Should().ContainInOrder(result.OrderByDescending(_ => _.Name));
        }

        [Test]
        public async Task Select_OrderBy_Actor_Names_Descending()
        {
            var result = (await _fluentEntity
                    .SelectAll<ActorMock>()
                    .Order()
                    .ByDescending(_ => _.FirstName)
                    .ByDescending(_ => _.LastName)
                    .QueryAsync())
                .ToList();

            result.Should().BeEquivalentTo(result.OrderByDescending(_ => _.FirstName).ThenByDescending(_ => _.LastName));
        }

        [Test]
        public async Task Select_Join_MovieGenre()
        {
            var result = await _fluentEntity.Select<MovieMock>()
                .Column(_ => _.Title)
                .ColumnRef<GenreMock, string>(_ => _.Name)
                .InnerJoin<MovieMock, GenreMock, long, long>(m => m.GenreMock, m => m.GenreId, g => g.Id)
                .Limit(1)
                .QuerySingleAsync();

            result.Title.Should().Be("Movie1");
            result.GenreMock.Name.Should().Be("Genre1");
        }

        [Test]
        public async Task SelectAll_Join_MovieGenre()
        {
            var result = await _fluentEntity.SelectAll<MovieMock>()
                .InnerJoin<MovieMock, GenreMock, long, long>(m => m.GenreMock, m => m.GenreId, g => g.Id)
                .Limit(1)
                .QuerySingleAsync();

            result.Id.Should().Be(1);
            result.Title.Should().Be("Movie1");
            result.GenreMock.Id.Should().Be(1);
            result.GenreMock.Name.Should().Be("Genre1");
        }

        [Test]
        public async Task Select_Complex_MovieActorGenre()
        {
            var result = (await _fluentEntity.Select<MovieMock>()
                .Column(cw => cw.Id)
                .Column(cw => cw.Title)
                .ColumnRef<GenreMock, string>(_ => _.Name)
                .ColumnRef<ActorMock, string>(_ => _.FirstName)
                .InnerJoin<MovieMock, GenreMock, long, long>(m => m.GenreMock, m => m.GenreId, g => g.Id)
                .InnerJoin<MovieMock, MovieActorMock, long, long>(m => m.MovieActorMock, m => m.Id,
                    m => m.MovieId)
                .InnerJoin<MovieActorMock, ActorMock, long, long>(m => m.MovieActorMock.ActorMock, m => m.ActorId,
                    a => a.Id)
                .Where(_ => _.Id)
                .GreaterThan(3)
                .And(_ => _.Id)
                .LowerThan(5)
                .Order()
                .ByDescending(_ => _.GenreMock.Name)
                .ByDescending(_ => _.Id)
                .ByDescending(_ => _.MovieActorMock.ActorMock.FirstName)
                .Limit(2)
                .QueryAsync()
                ).ToList();

            result.Should().HaveCount(2);
            var movie = result.ElementAt(0);
            movie.Id.Should().Be(4);
            movie.Title.Should().Be("Movie4");
            movie.GenreMock.Name.Should().Be("Genre4");
            movie.MovieActorMock.ActorMock.FirstName.Should().Be("Firstname6");
        }

        [Test]
        public async Task SelectNullValueFromDb_ShouldReturnNullInObject()
        {
            var movies = await _fluentEntity.SelectAll<MovieMock>().QueryAsync();
            var moviesWithoutTrailer = movies.Where(m => m.Trailer == null);
            moviesWithoutTrailer.Should().HaveCount(5);
        }

        [Test]
        public async Task SelectBooleanValueFromDb_ShouldReturnBooleanInObject()
        {
            var movies = await _fluentEntity.SelectAll<MovieMock>().QueryAsync();
            var moviesWithoutTrailer = movies.Where(m => m.Trailer == null);
            moviesWithoutTrailer.ToList().ForEach(m =>
            {
                m.Deleted = true;
            });
            await _fluentEntity.Update(moviesWithoutTrailer).ExecuteAsync();
            movies = await _fluentEntity.SelectAll<MovieMock>().QueryAsync();
            var inactiveMovies = movies.Where(m => m.Deleted).ToList();
            inactiveMovies.Should().HaveCount(5);
        }

        [Test]
        public async Task Select_OnlyActive_Entries()
        {
            await _fluentEntity.InsertInto(new GenreMock { Name = "Deleted", Deleted = true }).ExecuteAsync();

            var allGenres = (await _fluentEntity.SelectAll<GenreMock>().QueryAsync()).ToList();
            var allActiveGenres = (await _fluentEntity.SelectAll<GenreMock>().WhereActive().QueryAsync()).ToList();

            allGenres.Should().HaveCount(6);
            allActiveGenres.Should().HaveCount(5);
        }

        [Test]
        public async Task Select_GreaterThanEquals()
        {

            var allGenres = (await _fluentEntity.SelectAll<GenreMock>()
                    .Where(m => m.Id)
                    .GreaterThanEquals(4L)
                    .QueryAsync())
                    .ToList();

            allGenres.Should().HaveCount(2);
            allGenres.ForEach(g => g.Id.Should().BeGreaterOrEqualTo(4L));
        }

        [Test]
        public async Task Select_LessThanEquals()
        {

            var allGenres = (await _fluentEntity.SelectAll<GenreMock>()
                    .Where(m => m.Id)
                    .LowerThanEquals(4L)
                    .QueryAsync())
                .ToList();

            allGenres.Should().HaveCount(4);
            allGenres.ForEach(g => g.Id.Should().BeLessOrEqualTo(4L));
        }

    }
}
