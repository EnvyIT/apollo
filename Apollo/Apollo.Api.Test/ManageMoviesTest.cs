using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Util;
using Apollo.Util.Logger;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Apollo.Api.Test
{
    public class ManageMoviesTest : BaseApiTest
    {
        private const string BaseUrl = "movie";
        private const string EndpointActor = "actor";
        private const string EndpointActors = "actors";
        private const string EndpointImage = "image";
        private const string EndpointGenre = "genre";
        private const string EndpointGenres = "genres";

        private const string ParameterTitle = "title";
        private const string ParameterGenreIds = "genreIds";

        private static readonly IApolloLogger<ManageMoviesTest> Logger =
            LoggerFactory.CreateLogger<ManageMoviesTest>();

        [Test]
        public async Task Test_Workflow_Genres_Valid()
        {
            Logger.Info($"Load genres ...");
            var response = await GetQuery(BaseUrl, EndpointGenres);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var genres = JsonConvert.DeserializeObject<IEnumerable<GenreDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();
            genres.Should().HaveCount(DataSeeder.Genres.Count);
            Logger.Info(genres.ToPrettyString());

            var newGenre = new GenreDto {Name = "New genre"};
            var updatedGenre = genres.First();
            updatedGenre.Name = "Updated genre";
            var deleteReferencedGenre =
                DataSeeder.Genres.First(g => DataSeeder.Movies.Select(m => m.GenreId).Contains(g.Id)).Id;
            var deletedGenre = DataSeeder.Genres
                .First(g => !DataSeeder.Movies.Select(m => m.GenreId).Contains(g.Id))
                .Id;

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info($"Add new genre: ${newGenre} ...");
            response = await Post(BaseUrl, EndpointGenre, newGenre);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Created);
            var newId = await GetCreatedIdAsync(response);

            Logger.Info($"Update genre: ${updatedGenre} ...");
            response = await Put(BaseUrl, EndpointGenre, updatedGenre);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info($"Try delete referenced genre: ${deleteReferencedGenre} ...");
            response = await Delete(BaseUrl, $"{EndpointGenre}/{deleteReferencedGenre}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            Logger.Info($"Delete genre: ${deletedGenre} ...");
            response = await Delete(BaseUrl, $"{EndpointGenre}/{deletedGenre}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info("Load genres ...");
            response = await GetQuery(BaseUrl, EndpointGenres);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            genres = JsonConvert.DeserializeObject<IEnumerable<GenreDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();

            genres.Select(g => g.Id).Should().Contain(newId);
            genres.Select(g => g.Id).Should().Contain(updatedGenre.Id);
            genres.Select(g => g.Id).Should().Contain(deleteReferencedGenre);
            genres.Select(g => g.Id).Should().NotContain(deletedGenre);

            genres.First(g => g.Id == newId).Name.Should().Be(newGenre.Name);
            genres.First(g => g.Id == updatedGenre.Id).Name.Should().Be(updatedGenre.Name);

            Logger.Info(genres.ToPrettyString());
        }

        [Test]
        public async Task Test_LoadAvailable_Movies()
        {
            var count = DataSeeder.Movies.Count;

            Logger.Info($"Load first {count} movies ...");
            var response = await GetQuery(BaseUrl, null, GetPageParameter(1, count));
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var movies = JsonConvert.DeserializeObject<IEnumerable<MovieDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();
            movies.Should().HaveCount(count);
            Logger.Info(movies.ToPrettyString());

            var movie = movies.First();
            Logger.Info($"Load picture for movie {movie.Title} ...");
            response = await Get(BaseUrl, $"{EndpointImage}/{movie.Id}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var picture = JsonConvert.DeserializeObject<byte[]>(await response.Content.ReadAsStringAsync());
            picture.Should().NotBeNull();
            picture.Length.Should().BeGreaterThan(0);
            Logger.Info(picture.ToString());

            Logger.Info($"Load first {count} actors for movie {movie.Title} ...");
            response = await Get(BaseUrl, $"{movie.Id}/{EndpointActors}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var actors = JsonConvert.DeserializeObject<IEnumerable<ActorDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();
            actors.Should().HaveCount(DataSeeder.MovieActors.Count(m => m.MovieId == movie.Id));
            Logger.Info(actors.ToPrettyString());
        }

        [Test]
        public async Task Test_Workflow_Movies()
        {
            var count = DataSeeder.Movies.Count;

            Logger.Info($"Load first {count} movies ...");
            var response = await GetQuery(BaseUrl, null, GetPageParameter(1, count));
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var movies = JsonConvert.DeserializeObject<IEnumerable<MovieDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();
            movies.Should().HaveCount(count);
            Logger.Info(movies.ToPrettyString());

            var newMovie = new MovieDto
                {Title = "Demo", Description = "Demo", Duration = 15, GenreId = 1L, Rating = 2};
            var updatedMovie = movies.First();
            updatedMovie.Title = "Demo";
            updatedMovie.Genre = null;
            updatedMovie.Rating = 3;
            var deleteReferencedMovie =
                movies.First(m => DataSeeder.MovieActors.Select(a => a.MovieId).Contains(m.Id)).Id;
            var deletedMovie = movies.First(m => !DataSeeder.MovieActors.Select(a => a.MovieId).Contains(m.Id)).Id;

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info($"Add new movie: ${newMovie} ...");
            response = await Post(BaseUrl, null, newMovie);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Created);
            var newId = await GetCreatedIdAsync(response);

            Logger.Info($"Update movie: ${updatedMovie} ...");
            response = await Put(BaseUrl, null, updatedMovie);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info($"Try delete referenced movie: ${deleteReferencedMovie} ...");
            response = await Delete(BaseUrl, deleteReferencedMovie.ToString());
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            Logger.Info($"Delete movie: ${deletedMovie} ...");
            response = await Delete(BaseUrl, deletedMovie.ToString());
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info($"Load movies ...");
            response = await GetQuery(BaseUrl, null, GetPageParameter(1, DataSeeder.Movies.Count));
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            movies = JsonConvert.DeserializeObject<IEnumerable<MovieDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();

            movies.Select(g => g.Id).Should().Contain(newId);
            movies.Select(g => g.Id).Should().Contain(updatedMovie.Id);
            movies.Select(g => g.Id).Should().NotContain(deletedMovie);

            movies.First(g => g.Id == newId).Title.Should().Be(newMovie.Title);
            movies.First(g => g.Id == updatedMovie.Id).Title.Should().Be(updatedMovie.Title);
            movies.First(g => g.Id == updatedMovie.Id).Rating.Should().Be(updatedMovie.Rating);

            Logger.Info(movies.ToPrettyString());
        }

        [Test]
        public async Task Test_Workflow_Actors()
        {
            Logger.Info("Load actors ...");
            var response = await GetQuery(BaseUrl, EndpointActors, GetPageParameter(1, DataSeeder.Actors.Count));
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var actors = JsonConvert.DeserializeObject<IEnumerable<ActorDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();
            actors.Should().HaveCount(DataSeeder.Actors.Count);
            Logger.Info(actors.ToPrettyString());

            var newActor = new ActorDto {FirstName = "New", LastName = "Actor"};
            var updatedActor = actors.First();
            updatedActor.FirstName = "New name";
            var deleteReferencedActor =
                DataSeeder.Actors.First(a => DataSeeder.MovieActors.Select(m => m.ActorId).Contains(a.Id)).Id;
            var deletedActor = DataSeeder.Actors
                .First(a => !DataSeeder.MovieActors.Select(m => m.ActorId).Contains(a.Id))
                .Id;

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info($"Add new actor: ${newActor} ...");
            response = await Post(BaseUrl, EndpointActor, newActor);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Created);
            var newId = await GetCreatedIdAsync(response);

            Logger.Info($"Update actor: ${updatedActor} ...");
            response = await Put(BaseUrl, EndpointActor, updatedActor);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info($"Try delete referenced actor: ${deleteReferencedActor} ...");
            response = await Delete(BaseUrl, $"{EndpointActor}/{deleteReferencedActor}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            Logger.Info($"Delete actor: ${deletedActor} ...");
            response = await Delete(BaseUrl, $"{EndpointActor}/{deletedActor}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info("Load actors ...");
            response = await GetQuery(BaseUrl, EndpointActors, GetPageParameter(1, DataSeeder.Actors.Count));
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            actors = JsonConvert.DeserializeObject<IEnumerable<ActorDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();

            actors.Select(g => g.Id).Should().Contain(newId);
            actors.Select(g => g.Id).Should().Contain(updatedActor.Id);
            actors.Select(g => g.Id).Should().NotContain(deletedActor);

            actors.First(g => g.Id == newId).FirstName.Should().Be(newActor.FirstName);
            actors.First(g => g.Id == updatedActor.Id).FirstName.Should().Be(updatedActor.FirstName);

            Logger.Info(actors.ToPrettyString());
        }

        [Test]
        public async Task Test_Workflow_AddActor_ToMovie()
        {
            var movieId = DataSeeder.Movies.First().Id;
            var actorId = DataSeeder.MovieActors
                .First(ma => !DataSeeder.MovieActors
                    .Where(m => m.MovieId == movieId)
                    .Select(m => m.ActorId)
                    .Contains(ma.ActorId))
                .ActorId;

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info($"Add actor {actorId} to movie {movieId} ...");
            var response = await Post<object>(BaseUrl, $"{movieId}/{actorId}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Created);

            Logger.Info("Load movie actors ...");
            response = await Get(BaseUrl, $"{movieId}/{EndpointActors}");
            var actors = JsonConvert
                .DeserializeObject<IEnumerable<ActorDto>>(await response.Content.ReadAsStringAsync()).ToList();
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            actors.Select(a => a.Id).Should().Contain(actorId);
            Logger.Info(actors.ToPrettyString());
        }

        [Test]
        public async Task Test_Workflow_FilterMovies()
        {
            const int pageSize = 5;
            var title = DataSeeder.Movies.First().Title;
            IEnumerable<long> genreIds = new[] {DataSeeder.Movies.First().GenreId, 2L};

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(ParameterTitle, title)
            };
            parameters.AddRange(GetPageParameter(1, pageSize));
            parameters.AddRange(
                genreIds.Select(id => new KeyValuePair<string, string>(ParameterGenreIds, id.ToString())));

            Logger.Info("Load movies ...");
            var response = await GetQuery(BaseUrl, null, parameters);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var movies = JsonConvert
                .DeserializeObject<IEnumerable<MovieDto>>(await response.Content.ReadAsStringAsync()).ToList();
            movies.Should().HaveCount(1);
            movies.First().Title.Should().Be(title);

            Logger.Info(movies.ToPrettyString());
        }

        [Test]
        public async Task Test_Workflow_Genres_Invalid()
        {
            await Authenticate(AdminUser, AdminPassword);

            var response = await Delete(BaseUrl, $"{EndpointGenre}/500");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointGenre, new GenreDto());
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointGenre, new GenreDto {Id = 500L});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointGenre, new GenreDto {Id = 500L, Name = "Invalid"});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Post(BaseUrl, EndpointActor, new GenreDto());
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Post(BaseUrl, EndpointActor, new GenreDto {Name = ""});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Post(BaseUrl, EndpointActor, new GenreDto {Name = "a"});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Test_Workflow_Actor_Invalid()
        {
            await Authenticate(AdminUser, AdminPassword);

            var response = await Delete(BaseUrl, $"{EndpointActor}/500");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointActor, new ActorDto());
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointActor, new ActorDto {Id = 500L});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointActor,
                new ActorDto {Id = 500L, FirstName = "First", LastName = "Last"});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Post(BaseUrl, EndpointActor, new ActorDto());
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Post(BaseUrl, EndpointActor, new ActorDto {FirstName = "", LastName = ""});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Post(BaseUrl, EndpointActor, new ActorDto {FirstName = "a", LastName = "b"});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Test_Workflow_Movie_Invalid()
        {
            await Authenticate(AdminUser, AdminPassword);

            var response = await Delete(BaseUrl, $"{EndpointActor}/500");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointActor, new MovieDto());
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointActor, new MovieDto {Id = 500L});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointActor, new MovieDto {Id = 500L, Title = ""});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointActor, new MovieDto {Description = "a"});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointActor, new MovieDto {Duration = 0});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointActor, new MovieDto {Rating = -1});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointActor, new MovieDto {Rating = 500});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Test_Workflow_Genre_UpdateParallel_Users()
        {
            const string newName = "New Genre";

            Logger.Info("Load genres ...");
            var response = await Get(BaseUrl, EndpointGenres);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var genres = JsonConvert
                .DeserializeObject<IEnumerable<GenreDto>>(await response.Content.ReadAsStringAsync()).ToList();
            genres.Should().HaveCount(DataSeeder.Genres.Count);
            Logger.Info(genres.ToPrettyString());

            var genre = genres.First();
            genre.Name = newName;

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info("Update genre ...");
            response = await Put(BaseUrl, EndpointGenre, genre);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info("Try to update outdated genre ...");
            genre.Name = "Best";
            response = await Put(BaseUrl, EndpointGenre, genre);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            Logger.Info("Load genres ...");
            response = await Get(BaseUrl, EndpointGenres);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            genres = JsonConvert
                .DeserializeObject<IEnumerable<GenreDto>>(await response.Content.ReadAsStringAsync()).ToList();

            genres.First(g => g.Id == genre.Id).Name.Should().Be(newName);

            Logger.Info(genre.ToPrettyString());
        }

        [Test]
        public async Task Test_Workflow_Movie_UpdateParallel_Users()
        {
            const string newTitle = "New Movie";

            Logger.Info("Load movies ...");
            var response = await GetQuery(BaseUrl, null, GetPageParameter(1, DataSeeder.Movies.Count));
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var movies = JsonConvert
                .DeserializeObject<IEnumerable<MovieDto>>(await response.Content.ReadAsStringAsync()).ToList();
            movies.Should().HaveCount(DataSeeder.Movies.Count);
            Logger.Info(movies.ToPrettyString());

            var movie = movies.First();
            movie.Title = newTitle;
            movie.Genre = null;
            movie.MovieActor = null;

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info("Update movie ...");
            response = await Put(BaseUrl, null, movie);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info("Try to update outdated movie ...");
            movie.Title = "Best movie";
            response = await Put(BaseUrl, null, movie);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            Logger.Info("Load movies ...");
            response = await GetQuery(BaseUrl, null, GetPageParameter(1, DataSeeder.Movies.Count));
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            movies = JsonConvert
                .DeserializeObject<IEnumerable<MovieDto>>(await response.Content.ReadAsStringAsync()).ToList();

            movies.First(g => g.Id == movie.Id).Title.Should().Be(newTitle);

            Logger.Info(movie.ToPrettyString());
        }

        [Test]
        public async Task Test_Workflow_Actor_UpdateParallel_Users()
        {
            const string newName = "New Firstname";

            Logger.Info("Load actors ...");
            var response = await GetQuery(BaseUrl, EndpointActors, GetPageParameter(1, DataSeeder.Actors.Count));
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var actors = JsonConvert
                .DeserializeObject<IEnumerable<ActorDto>>(await response.Content.ReadAsStringAsync()).ToList();
            actors.Should().HaveCount(DataSeeder.Actors.Count);
            Logger.Info(actors.ToPrettyString());

            var actor = actors.First();
            actor.FirstName = newName;

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info("Update actor ...");
            response = await Put(BaseUrl, EndpointActor, actor);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info("Try to update outdated actor ...");
            actor.FirstName = "Max";
            response = await Put(BaseUrl, EndpointActor, actor);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            Logger.Info("Load actors ...");
            response = await GetQuery(BaseUrl, EndpointActors, GetPageParameter(1, DataSeeder.Actors.Count));
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            actors = JsonConvert
                .DeserializeObject<IEnumerable<ActorDto>>(await response.Content.ReadAsStringAsync()).ToList();

            actors.First(g => g.Id == actor.Id).FirstName.Should().Be(newName);

            Logger.Info(actor.ToPrettyString());
        }

        [Test]
        public async Task Test_Delete_WithoutAuthentication()
        {
            var urls = new[]
            {
                "1",
                $"{EndpointGenre}/1",
                $"{EndpointActor}/1"
            };
            foreach (var endpoint in urls)
            {
                var response = await Delete(BaseUrl, endpoint);
                await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Unauthorized);
            }
        }

        [Test]
        public async Task Test_Create_WithoutAuthentication()
        {
            var endpoints = new[]
            {
                null,
                EndpointGenre,
                EndpointActor
            };
            foreach (var endpoint in endpoints)
            {
                var response = await Post(BaseUrl, endpoint, new object());
                await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Unauthorized);
            }
        }

        [Test]
        public async Task Test_Update_WithoutAuthentication()
        {
            var endpoints = new[]
            {
                null,
                EndpointGenre,
                EndpointActor
            };
            foreach (var endpoint in endpoints)
            {
                var response = await Put(BaseUrl, endpoint, new object());
                await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Unauthorized);
            }
        }
    }
}