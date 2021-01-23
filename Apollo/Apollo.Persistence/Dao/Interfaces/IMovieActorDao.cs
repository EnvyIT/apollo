using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Persistence.Dao.Interfaces
{
    public interface IMovieActorDao : IBaseDao<MovieActor>
    {
        Task<IEnumerable<Actor>> SelectActorsByMovieIdAsync(long movieId);
        Task<long> SelectByIdsAsync(long movieId, long actorId);
        Task<int> SoftDeleteByMovieIdAsync(long movieId);
        Task<int> SoftDeleteByActorIdAsync(long actorId);
        Task<IEnumerable<Movie>> SelectMoviesByActorId(long actorId);
    }
}
