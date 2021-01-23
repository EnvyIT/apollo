using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Exception;
using Apollo.Repository.Interfaces;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Repository.Test
{
    public class RepositoryMovieTest
    {
        private IRepositoryMovie _repositoryMovie;
        private RepositoryHelper _repositoryHelper;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryHelper = new RepositoryHelper();
            _repositoryHelper.FillTypes.Add(EntityType.Movie);
            _repositoryHelper.FillTypes.Add(EntityType.MovieActor);
            _repositoryHelper.FillTypes.Add(EntityType.Actor);
            _repositoryMovie = _repositoryHelper.RepositoryFactory.CreateRepositoryMovie();
        }

        [SetUp]
        public async Task Setup()
        {
            await _repositoryHelper.SeedDatabase();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _repositoryHelper.TearDown();
        }

        [Test]
        public async Task Test_GetAllMovies()
        {
            var movies = (await _repositoryMovie.GetMoviesAsync())
                .OrderBy(movie => movie.Id)
                .ToList();

            movies.Should().HaveCount(_repositoryHelper.Movies.Count());
            movies.ForEach(movie => _repositoryHelper.Movies.Contains(movie).Should().BeTrue());
        }

        [Test]
        public async Task Test_GetAllActiveMovies()
        {
            var movies = (await _repositoryMovie.GetActiveMoviesAsync())
                .OrderBy(movie => movie.Id)
                .ToList();

            movies.Should().HaveCount(_repositoryHelper.Movies.Count());
        }

        [Test]
        public async Task Test_GetGenres()
        {
            var genres = (await _repositoryMovie.GetGenresAsync())
                .OrderBy(movie => movie.Id)
                .ToList();

            genres.Should().HaveCount(_repositoryHelper.Genres.Count());
            genres.ForEach(movie => _repositoryHelper.Genres.Contains(movie).Should().BeTrue());
        }

        [Test]
        public async Task Test_GetMoviesByTitle()
        {
            const string movieTitle = "Salt";
            const string description = "Pretty good movie...";
            var movies = (await _repositoryMovie.GetActiveMoviesByTitleAsync(movieTitle)).ToList();

            movies.Should().HaveCount(1);
            movies[0].Title.Should().Be(movieTitle);
            movies[0].Description.Should().Be(description);
            movies[0].Rating.Should().Be(1);
        }

        [Test]
        public async Task Test_GetMoviesByGenre_Single()
        {
            var movies = (await _repositoryMovie.GetActiveMoviesByGenreIdAsync(3L)); //Genre: "Action"
            movies.Should().HaveCount(4);
        }

        [Test]
        public async Task Test_GetMoviesByGenre()
        {
            var movies = await _repositoryMovie.GetActiveMoviesByGenreIdAsync(new[] {1L, 2L});
            movies.Should().HaveCount(0);
        }

        [Test]
        public async Task Test_GetMoviesByTitle_Deleted()
        {
            var movies = (await _repositoryMovie.GetActiveMoviesByTitleAsync("Movie40"));
            movies.Should().HaveCount(0);
        }

        [Test]
        public async Task Test_GetActorsByMovieId()
        {
            var actors = (await _repositoryMovie.GetActorsByMovieIdAsync(1)).ToList();

            actors.Should().HaveCount(5);
            actors.ForEach(actor => _repositoryHelper.Actors.Contains(actor).Should().BeTrue());
        }

        [Test]
        public async Task Test_GetActiveGenres()
        {
            var genres = (await _repositoryMovie.GetActiveGenresAsync()).ToList();

            genres.Should().HaveCount(_repositoryHelper.Genres.Count());
            genres.ForEach(genre => _repositoryHelper.Genres.Contains(genre).Should().BeTrue());
        }

        [Test]
        public async Task Test_AddMovie_With_Invalid_GenreId_Should_ThrowException()
        {
            var movie = new Movie
            {
                Description = "Movie description",
                Duration = 129,
                GenreId = 500,
                Image = new byte[] {1, 2, 3},
                Title = "Best movie",
                Trailer = "http://trailer.at",
                Rating = 4
            };
            Func<Task> execution = async () => await _repositoryMovie.AddMovieAsync(movie);

            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddMovie_With_Invalid_GenreObject_Should_ThrowException()
        {
            var genre = new Genre {Id = 600, Name = "Great"};
            var movie = new Movie
            {
                Description = "Movie description",
                Duration = 129,
                Genre = genre,
                Image = new byte[] {1, 2, 3},
                Title = "Best movie",
                Trailer = "http://trailer.at",
                Rating = 2
            };
            Func<Task> execution = async () => await _repositoryMovie.AddMovieAsync(movie);

            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddMovie_Without_Genre_Should_ThrowException()
        {
            var movie = new Movie
            {
                Description = "Movie description",
                Duration = 129,
                Image = new byte[] {1, 2, 3},
                Title = "Best movie",
                Trailer = "http://trailer.at",
                Rating = 4
            };
            Func<Task> execution = async () => await _repositoryMovie.AddMovieAsync(movie);

            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddMovie_With_GenreObject()
        {
            const string title = "Fast & Furious";
            const string genreName = "Action";
            var movie = new Movie
            {
                Description = "Movie description",
                Duration = 129,
                GenreId = 3L, //Genre: "Action"
                Image = new byte[] {1, 2, 3},
                Title = title,
                Trailer = "http://trailer.at"
            };
            await _repositoryMovie.AddMovieAsync(movie);

            var resultMovie = (await _repositoryMovie.GetActiveMoviesByTitleAsync(title)).Single();

            resultMovie.Title.Should().Be(title);
            resultMovie.Genre.Name.Should().Be(genreName);
        }

        [Test]
        public async Task Test_AddMovie_With_GenreId()
        {
            var title = "Fast & Furious";
            var movie = new Movie
            {
                Description = "Movie description",
                Duration = 129,
                GenreId = 1L,
                Image = new byte[] {1, 2, 3},
                Title = title,
                Trailer = "http://trailer.at"
            };
            await _repositoryMovie.AddMovieAsync(movie);

            var resultMovie = (await _repositoryMovie.GetActiveMoviesByTitleAsync(title)).Single();

            resultMovie.Title.Should().Be(title);
            resultMovie.Genre.Name.Should().Be("Horror");
            resultMovie.Rating.Should().Be(0);
        }

        [Test]
        public async Task Test_Get_AllActors()
        {
            var actors = (await _repositoryMovie.GetActiveActorsAsync())
                .OrderBy(actor => actor.Id)
                .ToList();

            actors.Should().HaveCount(_repositoryHelper.Actors.Count());
            actors.ForEach(actor => _repositoryHelper.Actors.Contains(actor).Should().BeTrue());
        }

        [Test]
        public async Task Test_AddMovieWithActors_Without_Genre_Should_ThrowException()
        {
            var movie = new Movie
            {
                Description = "Movie description",
                Duration = 129,
                Image = new byte[] {1, 2, 3},
                Title = "Best movie",
                Trailer = "http://trailer.at"
            };
            Func<Task> execution = async () => await _repositoryMovie.AddMovieAsync(movie, new List<long>());

            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddMovieWithActors_With_Invalid_ActorId_ShouldThrow()
        {
            var movie = new Movie
            {
                Description = "Movie description",
                Duration = 129,
                Image = new byte[] {1, 2, 3},
                Title = "Best movie",
                Trailer = "http://trailer.at"
            };
            Func<Task> execution = async () => await _repositoryMovie.AddMovieAsync(movie, new List<long> {1L, 500L});

            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddMovieWithActors()
        {
            const string title = "Fast & Furious 16";
            var movie = new Movie
            {
                Description = "Movie description",
                Duration = 129,
                GenreId = 3L,
                Image = new byte[] {1, 2, 3},
                Title = title,
                Trailer = "http://trailer.at"
            };
            await _repositoryMovie.AddMovieAsync(movie, new List<long> {1L, 2L});

            var resultActors = (await _repositoryMovie.GetActiveActorsAsync()).Select(a => a.Name).ToList();
            var resultMovie = (await _repositoryMovie.GetActiveMoviesByTitleAsync(title)).Single();
            var resultActorsByMovie =
                (await _repositoryMovie.GetActorsByMovieIdAsync(resultMovie.Id)).Select(a => a.Name).ToList();

            const string expectedName1 = "Angelina Jolie";
            const string expectedName2 = "Brad Pitt";

            resultActors.Should().Contain(expectedName1);
            resultActors.Should().Contain(expectedName2);
            resultActorsByMovie.Should().HaveCount(2);
            resultActorsByMovie.Should().Contain(expectedName1);
            resultActorsByMovie.Should().Contain(expectedName2);
        }

        [Test]
        public async Task Test_AddActorsToMovies_With_Invalid_MovieId_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryMovie.AddActorToMovieAsync(500L, 1L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddActorsToMovies_With_Invalid_ActorId_ShouldThrow()
        {
            Func<Task> execution = async () =>
                await _repositoryMovie.AddActorToMovieAsync(1L, new List<long> {1L, 500L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddActorsToMovies()
        {
            await _repositoryMovie.AddActorToMovieAsync(1L, new List<long> {5L, 6L});

            var resultActorsByMovie = (await _repositoryMovie.GetActorsByMovieIdAsync(1L)).Select(a => a.Id).ToList();

            resultActorsByMovie.Should().Contain(5L);
            resultActorsByMovie.Should().Contain(6L);
        }

        [Test]
        public async Task Test_AddActor_DuplicateId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryMovie.AddActorAsync(new Actor
            {
                Id = 1L, RowVersion = DateTime.UtcNow, FirstName = "First", LastName = "Last"
            });
            await execution.Should().ThrowAsync<DuplicateEntryException>();
        }

        [Test]
        public async Task Test_AddActor()
        {
            var id = await _repositoryMovie.AddActorAsync(new Actor {FirstName = "New", LastName = "Actor"});
            var actors = (await _repositoryMovie.GetActiveActorsAsync()).ToList();

            actors.Should().Contain(_ => _.Id == id);
            actors.First(_ => _.Id == id).Name.Should().Be("New Actor");
        }

        [Test]
        public async Task Test_AddGenre_DuplicateId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryMovie.AddGenreAsync(new Genre
            {
                Id = 1L,
                RowVersion = DateTime.UtcNow,
                Name = "Best ever"
            });
            await execution.Should().ThrowAsync<DuplicateEntryException>();
        }

        [Test]
        public async Task Test_AddGenre()
        {
            var id = await _repositoryMovie.AddGenreAsync(new Genre {Name = "Best ever"});
            var genres = (await _repositoryMovie.GetActiveGenresAsync()).ToList();

            genres.Should().Contain(_ => _.Id == id);
            genres.First(_ => _.Id == id).Name.Should().Be("Best ever");
        }

        [Test]
        public async Task Test_DeleteMovie_InvalidId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryMovie.DeleteMovieAsync(500L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_DeleteMovieAsync()
        {
            var result = await _repositoryMovie.DeleteMovieAsync(1L);
            result.Should().Be(1);

            var movies = (await _repositoryMovie.GetActiveMoviesAsync()).ToList();
            var actors = (await _repositoryMovie.GetActorsByMovieIdAsync(1L)).ToList();

            movies.Should().HaveCount(_repositoryHelper.Movies.Count() - 1);
            movies.Select(_ => _.Id).Should().NotContain(1L);
            actors.Should().HaveCount(0);
        }

        [Test]
        public async Task Test_RemoveActorsFromMovie_InvalidMovieId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryMovie.RemoveActorFromMovieAsync(500L, 1L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_RemoveActorsFromMovie_InvalidActorId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryMovie.RemoveActorFromMovieAsync(1L, new[] {1L, 500L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_RemoveActorsFromMovie_Single()
        {
            var result = await _repositoryMovie.RemoveActorFromMovieAsync(1L, 1L);
            result.Should().Be(1);

            var actors = (await _repositoryMovie.GetActorsByMovieIdAsync(1L)).ToList();

            actors.Should().HaveCount(4);
            actors.Select(_ => _.Id).Should().NotContain(1L);
        }

        [Test]
        public async Task Test_RemoveActorsFromMovie_Multiple()
        {
            var result = await _repositoryMovie.RemoveActorFromMovieAsync(1L, new[] {1L, 3L});
            result.Should().Be(2);

            var actors = (await _repositoryMovie.GetActorsByMovieIdAsync(1L)).ToList();

            actors.Should().HaveCount(3);
            actors.Select(_ => _.Id).Should().NotContain(1L);
            actors.Select(_ => _.Id).Should().NotContain(3L);
        }

        [Test]
        public async Task Test_DeleteGenre_InvalidId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryMovie.DeleteGenreAsync(500L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_DeleteGenre()
        {
            var result = await _repositoryMovie.DeleteGenreAsync(2L);
            result.Should().Be(1);

            var genres = (await _repositoryMovie.GetActiveGenresAsync()).ToList();

            genres.Should().HaveCount(_repositoryHelper.Genres.Count() - 1);
            genres.Select(_ => _.Id).Should().NotContain(2L);
        }

        [Test]
        public async Task Test_DeleteActor_InvalidId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryMovie.DeleteActorAsync(500L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_DeleteActor()
        {
            var result = await _repositoryMovie.DeleteActorAsync(2L);
            result.Should().Be(1);

            var actors = (await _repositoryMovie.GetActiveActorsAsync()).ToList();
            var actorsInMovie = (await _repositoryMovie.GetActorsByMovieIdAsync(1L)).ToList();

            actors.Should().HaveCount(_repositoryHelper.Actors.Count() - 1 );
            actors.Select(_ => _.Id).Should().NotContain(2L);
            actorsInMovie.Should().HaveCount(4);
            actorsInMovie.Select(_ => _.Id).Should().NotContain(2L);
        }

        [Test]
        public async Task Test_UpdateMovie_InvalidId_Should_Throw()
        {
            var movie = new Movie {Id = 500L};
            Func<Task> execution = async () => await _repositoryMovie.UpdateMovieAsync(movie);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateMovie()
        {
            var movie = await _repositoryMovie.GetActiveMovieByIdAsync(1L);
            movie.GenreId = 5L;
            movie.Description = "New description";
            movie.Duration = 50;
            movie.Image = new byte[] {1, 2, 3};
            movie.Title = "Newest movie";
            movie.Trailer = "http://www.new.at";

            var result = await _repositoryMovie.UpdateMovieAsync(movie);
            result.Should().Be(1);

            var resultMovie = await _repositoryMovie.GetActiveMovieByIdAsync(1L);
            resultMovie.Should().Be(movie);
        }

        [Test]
        public async Task Test_UpdateGenre_InvalidId_Should_Throw()
        {
            var genre = new Genre {Id = 500L};
            Func<Task> execution = async () => await _repositoryMovie.UpdateGenreAsync(genre);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateGenre()
        {
            var genre = (await _repositoryMovie.GetActiveGenresAsync()).First(_ => _.Id == 1L);
            genre.Name = "Best Corona movies";

            var result = await _repositoryMovie.UpdateGenreAsync(genre);
            result.Should().Be(1);

            var resultGenres = (await _repositoryMovie.GetActiveGenresAsync()).ToList();
            resultGenres.Select(_ => _.Id).Should().Contain(1L);
            resultGenres.First(_ => _.Id == 1L).Equals(genre).Should().BeTrue();
        }

        [Test]
        public async Task Test_UpdateActor_InvalidId_Should_Throw()
        {
            var actor = new Actor {Id = 500L, FirstName = "Updated", LastName = "Actor"};
            Func<Task> execution = async () => await _repositoryMovie.UpdateActorAsync(actor);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateActor()
        {
            var actor = (await _repositoryMovie.GetActiveActorsAsync()).First(_ => _.Id == 1L);
            actor.FirstName = "Updated";
            actor.LastName = "Actor";

            var result = await _repositoryMovie.UpdateActorAsync(actor);
            result.Should().Be(1);

            var resultActors = (await _repositoryMovie.GetActiveActorsAsync()).ToList();
            resultActors.Select(_ => _.Id).Should().Contain(1L);
            resultActors.First(_ => _.Id == 1L).Equals(actor).Should().BeTrue();
        }

        [Test]
        public async Task Test_GetActorById()
        {
            var actor = await _repositoryMovie.GetActorByIdAsync(1L);
            actor.Id.Should().Be(1);
            actor.FirstName.Should().Be("Angelina");
            actor.LastName.Should().Be("Jolie");
        }

        [Test]
        public async Task Test_GetActiveGenreById()
        {
            var genre = await _repositoryMovie.GetActiveGenreByIdAsync(1L);
            genre.Id.Should().Be(1);
            genre.Name.Should().Be("Horror");
        }
    }
}