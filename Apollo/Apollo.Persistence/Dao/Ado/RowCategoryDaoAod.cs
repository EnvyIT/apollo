using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class RowCategoryDaoAod : BaseDao<RowCategory>, IRowCategoryDao {
        public RowCategoryDaoAod(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }
    }
}
