using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Core.Interfaces;
using Apollo.Domain.Entity;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using static Apollo.Core.Dto.Mapper;

namespace Apollo.Core.Test
{
    public class MovieServiceTest
    {
        private MockingHelper _mockingHelper;
        private IMovieService _movieService;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _mockingHelper = new MockingHelper();
            _movieService = _mockingHelper.ServiceFactory.Object.CreateMovieService();
        }

        [Test]
        public async Task Test_Helper()
        {
            var expectedGenre = new Genre { Id = 1L, Deleted = false, Name = "Genre1" };
            _mockingHelper.RepositoryMovie
                .Setup(_ => _.GetGenresAsync())
                .ReturnsAsync(new List<Genre> { expectedGenre });

            var genres = (await _mockingHelper.RepositoryMovie.Object.GetGenresAsync()).ToList();

            genres.Should().HaveCount(1);
            genres.First().Should().Be(expectedGenre);
        }

        [Test]
        public async Task GetMovies_ShouldReturnedMappedMoviesWithGenres()
        {

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.GetMoviesAsync())
                .ReturnsAsync(CreateMovies);

            var result = (await _movieService.GetMoviesAsync()).ToList();

            result.Should().HaveCount(4);
            result[0].Title.Should().Be("Salt");
            result[1].Title.Should().Be("007-Golden Eye");
            result[2].Title.Should().Be("Pirates of the Caribbean");
            result[3].Title.Should().Be("Dummy Title");

            result[0].Genre.Should().NotBeNull();
            result[1].Genre.Should().NotBeNull();
            result[2].Genre.Should().NotBeNull();
            result[3].Genre.Should().NotBeNull();

            result[0].Genre.Name.Should().Be("Action");
            result[1].Genre.Name.Should().Be("Action");
            result[2].Genre.Name.Should().Be("Action");
            result[3].Genre.Name.Should().Be("Horror");

