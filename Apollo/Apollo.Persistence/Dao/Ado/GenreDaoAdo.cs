using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class GenreDaoAdo : BaseDao<Genre>, IGenreDao {
        public GenreDaoAdo(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }
    }
}
