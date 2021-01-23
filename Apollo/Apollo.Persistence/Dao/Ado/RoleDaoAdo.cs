using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class RoleDaoAdo: BaseDao<Role>, IRoleDao {
        public RoleDaoAdo(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<(bool Exist, long Id)> ExistAsync(string label)
        {
            var queryResult = await FluentSelect()
                .Column(_ => _.Id)
                .WhereActive()
                .And(_ => _.Label)
                .Equal(label)
                .QueryAsync();

            return CreateIdExistTuple(queryResult.FirstOrDefault());
        }
    }
}
