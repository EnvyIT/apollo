using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class CityDaoAdo : BaseDao<City>, ICityDao
    {
        public CityDaoAdo(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<(bool Exist, long Id)> ExistAsync(string postalCode, string name)
        {
            var queryResult = await FluentSelect()
                .Column(_ => _.Id)
                .WhereActive()
                .And(_ => _.PostalCode)
                .Equal(postalCode)
                .And(_ => _.Name)
                .Equal(name)
                .QueryAsync();

            return CreateIdExistTuple(queryResult.FirstOrDefault());
        }
    }
}