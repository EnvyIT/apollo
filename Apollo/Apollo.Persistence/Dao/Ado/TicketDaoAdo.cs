using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class TicketDaoAdo : BaseDao<Ticket>, ITicketDao
    {
        public TicketDaoAdo(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

    }
}