            result[0].Genre.Id.Should().Be(3L);
            result[1].Genre.Id.Should().Be(3L);
            result[2].Genre.Id.Should().Be(3L);
            result[3].Genre.Id.Should().Be(1L);
        }

        [Test]
        public async Task GetActors_ShouldReturnMappedActorsWithGenres()
        {

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.GetActiveActorsAsync())
                .ReturnsAsync(CreateActors);

            var result = (await _movieService.GetActiveActorsAsync(1, 25)).ToList();
            result.Should().HaveCount(6);
        }

        [Test]
        public async Task GetGenre_ShouldReturnMappedGenres()
        {

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.GetActiveGenresAsync())
                .ReturnsAsync(CreateGenres);

            var result = (await _movieService.GetActiveGenresAsync()).ToList();
            result.Should().HaveCount(8);
        }

        [Test]
        public async Task GetActiveMoviesByGenreId()
        {
            var movies = CreateMovies().Where(m => m.Id == 1L);

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.GetActiveMoviesByGenreIdAsync(1L))
                .ReturnsAsync(movies);

            var genreDto = new GenreDto
            {
                Id = 1L
            };

            var result = (await _movieService.GetActiveMoviesByGenreIdAsync(genreDto)).ToList();
            result.Should().HaveCount(1);
            result[0].Id.Should().Be(1L);
        }

        [Test]
        public async Task GetActiveMoviesByTitle()
        {
            var movies = CreateMovies().Where(m => m.Id == 1L);

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.GetActiveMoviesByTitleAsync("Horror"))
                .ReturnsAsync(movies);

            var genreDto = new GenreDto
            {
                Id = 1L
            };

            var result = (await _movieService.GetActiveMoviesByTitleAsync("Horror")).ToList();
            result.Should().HaveCount(1);
            result[0].Id.Should().Be(1L);
        }

        [Test]
        public async Task GetActiveMoviesByGenreIds()
        {
            var genreIds = CreateGenres().Select(g => g.Id);

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.GetActiveMoviesByGenreIdAsync(genreIds, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(CreateMovies);

            var genreDto = new GenreDto
            {
                Id = 1L
            };

            var result = (await _movieService.GetActiveMoviesByGenreIdAsync(genreIds, 1, 6)).ToList();
            result.Should().HaveCount(CreateMovies().Count());
        }

        [Test]
        public async Task GetActiveMovieById()
        {
            var movie = CreateMovies().Single(m => m.Id == 1L);

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.GetActiveMovieByIdAsync(1L))
                .ReturnsAsync(movie);

            var genreDto = new GenreDto
            {
                Id = 1L
            };

            var result = await _movieService.GetActiveMovieByIdAsync(1L);
            result.Id.Should().Be(1L);
        }

        [Test]
        public async Task UpdateMovie()
        {
            var movieDto = CreateMovies().Select(Map).Single(m => m.Id == 1L);
            var movie = Map(movieDto);
            _mockingHelper.RepositoryMovie
                .Setup(_ => _.UpdateMovieAsync(movie))
                .ReturnsAsync(1);


            var result = await _movieService.UpdateMovieAsync(movieDto);
            result.Should().Be(movieDto);
        }

        [Test]
        public async Task AddMovie()
        {
            var movieDto = CreateMovies().Select(Map).Single(m => m.Id == 1L);
            var movie = Map(movieDto);
            _mockingHelper.RepositoryMovie
                .Setup(_ => _.AddMovieAsync(movie))
                .ReturnsAsync(1L);

            _mockingHelper.RepositoryMovie.Setup(_ => _.GetActiveMovieByIdAsync(1L))
                .ReturnsAsync(movie);


            var result = await _movieService.AddMovieAsync(movieDto);
            result.Should().BeEquivalentTo(movieDto);
        }

        [Test]
        public async Task DeleteMovie()
        {
            var movieDto = CreateMovies().Select(Map).Single(m => m.Id == 1L);

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.DeleteMovieAsync(movieDto.Id))
                .ReturnsAsync(1);

            var result = await _movieService.DeleteMovieAsync(movieDto);
            result.Should().BeTrue();
        }

        [Test]
        public async Task AddMovie_WithActorIds()
        {
            var movieDto = CreateMovies().Select(Map).Single(m => m.Id == 1L);
            var movie = Map(movieDto);
            var actorIds = CreateActors().Select(a => a.Id);

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.AddMovieAsync(movie, actorIds))
                .ReturnsAsync(1L);

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.GetActiveMovieByIdAsync(1L))
                .ReturnsAsync(movie);

            var result = await _movieService.AddMovieAsync(movieDto, actorIds);
            result.Should().BeEquivalentTo(movieDto);
        }

        [Test]
        public async Task AddActorToMovieAsync()
        {
            _mockingHelper.RepositoryMovie.Setup(_ => _.AddActorToMovieAsync(It.IsAny<long>(), It.IsAny<long>()));
            await _movieService.AddActorToMovieAsync(1L, 13L);
            _mockingHelper.RepositoryMovie.Verify(_ => _.AddActorToMovieAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Once);
        }

        [Test]
        public async Task AddActorsToMovieAsync()
        {
            _mockingHelper.RepositoryMovie.Setup(_ => _.AddActorToMovieAsync(It.IsAny<long>(), It.IsAny<IEnumerable<long>>()));
            await _movieService.AddActorToMovieAsync(1L, new List<long>{ 13L, 42L, 122L});
            _mockingHelper.RepositoryMovie.Verify(_ => _.AddActorToMovieAsync(It.IsAny<long>(), It.IsAny<IEnumerable<long>>()), Times.Once);
        }


        [Test]
        public async Task RemoveActorFromMovie()
        {
            _mockingHelper.RepositoryMovie
                .Setup(_ => _.RemoveActorFromMovieAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(1);

            _mockingHelper.RepositoryMovie.Setup(_ => _.RemoveActorFromMovieAsync(It.IsAny<long>(), It.IsAny<IEnumerable<long>>()))
                .ReturnsAsync(1); ;

            var result = await _movieService.RemoveActorFromMovieAsync(1L, 1L);
            result.Should().BeTrue();

            result = await _movieService.RemoveActorFromMovieAsync(1L, new List<long>{ 42L, 13L, 37L});
            result.Should().BeTrue();
        }


        [Test]
        public async Task GetActorsByMovieId()
        {
            _mockingHelper.RepositoryMovie
                .Setup(_ => _.GetActorsByMovieIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateActors);

            var result = await _movieService.GetActorsByMovieIdAsync(1L);
            result.Should().HaveCount(6);
        }


        [Test]
        public async Task DeleteActor()
        {
            _mockingHelper.RepositoryMovie
                .Setup(_ => _.DeleteActorAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            var result = await _movieService.DeleteActorAsync(1L);
            result.Should().BeTrue();
        }

        [Test]
        public async Task AddActor()
        {
            var actor = CreateActors().First();
            var actorDto = Map(actor);

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.AddActorAsync(It.IsAny<Actor>()))
                .ReturnsAsync(It.IsAny<long>());

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.GetActorByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(actor);

            var result = await _movieService.AddActorAsync(actorDto);
            result.Should().BeEquivalentTo(actorDto);
        }


        [Test]
        public async Task UpdateActor()
        {
            var actor = CreateActors().First();
            var actorDto = Map(actor);

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.UpdateActorAsync(It.IsAny<Actor>()))
                .ReturnsAsync(1);

            var result = await _movieService.UpdateActorAsync(actorDto);
            result.Should().BeEquivalentTo(actorDto);
        }

        [Test]
        public async Task DeleteGenre()
        {
            _mockingHelper.RepositoryMovie
                .Setup(_ => _.DeleteGenreAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            var result = await _movieService.DeleteGenreAsync(1L);
            result.Should().BeTrue();
        }

        [Test]
        public async Task AddGenre()
        {

            var genre = CreateGenres().First();
            var genreDto = Map(genre);

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.AddGenreAsync(It.IsAny<Genre>()))
                .ReturnsAsync(It.IsAny<long>());

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.GetActiveGenreByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(genre);

            var result = await _movieService.AddGenreAsync(genreDto);
            result.Should().BeEquivalentTo(genreDto);
        }

        [Test]
        public async Task UpdateGenre()
        {

            var genre = CreateGenres().First();
            var genreDto = Map(genre);

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.UpdateGenreAsync(It.IsAny<Genre>()))
                .ReturnsAsync(1);


            var result = await _movieService.UpdateGenreAsync(genreDto);
            result.Should().BeEquivalentTo(genreDto);
        }

        [Test]
        public async Task DeleteMovieById()
        {

            _mockingHelper.RepositoryMovie
                .Setup(_ => _.DeleteMovieAsync(It.IsAny<long>()))
                .ReturnsAsync(1);


            var result = await _movieService.DeleteMovieAsync(1L);
            result.Should().BeTrue();
        }

        [Test]
        public void MapMovieActor()
        {

            var movieActor = CreateMovieActors().First();
            var movieActorDto = Map(movieActor);

            var movieActorRemapped = Map(movieActorDto);
            movieActorRemapped.Should().BeEquivalentTo(movieActor);
        }



        private static IEnumerable<Movie> CreateMovies()
        {
            return new List<Movie>
            {
                new Movie{Id = 1L, Duration = 1200, Description = "Pretty good movie...", GenreId = 3L, Genre =  new Genre {Id = 3L, Name = "Action"}, Title = "Salt", Trailer = "youtube.com/salt", Image= new byte[5]},
                new Movie{Id = 2L,  Duration = 1337, Description = "A secret agent saves the world...", GenreId = 3L,Genre =  new Genre {Id = 3L, Name = "Action"}, Title = "007-Golden Eye", Trailer = "youtube.com/james007" , Image= new byte[5]},
                new Movie{Id = 3L,  Duration = 4242, Description = "Something with ships in the caribbean...", GenreId = 3L,Genre =  new Genre {Id = 3L, Name = "Action"}, Title = "Pirates of the Caribbean", Trailer = "youtube.com/potc" , Image= new byte[5]},
                new Movie{Id = 4L,  Duration = 9120, Description = "Dummy Movie", GenreId = 3L,Genre =  new Genre {Id = 1L, Name = "Horror"}, Title = "Dummy Title", Trailer = "dummy.com" , Image= new byte[5]},
            };
        }

        private static IEnumerable<Genre> CreateGenres()
        {
            return new List<Genre>
            {
                new Genre {Id = 1L, Name = "Horror"},
                new Genre {Id = 2L, Name = "Romance"},
                new Genre {Id = 3L, Name = "Action"},
                new Genre {Id = 4L, Name = "Comedy"},
                new Genre {Id = 5L, Name = "Sci-Fi"},
                new Genre {Id = 6L, Name = "Action-Adventure"},
                new Genre {Id = 7L, Name = "Fantasy"},
                new Genre {Id = 8L, Name = "Splatter"}
            };
        }

        private static IEnumerable<Actor> CreateActors()
        {
            return new List<Actor>
            {
                new Actor {Id = 1L,  LastName = "Jolie", FirstName = "Angelina"},
                new Actor {Id = 2L,  LastName = "Pitt", FirstName = "Brad"},
                new Actor {Id = 3L, LastName = "Cloney", FirstName = "George"},
                new Actor {Id = 4L,   LastName = "Diesel", FirstName = "Vin"},
                new Actor {Id = 5L,   LastName = "Depp", FirstName = "Jhonny"},
                new Actor {Id = 6L,   LastName = "Brosnan", FirstName = "Pierce"}
            };
        }

        private static IEnumerable<MovieActor> CreateMovieActors()
        {
            return new List<MovieActor>
            {
                new MovieActor{Id = 1L, ActorId = 1L, MovieId = 1L, Actor = new Actor {Id = 1L,  LastName = "Jolie", FirstName = "Angelina"}, Movie = new Movie{Id = 1L, Duration = 1200, Description = "Pretty good movie...", GenreId = 3L, Genre =  new Genre {Id = 3L, Name = "Action"}, Title = "Salt", Trailer = "youtube.com/salt", Image= new byte[5]}},
            };
        }
    }
}