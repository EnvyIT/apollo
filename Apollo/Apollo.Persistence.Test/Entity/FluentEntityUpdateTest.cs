using System.Linq;
using System.Threading.Tasks;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.Test.Entity.Mock;
using Apollo.Util;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Persistence.Test.Entity
{
    public class FluentEntityUpdateTest : FluentEntityBaseTest
    {

        [SetUp]
        public async Task Setup()
        {
            await _updateHelper.SetupAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _updateHelper.TearDownAsync();
        }

        [Test]
        public async Task CallExecute_ShouldExecuteWithoutReturnValue()
        {

            var comedyGenre = await _fluentEntity.SelectAll<GenreMock>()
                .Where(gm => gm.Name)
                .Equal("Comedy")
                .QuerySingleAsync();

            const string horrorAdventure = "Horror-Adventure";

            comedyGenre.Name = horrorAdventure;

            ICallExecute directCall = _fluentEntity.Update(comedyGenre);

            await directCall.ExecuteAsync();

            var genres = await _fluentEntity.SelectAll<GenreMock>().QueryAsync();
            var renamedGenre = genres.SingleOrDefault(g => g.Name == horrorAdventure);

            renamedGenre.Should().NotBeNull();
            renamedGenre.Name.Should().Be(horrorAdventure);
        }

        [Test]
        public async Task UpdateHorrorGenre_ShouldReturnHorrorAdventureGenre()
        {

            var horrorGenre = await _fluentEntity.SelectAll<GenreMock>()
                .Where(gm => gm.Name)
                .Equal("Horror")
                .QuerySingleAsync();

            const string horrorAdventure = "Horror-Adventure";

            horrorGenre.Name = horrorAdventure;

            var updateCount = await _fluentEntity.Update<GenreMock>(horrorGenre).ExecuteAsync();

            updateCount.Should().Be(1);

            var genres = await _fluentEntity.SelectAll<GenreMock>().QueryAsync();
            var renamedGenre = genres.SingleOrDefault(g => g.Name == horrorAdventure);

            renamedGenre.Should().NotBeNull();
            renamedGenre.Name.Should().Be(horrorAdventure);
        }

        [Test]
        public async Task UpdateMultiple_ShouldReturnCorrectUpdatedEntities()
        {

            var genresWithS = await _fluentEntity.SelectAll<GenreMock>()
                .Where(gm => gm.Name)
                .StartsWith("S")
                .QueryAsync();

            const string horrorAdventure = "Horror-Adventure";

            var genreList = genresWithS.OrderBy(g => g.Name).ToList();
            genreList[0].Name = horrorAdventure;

            var updateCount = await _fluentEntity.Update(genreList).ExecuteAsync();

            updateCount.Should().Be(2);

            var genres = await _fluentEntity.SelectAll<GenreMock>()
                .QueryAsync();
            var orderedGenres = genres.OrderBy(g => g.Name).ToList();
            orderedGenres.Should().HaveCount(_updateHelper.GenreCount);
            orderedGenres[0].Name.Should().Be("Action");
            orderedGenres[1].Name.Should().Be("Action-Adventure");
            orderedGenres[2].Name.Should().Be("Comedy");
            orderedGenres[3].Name.Should().Be("Fantasy");
            orderedGenres[4].Name.Should().Be("Horror");
            orderedGenres[5].Name.Should().Be("Horror-Adventure");
            orderedGenres[6].Name.Should().Be("Romance");
            orderedGenres[7].Name.Should().Be("Splatter");
        }

        [Test]
        public async Task TwoSimultaneousUpdate_LatestUpdateShouldNotWin()
        {

            const string Jhonny = "Jhonny";
            const string Bonny = "Bonny";
            const string Ronny = "Ronny";

            var actors = await _fluentEntity.SelectAll<ActorMock>().QueryAsync();

            var actor = actors.First(a => a.FirstName.EqualsIgnoreCase(Jhonny));
            var actor2 = (ActorMock)actor.Clone();

            actor.Equals(actor2).Should().BeTrue();

            actor.FirstName = Bonny;
            var updateOne = _fluentEntity.Update(actor).ExecuteAsync();
            actor2.FirstName = Ronny;
            var updateTwo = _fluentEntity.Update(actor2).ExecuteAsync();


            await Task.WhenAll(updateOne, updateTwo);

            updateOne.Result.Should().Be(1);
            updateTwo.Result.Should().Be(0);

            actors = await _fluentEntity.SelectAll<ActorMock>().QueryAsync();
            var result = actors.SingleOrDefault(a => a.FirstName.EqualsIgnoreCase(Bonny));
            result.Should().NotBeNull();
            result.FirstName.Should().Be(Bonny);
            result.Name.Should().Be($"{Bonny} {actor.LastName}");

        }

        [Test]
        public async Task UpdateIdOfEntity_ShouldNotBePossible()
        {
            var movieActors = await _fluentEntity.SelectAll<MovieActorMock>()
                .InnerJoin<MovieActorMock, MovieMock, long, long>(ma => ma.MovieMock, ma => ma.MovieId, m => m.Id)
                .QueryAsync(); 
            var  movieActorList = movieActors.ToList();

            var firstMovieActor = movieActorList[0];

            var movie = firstMovieActor.MovieMock;
            const string newTitle = "This title should not be displayed";
            movie.Title = newTitle;
            var oldId =  movie.Id;
            const long newId = 4L;
            movie.Id = newId;

            await _fluentEntity.Update(movie).ExecuteAsync();


            var movies = await _fluentEntity.SelectAll<MovieMock>().QueryAsync();
            var result = movies.SingleOrDefault(m => m.Id == newId);
            result.Should().NotBeNull();
            result.Title.Should().NotBe(newTitle);

        }

        [Test]
        public async Task UpdateEntities_ShouldReturnUpdateCount()
        {
            var movies = await _fluentEntity.SelectAll<MovieMock>().QueryAsync();

            var movieCount = movies.Count();
            const string noTrailer = "TBD";
            foreach (var movie in movies)
            {
                movie.Trailer = noTrailer;
            }

            var updateCount = await _fluentEntity.Update(movies).ExecuteAsync();
            updateCount.Should().Be(movieCount);
        }

    }
}
