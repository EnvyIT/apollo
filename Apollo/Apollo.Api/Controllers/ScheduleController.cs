using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Api.Authorization;
using Apollo.Api.Controllers.Base;
using Apollo.Core.Dto;
using Apollo.Core.Interfaces;
using Apollo.Util.Logger;
using Microsoft.AspNetCore.Mvc;
using LoggerFactory = Apollo.Util.Logger.LoggerFactory;

namespace Apollo.Api.Controllers
{
    public class ScheduleController : ApiControllerBase
    {
        private static readonly IApolloLogger<ScheduleController> Logger = LoggerFactory.CreateLogger<ScheduleController>();
        private readonly IServiceFactory _service;

        public ScheduleController(IServiceFactory service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Get all schedules from a given day which are not already started.
        /// </summary>
        /// <param name="day">Selected day (Only year, month and day are used).</param>
        /// <returns>List with schedules.</returns>
        /// <response code="200">List with schedules.</response>
        [HttpGet("day/{day}")]
        public async Task<ActionResult<IList<ScheduleDto>>> GetSchedules(DateTime day)
        {
            Logger.Here().Info("{GetSchedules} from {day}", nameof(GetSchedules), day.ToShortDateString());
            return (await GetSchedulesByDay(day)).ToList();
        }

        /// <summary>
        /// Get all schedules from a given day which are not already started and match the given genre.
        /// </summary>
        /// <param name="day">Selected day (Only year, month and day are used).</param>
        /// <param name="genre">Selected genre.</param>
        /// <returns>List with schedules.</returns>
        /// <response code="200">List with schedules.</response>
        [HttpGet("{day}/genre/{genre}")]
        public async Task<ActionResult<IList<ScheduleDto>>> GetSchedulesByGenre(DateTime day, string genre)
        {
            Logger.Here().Info("{GetSchedulesByGenre} from {day} and {genre}", nameof(GetSchedulesByGenre), day.ToShortDateString(), genre);
            return (await GetSchedulesByDay(day))
                .Where(schedule => schedule.Movie.Genre.Name.ToLower().Contains(genre.ToLower()))
                .ToList();
        }

        /// <summary>
        /// Get all schedules from a given day which are not already started and match the given title.
        /// </summary>
        /// <param name="day">Selected day (Only year, month and day are used).</param>
        /// <param name="title">Selected title.</param>
        /// <returns>List with schedules.</returns>
        /// <response code="200">List with schedules.</response>
        [HttpGet("{day}/title/{title}")]
        public async Task<ActionResult<IList<ScheduleDto>>> GetSchedulesByTitle(DateTime day, string title)
        {
            Logger.Here().Info("{GetSchedulesByTitle} from {day} and {title}", nameof(GetSchedulesByTitle), day.ToShortDateString(), title);
            return (await GetSchedulesByDay(day))
                .Where(schedule => schedule.Movie.Title.ToLower().Contains(title.ToLower()))
                .ToList();
        }

        /// <summary>
        /// Get a schedule from a given id.
        /// </summary>
        /// <param name="id">Selected day (Only year, month and day are used).</param>
        /// <returns>List with schedules.</returns>
        /// <response code="200">Schedule with the id.</response>
        /// <response code="400">Schedule not exist.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduleDto>> GetSchedule(long id)
        {
            Logger.Here().Info("{GetSchedule} with {id}", nameof(GetSchedule), id);
            var result = await _service.CreateScheduleService().GetScheduleByIdAsync(id);
            if (result != null)
            {
                return result;
            }

            return BadRequestResponse("Invalid id given!");
        }

        /// <summary>
        /// Add a new schedule.
        /// </summary>
        /// <param name="schedule">New schedule data.</param>
        /// <returns>The new ID if the given schedule is valid.</returns>
        /// <response code="200">The created ID for this schedule.</response>
        [HttpPost]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> AddSchedule(ScheduleDto schedule)
        {
            Logger.Here().Info("{AddSchedule} - {schedule}", nameof(AddSchedule), schedule);
            var result = await _service.CreateScheduleService().AddScheduleAsync(schedule);
            return GetCreatedStatus(result, nameof(GetSchedule), "Given schedule is not addable.");
        }

        /// <summary>
        /// Deletes the schedule with the given id.
        /// </summary>
        /// <param name="id">ID from the schedule.</param>
        /// <returns>Successful state.</returns>
        /// <response code="200">If the schedule was successfully deleted.</response>
        /// <response code="400">If the schedule is not deletable.</response>
        [HttpDelete("{id}")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> DeleteSchedule(long id)
        {
            Logger.Here().Info("{DeleteSchedule()} - {id}", nameof(DeleteSchedule), id);
            var scheduleService = _service.CreateScheduleService();
            if (await ScheduleContainsReservations(id))
            {
                Logger.Here().Info("{DeleteSchedule} - {id} - contains reservations", nameof(DeleteSchedule), id);
                return BadRequestResponse("Schedule contains reservations!");
            }

            var schedule = await scheduleService.GetScheduleByIdAsync(id);
            var result = await scheduleService.DeleteScheduleAsync(schedule);
            return GetDeletionStatus(result, "Schedule is not deletable!");
        }

        /// <summary>
        /// Updates the given schedule.
        /// </summary>
        /// <param name="schedule">Updated schedule data.</param>
        /// <returns>Successful if the update was successful.</returns>
        /// <response code="200">If the schedule was successfully updated.</response>
        /// <response code="400">If the schedule is not updateable.</response>
        [HttpPut]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> UpdateSchedule(ScheduleDto schedule)
        {
            Logger.Here().Info("{UpdateSchedule()} - {schedule}", nameof(UpdateSchedule), schedule);
            if (await ScheduleContainsReservations(schedule.Id))
            {
                Logger.Here().Info("{UpdateSchedule()} - {schedule} - contains reservations", nameof(UpdateSchedule), schedule);
                return BadRequestResponse("Schedule contains reservations!");
            }

            if (await _service.CreateScheduleService().UpdateScheduleAsync(schedule))
            {
                return UpdatedResponse();
            }

            Logger.Here().Info("{UpdateSchedule} - {schedule} - not deletable", nameof(UpdateSchedule), schedule);
            return BadRequestResponse("Schedule is not updateable!");
        }

        /// <summary>
        /// Get the load factor for a given schedule id.
        /// </summary>
        /// <param name="id">Schedule id.</param>
        /// <returns>Load factor if the schedule exist.</returns>
        /// <response code="200">Load factor as percentage.</response>
        /// <response code="400">Invalid id given.</response>
        [HttpGet("loadfactor/{id}")]
        public async Task<ActionResult<double>> GetLoadFactor(long id)
        {
            Logger.Here().Info("{GetLoadFactor} - {id}", nameof(GetLoadFactor), id);
            return await _service.CreateScheduleService().GetCapacityAsync(id);
        }

        /// <summary>
        /// Gets the seat layout for a given schedule id.
        /// </summary>
        /// <param name="id">Schedule id.</param>
        /// <returns>Seat layout.</returns>
        /// <response code="200">If the given schedule id was valid.</response>
        /// <response code="400">If the given schedule id was invalid.</response>
        [HttpGet("seatlayout/{id}")]
        public async Task<ActionResult<IList<SeatDto>>> GetSeatLayout(long id)
        {
            var infrastructureService = _service.CreateInfrastructureService();
            Logger.Here().Info(nameof(GetSeatLayout));

            var schedule = await _service.CreateScheduleService().GetScheduleByIdAsync(id);
            if (schedule == null)
            {
                Logger.Here().Error("{GetSeatLayout} - Layout for schedule with {id} does not exist", nameof(GetSeatLayout), id);
                return BadRequestResponse("Given schedule id is invalid.");
            }

            return (await infrastructureService.GetLayout(schedule))
                .Cast<SeatDto>()
                .Where(s => s != null)
                .Select(s => new SeatDto
                {
                    Id = s.Id,
                    LayoutColumn = s.LayoutColumn,
                    LayoutRow = s.LayoutRow,
                    Number = s.Number,
                    RowId = s.RowId,
                    State = s.State
                })
                .ToList();
        }

        private async Task<bool> ScheduleContainsReservations(long id)
        {
            var reservations = await _service.CreateReservationService().GetReservationsByScheduleIdAsync(id);
            return reservations.Any();
        }

        private async Task<IEnumerable<ScheduleDto>> GetSchedulesByDay(DateTime day)
        {
            return await _service.CreateScheduleService().GetInTimeRangeAsync(
                new DateTime(day.Year, day.Month, day.Day, 0, 0, 0),
                new DateTime(day.Year, day.Month, day.Day, 23, 59, 59));
        }
    }
}