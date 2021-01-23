using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Repository.Interfaces
{
    public interface IRepositoryMovie : IRepository
    {
        /*
         * Select Movie
         */
        Task<IEnumerable<Movie>> GetMoviesAsync();
        Task<IEnumerable<Movie>> GetMoviesAsync(int page, int pageSize);
        Task<IEnumerable<Movie>> GetActiveMoviesByTitleAsync(string title, int page, int pageSize);
        Task<IEnumerable<Movie>> GetActiveMoviesByGenreIdAsync(IEnumerable<long> genreIds, int page, int pageSize);
        Task<IEnumerable<Genre>> GetGenresAsync();
        Task<Movie> GetActiveMovieByIdAsync(long id);
        Task<IEnumerable<Movie>> GetActiveMoviesAsync();
        Task<IEnumerable<Movie>> GetActiveMoviesByTitleAsync(string title);
        Task<IEnumerable<Movie>> GetActiveMoviesByGenreIdAsync(long genreId);
        Task<IEnumerable<Movie>> GetActiveMoviesByGenreIdAsync(IEnumerable<long> genreIds);
        Task<IEnumerable<Movie>> GetMoviesByActorIdAsync(long actorId);
        /*
         * Select Actor
         */
        Task<IEnumerable<Actor>> GetActiveActorsAsync();
        Task<IEnumerable<Actor>> GetActorsByMovieIdAsync(long movieId);

        /*
         * Select Genre
         */
        Task<IEnumerable<Genre>> GetActiveGenresAsync();

        /*
         * Add
         */
        Task<long> AddMovieAsync(Movie movie);
        Task<long> AddMovieAsync(Movie movie, IEnumerable<long> actorIds);
        Task AddActorToMovieAsync(long movieId, long actorId);
        Task AddActorToMovieAsync(long movieId, IEnumerable<long> actorIds);
        Task<long> AddActorAsync(Actor actor);
        Task<long> AddGenreAsync(Genre genre);

        /*
         * Delete
         */
        Task<int> DeleteMovieAsync(long movieId);
        Task<int> RemoveActorFromMovieAsync(long movieId, long actorId);
        Task<int> RemoveActorFromMovieAsync(long movieId, IEnumerable<long> actorIds);
        Task<int> DeleteActorAsync(long actorId);
        Task<int> DeleteGenreAsync(long genreId);

        /*
         * Update
         */
        Task<int> UpdateMovieAsync(Movie movie);
        Task<int> UpdateGenreAsync(Genre genre);
        Task<int> UpdateActorAsync(Actor actor);
        Task<Actor> GetActorByIdAsync(long actorId);
        Task<Genre> GetActiveGenreByIdAsync(long genreId);

        /*
         * Image
         */

        Task<byte[]> FetchImageByMovieId(long movieId);
    }
}