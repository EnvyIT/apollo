using System.Linq;
using System.Threading.Tasks;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.Test.Entity.Mock;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Persistence.Test.Entity
{
   public class FluentEntityDeleteTest : FluentEntityBaseTest
    {
        [SetUp]
        public async Task Setup()
        {
            await _deleteHelper.SetupAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _deleteHelper.TearDownAsync();
        }

        [Test]
        public async Task DeleteOneGenreByName_ShouldReturn7GenresWithoutHorror()
        {
            var result = await _fluentEntity.SelectAll<GenreMock>().QueryAsync();
            result.Should().HaveCount(_deleteHelper.GenreCount);

            await _fluentEntity.Delete<GenreMock>()
                .Where(gm => gm.Name)
                .Equal("Horror")
                .ExecuteAsync();

            result = await _fluentEntity.SelectAll<GenreMock>().QueryAsync();
            result.Should().HaveCount(_deleteHelper.GenreCount - 1);

        }

        [Test]
        public async Task DeleteGenresByNameOrGreater5Id_ShouldReturn4Genres()
        {
            var result = await _fluentEntity.SelectAll<GenreMock>().QueryAsync();
            result.Should().HaveCount(_deleteHelper.GenreCount);

            await _fluentEntity.Delete<GenreMock>()
                .Where(gm => gm.Name)
                .Equal("Horror")
                .Or(gm => gm.Id)
                .GreaterThan(5)
                .ExecuteAsync();

            result = await _fluentEntity.SelectAll<GenreMock>().QueryAsync();
            result.Should().HaveCount(4);

        }

        [Test]
        public async Task CallExecuteDirectlyFromInterface_ShouldDeleteToo()
        {
            var result = await _fluentEntity.SelectAll<GenreMock>().QueryAsync();
            result.Should().HaveCount(_deleteHelper.GenreCount);

            ICallExecute directCall = _fluentEntity.Delete<GenreMock>().Where(gm => gm.Name)
                .Equal("Horror")
                .Or(gm => gm.Id)
                .GreaterThan(5);

            await directCall.ExecuteAsync();

            result = await _fluentEntity.SelectAll<GenreMock>().QueryAsync();
            result.Should().HaveCount(4);
        }

        [Test]
        public async Task DeleteGenresByNameOrGreater5IdAndNameStartWithFan_ShouldReturn6Genres()
        {
            var result = await _fluentEntity.SelectAll<GenreMock>().QueryAsync();
            result.Should().HaveCount(_deleteHelper.GenreCount);

            await _fluentEntity.Delete<GenreMock>()
                .Where(gm => gm.Name)
                .Equal("Horror")
                .Or(gm => gm.Id)
                .GreaterThan(5)
                .And(gm => gm.Name)
                .StartsWith("Fan")
                .ExecuteAsync();

            result = await _fluentEntity.SelectAll<GenreMock>().QueryAsync();
            result.Should().HaveCount(6);
            var orderedGenres = result.OrderBy(genre => genre.Name).ToList();
            orderedGenres[0].Name.Should().Be("Action");
            orderedGenres[1].Name.Should().Be("Action-Adventure");
            orderedGenres[2].Name.Should().Be("Comedy");
            orderedGenres[3].Name.Should().Be("Romance");
            orderedGenres[4].Name.Should().Be("Sci-Fi");
            orderedGenres[5].Name.Should().Be("Splatter");
        }

        [Test]
        public async Task DeleteAllGenresLowerThanId8_ShouldReturnOneGenre()
        {
            var result = await _fluentEntity.SelectAll<GenreMock>().QueryAsync();
            result.Should().HaveCount(_deleteHelper.GenreCount);

            await _fluentEntity.Delete<GenreMock>()
                .Where(gm => gm.Id)
                .LowerThan(8)
                .ExecuteAsync();

            result = await _fluentEntity.SelectAll<GenreMock>().QueryAsync();
            var resultList = result.ToList();
            resultList.Should().HaveCount(1);
            resultList[0].Name.Should().Be("Splatter");
        }

        [Test]
        public async Task DeleteWhereClauseNonMatchingType_ShouldDeleteEverything()
        {
            var genres = await _fluentEntity.SelectAll<GenreMock>().QueryAsync();
            genres.Should().HaveCount(_deleteHelper.GenreCount);

            await _fluentEntity.Delete<GenreMock>()
                .Where(gm => gm.Name)
                .LowerThan(112341234)
                .ExecuteAsync();

            genres = (await _fluentEntity.SelectAll<GenreMock>().QueryAsync()).ToList();
            genres.Should().HaveCount(0);
        }

        [Test]
        public async Task DeleteGenresLessOrEquals_ShouldReturn2GenresLeft()
        {
            
            await _fluentEntity.Delete<GenreMock>()
                .Where(gm => gm.Id)
                .LowerThanEquals(6L)
                .ExecuteAsync();

            var genres = (await _fluentEntity.SelectAll<GenreMock>().QueryAsync()).ToList();
            genres.Should().HaveCount(2);
            genres.ForEach(g => g.Id.Should().BeGreaterOrEqualTo(7L));
        }

        [Test]
        public async Task DeleteGenresGreaterOrEquals_ShouldReturn5GenresLeft()
        {

            await _fluentEntity.Delete<GenreMock>()
                .Where(gm => gm.Id)
                .GreaterThanEquals(6L)
                .ExecuteAsync();

            var genres = (await _fluentEntity.SelectAll<GenreMock>().QueryAsync()).ToList();
            genres.Should().HaveCount(5);
            genres.ForEach(g => g.Id.Should().BeLessOrEqualTo(6L));
        }

    }
}
