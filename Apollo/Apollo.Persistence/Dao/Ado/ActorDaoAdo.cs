using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class ActorDaoAdo : BaseDao<Actor>, IActorDao
    {
        public ActorDaoAdo(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<long?> FindByNameAsync(string firstName, string lastName)
        {
            return (await FluentSelect()
                .Column(_ => _.Id)
                .Where(_ => _.FirstName)
                .Equal(firstName)
                .And(_ => _.LastName)
                .Equal(lastName)
                .QuerySingleAsync())?.Id;
        }
    }
}