using System;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.FluentEntity;
using Apollo.Persistence.FluentEntity.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.Util;
using Apollo.UnitOfWork.Interfaces;
using Apollo.Util;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Apollo.UnitOfWork.Test
{
    public class UnitOfWorkTest
    {
        private IUnitOfWork _unitOfWork;
        private IEntityManager _entityManager;
        private IFluentEntityFrom _fluentEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ConfigurationHelper.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json")
                .Build();
            var appSettings = ConfigurationHelper.GetValues("Apollo_Test");
            var connectionFactory = new ConnectionFactory(appSettings[0]);
            var unitOfWorkFactory = new UnitOfWorkFactory();

            _entityManager = EntityManagerFactory.CreateEntityManager(connectionFactory);
            _fluentEntity = _entityManager.FluentEntity();
            _unitOfWork = unitOfWorkFactory.Create(connectionFactory);
        }
        
        [TearDown]
        public async Task TearDown()
        {
            await _fluentEntity.Delete<MovieActor>().ExecuteAsync();
            await _fluentEntity.Delete<Actor>().ExecuteAsync();
            await _fluentEntity.Delete<Movie>().ExecuteAsync();
            await _fluentEntity.Delete<Genre>().ExecuteAsync();
        }

        private async Task CreateMovie()
        {
            var actor = new Actor {FirstName = "First", LastName = "Last"};
            var actorId = await _unitOfWork.RepositoryMovie.AddActorAsync(actor);

            var genre = new Genre {Name = "Best"};
            var genreId = await _unitOfWork.RepositoryMovie.AddGenreAsync(genre);

            var movie = new Movie
            {
                Description = "d",
                Duration = 1,
                Image = new byte[] {1},
                Title = "Testmovie",
                Trailer = "t",
                GenreId = genreId
            };
            var movieId = await _unitOfWork.RepositoryMovie.AddMovieAsync(movie);

            await _unitOfWork.RepositoryMovie.AddActorToMovieAsync(movieId, actorId);
        }
        
        private async Task CreateMovieNestedTransaction()
        {
            await _entityManager.FluentTransaction()
                .PerformBlock(CreateMovie)
                .Commit();

            var genre = new Genre { Name = "Better" };
            await _unitOfWork.RepositoryMovie.AddGenreAsync(genre);
        }

        [Test]
        public async Task Test_Transaction_Commit()
        {
            await _unitOfWork.Transaction()
                .PerformBlock(CreateMovie)
                .Commit();

            var movies = (await _unitOfWork.RepositoryMovie.GetMoviesAsync()).ToList();
            movies.Should().HaveCount(1);

            var movie = movies.First();
            movie.Title.Should().Be("Testmovie");
            movie.Genre.Name.Should().Be("Best");

            var actors = (await _unitOfWork.RepositoryMovie.GetActorsByMovieIdAsync(movie.Id)).ToList();
            actors.Should().HaveCount(1);

            var actor = actors.First();
            actor.Name.Should().Be("First Last");
        }

        [Test]
        public async Task Test_Transaction_Rollback()
        {
            Func<Task> execution = async () => await _unitOfWork.Transaction()
                .PerformBlock(async () =>
                    {
                        await CreateMovie();
                        throw new InvalidOperationException("Failed");
                    }
                ).Commit();
            await execution.Should().ThrowAsync<InvalidOperationException>();

            var movies = await _unitOfWork.RepositoryMovie.GetMoviesAsync();
            movies.Should().HaveCount(0);

            var genres = await _unitOfWork.RepositoryMovie.GetGenresAsync();
            genres.Should().HaveCount(0);

            var actors = await _fluentEntity.SelectAll<Actor>().QueryAsync();
            actors.Should().HaveCount(0);
        }

        [Test]
        public async Task Test_Transaction_Nested_Commit()
        {
            await _unitOfWork.Transaction()
                .PerformBlock(CreateMovieNestedTransaction)
                .Commit();

            var movies = (await _unitOfWork.RepositoryMovie.GetMoviesAsync()).ToList();
            movies.Should().HaveCount(1);

            var movie = movies.First();
            movie.Title.Should().Be("Testmovie");
            movie.Genre.Name.Should().Be("Best");

            var actors = (await _unitOfWork.RepositoryMovie.GetActorsByMovieIdAsync(movie.Id)).ToList();
            actors.Should().HaveCount(1);

            var actor = actors.First();
            actor.Name.Should().Be("First Last");

            var genres = (await _unitOfWork.RepositoryMovie.GetGenresAsync()).Select(_ =>_.Name);
            genres.Should().Contain("Better");
        }

        [Test]
        public async Task Test_Transaction_Nested_Rollback()
        {
            Func<Task> execution = async () => await _unitOfWork.Transaction()
                .PerformBlock(async () =>
                    {
                        await CreateMovieNestedTransaction();
                        throw new InvalidOperationException("Failed");
                    }
                ).Commit();
            await execution.Should().ThrowAsync<InvalidOperationException>();

            var movies = await _unitOfWork.RepositoryMovie.GetMoviesAsync();
            movies.Should().HaveCount(0);

            var genres = await _unitOfWork.RepositoryMovie.GetGenresAsync();
            genres.Should().HaveCount(0);

            var actors = await _fluentEntity.SelectAll<Actor>().QueryAsync();
            actors.Should().HaveCount(0);
        }
    }
}