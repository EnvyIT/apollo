using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Persistence.Dao.Interfaces
{
    public interface IScheduleDao : IBaseDao<Schedule>
    {
        Task<IEnumerable<Schedule>> SelectByMovieTitleAsync(string title);
        Task<IEnumerable<Schedule>> SelectByPriceAsync(decimal price);
        Task<IEnumerable<Schedule>> SelectByStartTimeAsync(DateTime startTime);
        Task<IEnumerable<Schedule>> SelectInTimeRangeAsync(DateTime @from, DateTime to);
        Task<IEnumerable<Schedule>> SelectByTitleAndPriceAsync(string title, decimal price);
        Task<IEnumerable<Schedule>> SelectByPriceAndStartTimeAsync(decimal price, DateTime startTime);
        Task<IEnumerable<Schedule>> SelectByTitleAndStartTimeAsync(string title, DateTime startTime);
        Task<IEnumerable<Schedule>> SelectByTitlePriceAndStartTimeAsync(string title, decimal price, DateTime startTime);
    }
}
