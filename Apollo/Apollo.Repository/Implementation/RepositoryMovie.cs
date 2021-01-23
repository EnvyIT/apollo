using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Exception;
using Apollo.Persistence.Util;
using Apollo.Repository.Interfaces;
using Apollo.Util.Logger;

namespace Apollo.Repository.Implementation
{
    public class RepositoryMovie : Repository, IRepositoryMovie
    {
        private static readonly IApolloLogger<RepositoryMovie> Logger = LoggerFactory.CreateLogger<RepositoryMovie>();
        private readonly IMovieDao _movieDao;
        private readonly IGenreDao _genreDao;
        private readonly IMovieActorDao _movieActorDao;
        private readonly IActorDao _actorDao;

        public RepositoryMovie(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _movieDao = DaoFactory.CreateMovieDao();
            _genreDao = DaoFactory.CreateGenreDao();
            _movieActorDao = DaoFactory.CreateMovieActorDao();
            _actorDao = DaoFactory.CreateActorDao();
        }

        public async Task<IEnumerable<Movie>> GetMoviesAsync()
        {
            return await _movieDao.SelectWithReferencesAsync();
        }

        public async Task<IEnumerable<Movie>> GetMoviesAsync(int page, int pageSize)
        {
            return await _movieDao.SelectPagedAsync(page, pageSize);
        }

        public async Task<IEnumerable<Movie>> GetActiveMoviesByTitleAsync(string title, int page, int pageSize)
        {
            return await _movieDao.SelectByTitlePagedAsync(title, page, pageSize);
        }

        public async Task<IEnumerable<Movie>> GetActiveMoviesByGenreIdAsync(IEnumerable<long> genreIds, int page, int pageSize)
        {
            return await _movieDao.SelectByGenreIdPagedAsync(genreIds, page, pageSize);
        }

        public async  Task<IEnumerable<Genre>> GetGenresAsync()
        {
            return await _genreDao.SelectActiveAsync();
        }

        public async Task<Movie> GetActiveMovieByIdAsync(long id)
        {
            await ValidateId(_movieDao, id);
            return await _movieDao.SelectWithReferencesByIdAsync(id);
        }

        public async Task<IEnumerable<Movie>> GetActiveMoviesAsync()
        {
            return await _movieDao.SelectActiveWithGenreAsync();
        }

        public async Task<IEnumerable<Movie>> GetActiveMoviesByGenreIdAsync(long genreId)
        {
            await ValidateId(_genreDao, genreId);
            return await GetActiveMoviesByGenreIdAsync(new[] {genreId});
        }

        public async Task<IEnumerable<Movie>> GetActiveMoviesByGenreIdAsync(IEnumerable<long> genreIds)
        {
            await ValidateIds(_genreDao, genreIds);
            return await _movieDao.SelectByGenreIdAsync(genreIds);
        }

        public async Task<IEnumerable<Movie>> GetMoviesByActorIdAsync(long actorId)
        {
            await ValidateId(_actorDao, actorId);
            return await _movieActorDao.SelectMoviesByActorId(actorId);
        }

        public async Task<IEnumerable<Actor>> GetActiveActorsAsync()
        {
            return await  _actorDao.SelectActiveAsync();
        }

        public async Task<IEnumerable<Actor>> GetActorsByMovieIdAsync(long movieId)
        {
            await ValidateId(_movieDao, movieId);
            return await _movieActorDao.SelectActorsByMovieIdAsync(movieId);
        }

        public async Task<IEnumerable<Genre>> GetActiveGenresAsync()
        {
            return await _genreDao.SelectActiveAsync();
        }

        public async Task<IEnumerable<Movie>> GetActiveMoviesByTitleAsync(string title)
        {
            return await  _movieDao.SelectByTitleAsync(title);
        }

        public async Task<long> AddMovieAsync(Movie movie)
        {
            if (movie.Genre != null && movie.GenreId <= 0)
            {
                movie.GenreId = movie.Genre.Id;
            }
            await ValidateId(_genreDao, movie.GenreId);

            return (await _movieDao.FluentInsert(movie).ExecuteAsync()).FirstOrDefault();
        }

        public async Task<long> AddMovieAsync(Movie movie, IEnumerable<long> actorIds)
        {
            var actorList = actorIds.ToList();
            await ValidateIds(_actorDao, actorList);

            var movieId = await AddMovieAsync(movie);
            foreach (var actorId in actorList)
            {
                await InsertActorToMovie(movieId, actorId);
            }

            return movieId;
        }

