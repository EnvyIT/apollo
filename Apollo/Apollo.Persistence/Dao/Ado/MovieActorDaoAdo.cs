using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Select;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class MovieActorDaoAdo : BaseDao<MovieActor>, IMovieActorDao
    {
        public MovieActorDaoAdo(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<IEnumerable<Actor>> SelectActorsByMovieIdAsync(long movieId)
        {
            return (await FluentSelectAll()
                .InnerJoin<MovieActor, Actor, long, long>(_ => _.Actor, _ => _.ActorId, _ => _.Id)
                .WhereActive()
                .And(_ => _.MovieId)
                .Equal(movieId)
                .QueryAsync()).Select(r => r.Actor);
        }

        public async Task<long> SelectByIdsAsync(long movieId, long actorId)
        {
            return (await FluentSelect()
                .Column(_ => _.Id)
                .Where(_ => _.MovieId).Equal(movieId)
                .And(_ => _.ActorId).Equal(actorId)
                .QuerySingleAsync())?.Id ?? -1;
        }

        public Task<int> SoftDeleteByMovieIdAsync(long movieId)
        {
            var statement = FluentSelect()
                .Column(_ => _.Id)
                .Where(_ => _.MovieId).Equal(movieId);
            return SoftDelete(statement);
        }

        public Task<int> SoftDeleteByActorIdAsync(long actorId)
        {
            var statement = FluentSelect()
                .Column(_ => _.Id)
                .Where(_ => _.ActorId).Equal(actorId);
            return SoftDelete(statement);
        }

        public async Task<IEnumerable<Movie>> SelectMoviesByActorId(long actorId)
        {
            return (await FluentSelectAll()
                .InnerJoin<MovieActor, Movie, long, long>(_ => _.Movie, _ => _.MovieId, _ => _.Id)
                .WhereActive()
                .And(_ => _.ActorId)
                .Equal(actorId)
                .QueryAsync()).Select(r => r.Movie);
        }

        private async Task<int> SoftDelete(IFluentEntityCall<MovieActor> statement)
        {
            var entities = await statement.QueryAsync();

            var result = 0;
            foreach (var entity in entities)
            {
                result += await FluentSoftDeleteById(entity.Id);
            }

            return result;
        }
    }
}