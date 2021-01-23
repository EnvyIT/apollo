using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Persistence.Dao.Interfaces
{
    public interface IMovieDao : IBaseDao<Movie>
    {
        Task<IEnumerable<Movie>> SelectWithReferencesAsync();
        Task<IEnumerable<Movie>> SelectActiveWithGenreAsync();
        Task<Movie> SelectWithReferencesByIdAsync(long id);

        Task<IEnumerable<Movie>> SelectByTitleAsync(string title);

        Task<IEnumerable<Movie>> SelectByGenreIdAsync(long genreId);

        Task<IEnumerable<Movie>> SelectByGenreIdAsync(IEnumerable<long> genreIds);
        Task<IEnumerable<Movie>> SelectPagedAsync(int page, int pageSize);
        Task<IEnumerable<Movie>> SelectByTitlePagedAsync(string title, int page, int pageSize);
        Task<IEnumerable<Movie>> SelectByGenreIdPagedAsync(IEnumerable<long> genreIds, int page, int pageSize);
        Task<byte[]> SelectImageByMovieId(long id);
    }
}
