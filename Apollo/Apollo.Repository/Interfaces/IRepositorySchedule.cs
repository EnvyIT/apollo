using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Repository.Interfaces
{
    public interface IRepositorySchedule : IRepository
    {
        decimal MinPriceWeekday { get; set; }
        decimal MaxPriceWeekday { get; set; }
        decimal MinPriceWeekend { get; set; }
        decimal MaxPriceWeekend { get; set; }
        int BufferTime { get; set; }

        Task AddSchedulesForMoviesAsync(DateTime from, DateTime to, IEnumerable<Movie> movies);

        Task<long> AddScheduleAsync(Schedule newSchedule);
        Task<Schedule> GetScheduleByIdAsync(long id);
        Task<bool> IdExistAsync(long id);
        Task<IEnumerable<Schedule>> GetSchedulesAsync();
        Task<int> DeleteScheduleAsync(Schedule newSchedule);
        Task<bool> UpdateScheduleAsync(Schedule schedule);
        Task<IEnumerable<long>> AddSchedulesAsync(IEnumerable<Schedule> newSchedules);

        Task<IEnumerable<Schedule>> GetByMovieTitleAsync(string title);
        Task<IEnumerable<Schedule>> GetByPriceAsync(decimal price);
        Task<IEnumerable<Schedule>> GetStartTimeAsync(DateTime startTime);
        Task<IEnumerable<Schedule>> GetInTimeRangeAsync(DateTime from , DateTime to);
        Task<IEnumerable<Schedule>> GetByTitleAndPriceAsync(string title, decimal price);
        Task<IEnumerable<Schedule>> GetByPriceAndStartTimeAsync(decimal price, DateTime startTime);
        Task<IEnumerable<Schedule>> GetByTitleAndStartTimeAsync(string title, DateTime startTime);
        Task<IEnumerable<Schedule>> GetByTitlePriceAndStartTimeAsync(string title, decimal price, DateTime startTime);
    }
}
