using Apollo.Api.Controllers.Base;
using Apollo.Core.Dto;
using Apollo.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Api.Authorization;
using Apollo.Util.Logger;
using LoggerFactory = Apollo.Util.Logger.LoggerFactory;

namespace Apollo.Api.Controllers
{
    public class MovieController : ApiControllerBase
    {
        private static readonly IApolloLogger<MovieController> Logger = LoggerFactory.CreateLogger<MovieController>();
        private readonly IServiceFactory _service;

        public MovieController(IServiceFactory service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Get all genres.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /v1/movie/genres
        ///
        /// </remarks>
        /// <returns>Return all active movie genres.</returns>
        /// <response code="200">Return all active movie genres.</response>
        [HttpGet("genres")]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenre()
        {
            Logger.Here().Info("{GetActors}", nameof(GetActors));
            return (await _service.CreateMovieService().GetActiveGenresAsync()).ToList();
        }

        /// <summary>
        /// Updates a genre.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///C
        ///     PUT /v1/movie/genre
        ///
        /// </remarks>
        /// <param name="genre">A genre.</param>
        /// <returns>Returns the updated genre.</returns>
        /// <response code="200">Returns the updated genre id.</response>
        /// <response code="400">Return Bad Request if the genre could not be updated.</response>
        [HttpPut("genre")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> UpdateGenre(GenreDto genre)
        {
            Logger.Here().Info("{UpdateGenre} - {genre}", nameof(UpdateGenre));
            var result = await _service.CreateMovieService().UpdateGenreAsync(genre);
            return GetUpdatedStatus(result, "Given genre is not updateable.");
        }

        /// <summary>
        /// Creates a new genre.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /v1/movie/genre
        ///
        /// </remarks>
        /// <param name="genre">A genre.</param>
        /// <returns>Returns the created genre.</returns>
        /// <response code="200">Returns the created genre id.</response>
        /// <response code="500">If a genre with the given id already exists.</response>
        [HttpPost("genre")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> CreateGenre(GenreDto genre)
        {
            Logger.Here().Info("{CreateGenre} - {genre}", nameof(CreateGenre), genre);
            var result = await _service.CreateMovieService().AddGenreAsync(genre);
            return GetCreatedStatus(result, null, "Given genre is not addable.");
        }

        /// <summary>
        /// Deletes a genre by id.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /v1/movie/genre/1
        ///
        /// </remarks>
        /// <param name="id">A genre id.</param>
        /// <response code="200">Returns OK if the genre could be deleted.</response>
        /// <response code="400">Returns Bad Request if the genre could not be deleted.</response>
        /// <response code="500">If the provided id does not exist.</response>
        [HttpDelete("genre/{id}")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> DeleteGenre(long id)
        {
            Logger.Here().Info($"{nameof(DeleteGenre)} - {nameof(id)}: {id}");
            if (await IsGenreReferenced(id))
            {
                Logger.Here().Error("{DeleteGenre} - Genre with {Id} cannot be deleted it is has still references", nameof(DeleteGenre), id);
                return BadRequestResponse("Genre cannot be deleted it is has still references");
            }

            var deleted = await _service.CreateMovieService().DeleteGenreAsync(id);
            return GetDeletionStatus(deleted);
        }

        private async Task<bool> IsGenreReferenced(long id)
        {
            return (await _service.CreateMovieService().GetActiveMoviesByGenreIdAsync(id)).Any();
        }

        /// <summary>
        /// Get all movies.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /v1/movie?genreIds=1,2,3&title=Terminator
        ///
        /// </remarks>
        /// <returns>Return all active movies by genre ids and/or title.</returns>
        /// <param name="genreIds">Genre ids.</param>
        /// <param name="title">A movie title - this can be a full title or even a part of it.</param>
        /// <response code="200">Return all active movie genres.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies([FromQuery] int page = 1,
            [FromQuery] int pageSize = 10, [FromQuery] string title = null,
            [FromQuery] IEnumerable<long> genreIds = null)
        {
            Logger.Here().Info("{GetMovies} - Request with {GenreIds} and {Title}", nameof(GetMovies), genreIds, title);
            if (!genreIds.Any() && string.IsNullOrWhiteSpace(title))
            {
                return (await _service.CreateMovieService().GetMoviesAsync(page, pageSize)).ToList();
            }

            if (genreIds.Any() && string.IsNullOrWhiteSpace(title))
            {
                return (await _service.CreateMovieService().GetActiveMoviesByGenreIdAsync(genreIds, page, pageSize))
                    .ToList();
            }

            if (!genreIds.Any() && !string.IsNullOrWhiteSpace(title))
            {
                return (await _service.CreateMovieService().GetActiveMoviesByTitleAsync(title, page, pageSize))
                    .ToList();
            }

            if (genreIds.Any() && !string.IsNullOrWhiteSpace(title))
            {
                var moviesByGenres =
                    (await _service.CreateMovieService().GetActiveMoviesByGenreIdAsync(genreIds, page, pageSize))
                    .ToList();
                return moviesByGenres.Where(m => m.Title.ToUpper().Contains(title.ToUpper())).ToList();
            }

            return BadRequestResponse();
        }

        /// <summary>
        /// Updates a movie.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /v1/movie
        ///
        /// </remarks>
        /// <returns>Returns the updated movie.</returns>
        /// <param name="movie">A movie.</param>
        /// <response code="200">Returns the updated movie id.</response>
        /// <response code="400">Return Bad Request if the movie could not be updated.</response>
        [HttpPut]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> UpdateMovie(MovieDto movie)
        {
            Logger.Here().Info("{UpdateMovie} - {Movie} with {Id}", nameof(UpdateMovie), movie.Title, movie.Id);
            var result = await _service.CreateMovieService().UpdateMovieAsync(movie);
            return GetUpdatedStatus(result, "Given movie is not updateable.");
        }

        /// <summary>
        /// Creates a movie.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /v1/movie
        ///
        /// </remarks>
        /// <returns> Returns the created movie.</returns>
        /// <param name="movie">A movie.</param>
        /// <response code="200">Returns the created movie id.</response>
        /// <response code="500">If a movie with the given id already exists.</response>
        [HttpPost]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> CreateMovie(MovieDto movie)
        {
            Logger.Here().Info("{CreateMovie} - {Movie}", nameof(CreateActor), movie);
            var result = await _service.CreateMovieService().AddMovieAsync(movie);
            return GetCreatedStatus(result, nameof(GetMovies), "Given movie is not addable.");
        }

        /// <summary>
        /// Deletes a movie.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /v1/movie/1
        ///
        /// </remarks>
        /// <param name="id">A movie id.</param>
        /// <response code="200">Returns OK if the movie could be deleted.</response>
        /// <response code="400">Returns Bad Request if the movie could not be deleted.</response>
        /// <response code="500">If the provided id does not exist.</response>
        [HttpDelete("{id}")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> DeleteMovie(long id)
        {
            Logger.Here().Info("{DeleteMovie} - {Id}", nameof(DeleteMovie), id);
            if (await IsMovieReferenced(id))
            {
                Logger.Here().Error("{DeleteMovie - Movie with {Id} cannot be deleted it is has still references", nameof(DeleteMovie), id);
                return BadRequestResponse("Movie cannot be deleted it is has still references");
            }

            var deleted = await _service.CreateMovieService().DeleteMovieAsync(id);
            return GetDeletionStatus(deleted);
        }

        private async Task<bool> IsMovieReferenced(long id)
        {
            return (await _service.CreateScheduleService().GetSchedulesAsync())
                .Any(s => s.MovieId == id);
        }

        /// <summary>
        /// Get all actors.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /v1/movie/actors?page=1&pageSize=50
        ///
        /// </remarks>
        ///  /// <param name="page">Number of the page.</param>
        ///  /// <param name="pageSize">Amount of entry per page.</param>
        /// <returns>Return all active actors.</returns>
        /// <response code="200">Return all active actors.</response>
        [HttpGet("actors")]
        public async Task<ActionResult<IEnumerable<ActorDto>>> GetActors(int page, int pageSize = 25)
        {
            Logger.Here().Info("{GetActors} - {Page} with {entries}", nameof(GetActors), page, pageSize);
            return (await _service.CreateMovieService().GetActiveActorsAsync(page, pageSize)).ToList();
        }

        /// <summary>
        /// Updates an actor.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /v1/movie/actor
        ///
        /// </remarks>
        /// <returns>Returns the actor movie.</returns>
        /// <param name="actor">An actor.</param>
        /// <response code="200">Returns the updated actor id.</response>
        ///  /// <response code="400">Return Bad Request if the actor could not be updated.</response>
        [HttpPut("actor")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> UpdateActor(ActorDto actor)
        {
            Logger.Here().Info("{UpdateActor} - Actor with {Id}", nameof(UpdateActor), actor.Id);
            var result = await _service.CreateMovieService().UpdateActorAsync(actor);
            return GetUpdatedStatus(result, "Given actor is not updateable.");
        }

        /// <summary>
        /// Creates an actor.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /v1/movie/actor
        ///
        /// </remarks>
        /// <returns> Returns the created actor.</returns>
        /// <param name="actor">An actor.</param>
        /// <response code="200">Returns the created actor id.</response>
        /// <response code="500">If an actor with the given id already exists.</response>
        [HttpPost("actor")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> CreateActor(ActorDto actor)
        {
            Logger.Here().Info("{CreateActor} - {Actor}", nameof(CreateActor), actor);
            var result = await _service.CreateMovieService().AddActorAsync(actor);
            return GetCreatedStatus(result, null, "Given actor is not addable.");
        }

        /// <summary>
        /// Deletes an actor.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /v1/movie/actor/1
        ///
        /// </remarks>
        /// <param name="id">An actor id.</param>
        /// <response code="200">Returns OK if the actor could be deleted.</response>
        /// <response code="400">Returns Bad Request if the actor could not be deleted.</response>
        /// <response code="500">If the provided id does not exist.</response>
        [HttpDelete("actor/{id}")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> DeleteActor(long id)
        {
            Logger.Here().Info("{DeleteActor} - {Id} ", nameof(DeleteActor), id);
            if (await IsActorReferenced(id))
            {
                Logger.Here().Error(
                    "{DeleteActor} - Actor with {Id} - Movie cannot be deleted it is has still references", nameof(DeleteActor), id);
                return BadRequestResponse("Movie cannot be deleted it is has still references");
            }

            var deleted = await _service.CreateMovieService().DeleteActorAsync(id);
            return GetDeletionStatus(deleted);
        }

        private async Task<bool> IsActorReferenced(long id)
        {
            return (await _service.CreateMovieService().GetMoviesByActorIdAsync(id)).Any();
        }

        /// <summary>
        /// Adds an actor to a movie.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /v1/movie/1/2
        ///
        /// </remarks>
        /// <response code="200">Returns OK if the actor is added to the movie.</response>
        [HttpPost("{movieId}/{actorId}")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> AddActorToMovie(long movieId, long actorId)
        {
            Logger.Here().Info("{AddActorToMovie} - Actor with {ActorId} added to Movie {MovieId}", nameof(AddActorToMovie), movieId, actorId);
            await _service.CreateMovieService().AddActorToMovieAsync(movieId, actorId);
            return CreatedIdResponse();
        }

        /// <summary>
        /// Get all actors from a movie.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /v1/movie/1/actors
        ///
        /// </remarks>
        /// <response code="200">Returns OK if the actor is added to the movie.</response>
        [HttpGet("{movieId}/actors")]
        public async Task<IEnumerable<ActorDto>> GetActorsFromMovie(long movieId)
        {
            Logger.Here().Info("{GetActorsFromMovie} - {MovieId}", nameof(GetActors), movieId);
            return (await _service.CreateMovieService().GetActorsByMovieIdAsync(movieId)).ToList();
        }

        /// <summary>
        /// Adds an actor to a movie.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /v1/movie/1/actors
        ///
        /// </remarks>
        /// <param name="actor">An actor.</param>
        /// <response code="200">Returns OK if the actor is added to the movie.</response>
        [HttpPost("{movieId}/actors")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> AddActorsToMovie(long movieId, IEnumerable<long> actorIds)
        {
            Logger.Here().Info("{AddActorToMovie} - Actors with {ActorIds} added to Movie {MovieId}", nameof(AddActorToMovie), movieId, actorIds);
            var tasks = actorIds.Select(id => _service.CreateMovieService().AddActorToMovieAsync(movieId, id))
                .ToList();
            await Task.WhenAll(tasks);
            return CreatedIdResponse();
        }


        /// <summary>
        /// Gets the image for a movie.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /v1/movie/image/12349
        ///
        /// </remarks>
        ///  /// <param name="id">The movie id.</param>
        /// <returns>Returns the image in form of a byte array.</returns>
        /// <response code="200">Returns the image in form of a byte array.</response>
        [HttpGet("image/{id}")]
        public async Task<ActionResult<byte[]>> GetImageByMovieId(long id)
        {
            Logger.Here().Info("{GetImageByMovieId} - Image for Movie with {Id}", nameof(GetImageByMovieId), id);
            return await _service.CreateMovieService().FetchImageByMovieId(id);
        }
    }
}