        public async Task AddActorToMovieAsync(long movieId, long actorId)
        {
            await ValidateId(_movieDao, movieId);
            await ValidateId(_actorDao, actorId);

            await InsertActorToMovie(movieId, actorId);
        }

        public async Task AddActorToMovieAsync(long movieId, IEnumerable<long> actorIds)
        {
            foreach (var actorId in actorIds)
            {
                await AddActorToMovieAsync(movieId, actorId);
            }
        }

        public async Task<long> AddActorAsync(Actor actor)
        {
            await ValidateUniqueId(_actorDao, actor.Id);
            return (await _actorDao.FluentInsert(actor).ExecuteAsync()).First();
        }

        public async Task<long> AddGenreAsync(Genre genre)
        {
            await ValidateUniqueId(_genreDao, genre.Id);
            return (await _genreDao.FluentInsert(genre).ExecuteAsync()).First();
        }

        public async Task<int> DeleteMovieAsync(long movieId)
        {
            await ValidateId(_movieDao, movieId);

            await _movieActorDao.SoftDeleteByMovieIdAsync(movieId);
            return await _movieDao.FluentSoftDeleteById(movieId);
        }

        public async Task<int> DeleteActorAsync(long actorId)
        {
            await ValidateId(_actorDao, actorId);

            await _movieActorDao.SoftDeleteByActorIdAsync(actorId);
            return await _actorDao.FluentSoftDeleteById(actorId);
        }

        public async Task<int> DeleteGenreAsync(long genreId)
        {
            await ValidateId(_genreDao, genreId);
            return await _genreDao.FluentSoftDeleteById(genreId);
        }

        public async Task<int> UpdateMovieAsync(Movie movie)
        {
            await ValidateId(_movieDao, movie.Id);
            return await _movieDao.FluentUpdate(movie).ExecuteAsync();
        }

        public async Task<int> UpdateGenreAsync(Genre genre)
        {
            await ValidateId(_genreDao, genre.Id);
            return await _genreDao.FluentUpdate(genre).ExecuteAsync();
        }

        public async Task<int> UpdateActorAsync(Actor actor)
        {
            await ValidateId(_movieDao, actor.Id);
            return await _actorDao.FluentUpdate(actor).ExecuteAsync();
        }

        public async Task<Actor> GetActorByIdAsync(long actorId)
        {
            await ValidateId(_actorDao, actorId);
            return await _actorDao.SelectSingleByIdAsync(actorId);
        }

        public async Task<Genre> GetActiveGenreByIdAsync(long genreId)
        {
            await ValidateId(_genreDao, genreId);
            return await _genreDao.SelectSingleByIdAsync(genreId);
        }

        public async Task<byte[]> FetchImageByMovieId(long movieId)
        {
            await ValidateId(_movieDao, movieId);
            return await _movieDao.SelectImageByMovieId(movieId);
        }

        public async Task<int> RemoveActorFromMovieAsync(long movieId, long actorId)
        {
            await ValidateId(_movieDao, movieId);
            return await SoftDeleteActorFromMovieAsync(movieId, actorId);
        }

        public async Task<int> RemoveActorFromMovieAsync(long movieId, IEnumerable<long> actorIds)
        {
            await ValidateId(_movieDao, movieId);

            var result = 0;
            foreach (var actorId in actorIds)
            {
                result += await SoftDeleteActorFromMovieAsync(movieId, actorId);
            }
            return result;
        }

        private async Task InsertActorToMovie(long movieId, long actorId)
        {
            await ValidateId(_movieDao, movieId);
            await ValidateId(_actorDao, actorId);
            await _movieActorDao.FluentInsert(new MovieActor {MovieId = movieId, ActorId = actorId}).ExecuteAsync();
        }

        private async Task<int> SoftDeleteActorFromMovieAsync(long movieId, long actorId)
        {
            var id = await _movieActorDao.SelectByIdsAsync(movieId, actorId);
            if (id <= 0)
            {
                var invalidEntityIdException = new InvalidEntityIdException(id, $"No entry found with the movie id {movieId} and actor id {actorId}!");
                Logger.Error(invalidEntityIdException, "No {Movie} and {Actor} found", movieId, actorId);
                throw invalidEntityIdException;
            }
            return await _movieActorDao.FluentSoftDeleteById(id);
        }
    }
}