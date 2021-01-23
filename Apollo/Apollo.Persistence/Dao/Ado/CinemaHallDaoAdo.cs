using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class CinemaHallDaoAdo : BaseDao<CinemaHall>, ICinemaHallDao {
        public CinemaHallDaoAdo(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }
    }
}
