using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class AddressDaoAdo : BaseDao<Address>, IAddressDao
    {
        public AddressDaoAdo(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<bool> IsAnyAddressReferencingCityAsync(long cityId)
        {
            return (await FluentSelectAll()
                    .WhereActive()
                    .And(_ => _.CityId)
                    .Equal(cityId)
                    .QueryAsync())
                .Any();
        }
    }
}