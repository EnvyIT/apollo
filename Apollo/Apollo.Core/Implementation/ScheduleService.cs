using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Core.Interfaces;
using Apollo.UnitOfWork.Interfaces;
using Apollo.Util.Logger;
using static Apollo.Core.Dto.Mapper;
using static Apollo.Util.ValidationHelper;

namespace Apollo.Core.Implementation
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private static readonly  IApolloLogger<ScheduleService> Logger = LoggerFactory.CreateLogger<ScheduleService>();

        public ScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ScheduleDto> AddScheduleAsync(ScheduleDto newSchedule)
        {
            ValidateNull( Logger.Here(), newSchedule);
            var id = await _unitOfWork.RepositorySchedule.AddScheduleAsync(Map(newSchedule));
            if (id <= 0L)
            {
                return null;
            }

            newSchedule.Id = id;
            return newSchedule;
        }

        public async Task<ScheduleDto> GetScheduleByIdAsync(long id)
        {
            if (!await _unitOfWork.RepositorySchedule.IdExistAsync(id))
            {
                return null;
            }
            var scheduleDto = Map(await _unitOfWork.RepositorySchedule.GetScheduleByIdAsync(id));
            scheduleDto.LoadFactor = await GetCapacityAsync(id);
            return scheduleDto;
        }

        public async Task<IEnumerable<ScheduleDto>> GetSchedulesAsync()
        {
            var schedules = (await _unitOfWork.RepositorySchedule.GetSchedulesAsync())
                .Select(Map).ToList();
            return SetLoadFactor(schedules);
        }

        private IEnumerable<ScheduleDto> SetLoadFactor(IEnumerable<ScheduleDto> schedules)
        {
            var scheduleList = schedules.ToList();
            scheduleList.ForEach(async s => s.LoadFactor = await GetCapacityAsync(s.Id));
            return scheduleList;
        }

        public async Task<bool> DeleteScheduleAsync(ScheduleDto schedule)
        {
            ValidateNull( Logger.Here(), schedule);
            if (!await _unitOfWork.RepositorySchedule.IdExistAsync(schedule.Id))
            {
                return false;
            }
            return await _unitOfWork.RepositorySchedule.DeleteScheduleAsync(Map(schedule)) > 0;
        }

        public async Task<bool> UpdateScheduleAsync(ScheduleDto schedule)
        {
            ValidateNull(Logger.Here(), schedule);
            if (!await _unitOfWork.RepositorySchedule.IdExistAsync(schedule.Id))
            {
                return false;
            }
            return await _unitOfWork.RepositorySchedule.UpdateScheduleAsync(Map(schedule));
        }

        public async Task<IEnumerable<ScheduleDto>> AddSchedulesAsync(IEnumerable<ScheduleDto> newSchedules)
        {
            ValidateNull( Logger.Here(), newSchedules);
            var added = await _unitOfWork.RepositorySchedule.AddSchedulesAsync(newSchedules.Select(Map));
            return added.Any() ? newSchedules : new List<ScheduleDto>();
        }

        public async Task<IEnumerable<ScheduleDto>> GetByMovieTitleAsync(string title)
        {
            ValidateNull( Logger.Here(), title);
            var schedules =  (await _unitOfWork.RepositorySchedule.GetByMovieTitleAsync(title)).Select(Map);
            return SetLoadFactor(schedules);
        }

        public async Task<IEnumerable<ScheduleDto>> GetByPriceAsync(decimal price)
        {
            var schedules = (await _unitOfWork.RepositorySchedule.GetByPriceAsync(price)).Select(Map);
            return SetLoadFactor(schedules);
        }

        public async Task<IEnumerable<ScheduleDto>> GetStartTimeAsync(DateTime startTime)
        {
            ValidateNull( Logger.Here(), startTime);
            var schedules =  (await _unitOfWork.RepositorySchedule.GetStartTimeAsync(startTime)).Select(Map);
            return SetLoadFactor(schedules);
        }

        public async Task<IEnumerable<ScheduleDto>> GetInTimeRangeAsync(DateTime @from, DateTime to)
        {
            ValidateNull( Logger.Here(), @from);
            ValidateNull( Logger.Here(), to);
            var schedules = (await _unitOfWork.RepositorySchedule.GetInTimeRangeAsync(@from, to)).Select(Map);
            return SetLoadFactor(schedules);
        }

        public async Task<IEnumerable<ScheduleDto>> GetByTitleAndPriceAsync(string title, decimal price)
        {
            ValidateNull(Logger.Here(), title);
            var schedules = (await _unitOfWork.RepositorySchedule.GetByTitleAndPriceAsync(title, price)).Select(Map);
            return SetLoadFactor(schedules);
        }

        public async Task<IEnumerable<ScheduleDto>> GetByPriceAndStartTimeAsync(decimal price, DateTime startTime)
        {
            ValidateNull( Logger.Here(), startTime);
            var schedules = (await _unitOfWork.RepositorySchedule.GetByPriceAndStartTimeAsync(price, startTime)).Select(Map);
            return SetLoadFactor(schedules);
        }

        public async Task<IEnumerable<ScheduleDto>> GetByTitleAndStartTimeAsync(string title, DateTime startTime)
        {
            ValidateNull( Logger.Here(), startTime);
            var schedules = (await _unitOfWork.RepositorySchedule.GetByTitleAndStartTimeAsync(title, startTime)).Select(Map); 
            return SetLoadFactor(schedules);
        }

        public async Task<IEnumerable<ScheduleDto>> GetByTitlePriceAndStartTimeAsync(string title, decimal price, DateTime startTime)
        {
            ValidateNull( Logger.Here(), title);
            ValidateNull( Logger.Here(), startTime);
            var schedules = (await _unitOfWork.RepositorySchedule.GetByTitlePriceAndStartTimeAsync(title, price, startTime)).Select(Map);
            return SetLoadFactor(schedules);
        }

        public async Task<double> GetCapacityAsync(long scheduleId)
        {
            if (!await _unitOfWork.RepositorySchedule.IdExistAsync(scheduleId))
            {
                throw new ArgumentException("Invalid id");
            }

            var reservationIds = (await _unitOfWork.RepositoryTicket.GetReservationsAsync())
                .Where(reservation => reservation.ScheduleId == scheduleId)
                .Select(reservation => reservation.Id)
                .ToList();
            var reservationSeatCount = reservationIds.Any() ? 
                (await _unitOfWork.RepositoryTicket.GetSeatReservationsByIds(reservationIds)).Count() :
                0;
            var schedule = await _unitOfWork.RepositorySchedule.GetScheduleByIdAsync(scheduleId);
            var seatCount =
                (await _unitOfWork.RepositoryInfrastructure.GetSeatsWithRowAndCategoryAsync(schedule.CinemaHallId))
                .Count(s => s != null);
            return seatCount == 0 ? 0.0 : reservationSeatCount / (double)seatCount;
        }

       

    }
}
