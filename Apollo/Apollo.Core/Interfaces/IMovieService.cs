using Apollo.Core.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apollo.Core.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieDto>> GetMoviesAsync();
        Task<IEnumerable<MovieDto>> GetMoviesAsync(int page, int pageSize);
        Task<IEnumerable<MovieDto>> GetActiveMoviesByGenreIdAsync(GenreDto genreDto);
        Task<IEnumerable<MovieDto>> GetActiveMoviesByGenreIdAsync(long genreId);
        Task<IEnumerable<MovieDto>> GetActiveMoviesByTitleAsync(string title);
        Task<IEnumerable<MovieDto>> GetActiveMoviesByTitleAsync(string title, int page, int pageSize);
        Task<IEnumerable<MovieDto>> GetActiveMoviesByGenreIdAsync(IEnumerable<long> genreIds, int page, int pageSize);
        Task<IEnumerable<MovieDto>> GetMoviesByActorIdAsync(long actorId);
        Task<MovieDto> GetActiveMovieByIdAsync(long id);
        Task<MovieDto> UpdateMovieAsync(MovieDto movieDto);
        Task<MovieDto> AddMovieAsync(MovieDto movieDto);
        Task<bool> DeleteMovieAsync(MovieDto movieDto);
        Task<bool> DeleteMovieAsync(long movieId);

        Task<MovieDto> AddMovieAsync(MovieDto movie, IEnumerable<long> actorIds);
        Task AddActorToMovieAsync(long movieId, long actorId);
        Task AddActorToMovieAsync(long movieId, IEnumerable<long> actorIds);
        Task<bool> RemoveActorFromMovieAsync(long movieId, long actorId);
        Task<bool> RemoveActorFromMovieAsync(long movieId, IEnumerable<long> actorIds);
     
  

        Task<IEnumerable<ActorDto>> GetActiveActorsAsync(int page, int pageSize);
        Task<IEnumerable<ActorDto>> GetActorsByMovieIdAsync(long movieId);
        Task<bool> DeleteActorAsync(long actorId);
        Task<ActorDto> AddActorAsync(ActorDto actor);
        Task<ActorDto> UpdateActorAsync(ActorDto actorDto);


        Task<IEnumerable<GenreDto>> GetActiveGenresAsync();
        Task<bool> DeleteGenreAsync(long genreId);
        Task<GenreDto> AddGenreAsync(GenreDto genreDto);
        Task<GenreDto> UpdateGenreAsync(GenreDto genreDto);
        Task<byte[]> FetchImageByMovieId(long movieId);
    }
}