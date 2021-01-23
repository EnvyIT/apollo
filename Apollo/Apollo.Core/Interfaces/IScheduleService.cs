using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Core.Dto;

namespace Apollo.Core.Interfaces
{
    public interface IScheduleService
    {
        Task<ScheduleDto> AddScheduleAsync(ScheduleDto newSchedule);
        Task<ScheduleDto> GetScheduleByIdAsync(long id);
        Task<IEnumerable<ScheduleDto>> GetSchedulesAsync();
        Task<bool> DeleteScheduleAsync(ScheduleDto schedule);
        Task<bool> UpdateScheduleAsync(ScheduleDto schedule);
        Task<IEnumerable<ScheduleDto>> AddSchedulesAsync(IEnumerable<ScheduleDto> newSchedules);

        Task<IEnumerable<ScheduleDto>> GetByMovieTitleAsync(string title);
        Task<IEnumerable<ScheduleDto>> GetByPriceAsync(decimal price);
        Task<IEnumerable<ScheduleDto>> GetStartTimeAsync(DateTime startTime);
        Task<IEnumerable<ScheduleDto>> GetInTimeRangeAsync(DateTime from, DateTime to);
        Task<IEnumerable<ScheduleDto>> GetByTitleAndPriceAsync(string title, decimal price);
        Task<IEnumerable<ScheduleDto>> GetByPriceAndStartTimeAsync(decimal price, DateTime startTime);
        Task<IEnumerable<ScheduleDto>> GetByTitleAndStartTimeAsync(string title, DateTime startTime);
        Task<IEnumerable<ScheduleDto>> GetByTitlePriceAndStartTimeAsync(string title, decimal price, DateTime startTime);

        Task<double> GetCapacityAsync(long scheduleId);

    }
}
