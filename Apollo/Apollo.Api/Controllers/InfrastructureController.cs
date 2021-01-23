using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Api.Authorization;
using Apollo.Api.Controllers.Base;
using Apollo.Core.Dto;
using Apollo.Core.Exception;
using Apollo.Core.Interfaces;
using Apollo.Util.Logger;
using Microsoft.AspNetCore.Mvc;
using LoggerFactory = Apollo.Util.Logger.LoggerFactory;


namespace Apollo.Api.Controllers
{
    public class InfrastructureController : ApiControllerBase
    {
        private static readonly IApolloLogger<InfrastructureController> Logger = LoggerFactory.CreateLogger<InfrastructureController>();
        private readonly IServiceFactory _service;

        public InfrastructureController(IServiceFactory service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Get all active cinema halls.
        /// </summary>
        /// <returns>List with cinema halls.</returns>
        /// <response code="200">Returns all active cinema halls.</response>
        [HttpGet("cinemahalls")]
        public async Task<ActionResult<IList<CinemaHallDto>>> GetCinemaHalls()
        {
            Logger.Here().Info(nameof(GetCinemaHalls));
            return (await _service.CreateInfrastructureService().GetActiveCinemaHallsAsync()).ToList();
        }

        /// <summary>
        /// Get single cinema hall by id.
        /// </summary>
        /// <param name="id">Id of the cinema hall.</param>
        /// <returns>Cinema hall details.</returns>
        /// <response code="200">Cinema hall details if the given id exist.</response>
        /// <response code="400">If the given id not exist.</response>
        [HttpGet("cinemahall/{id}")]
        public async Task<ActionResult<CinemaHallDto>> GetCinemaHall(long id)
        {
            Logger.Here().Info(nameof(GetCinemaHall));
            try
            {
                return await _service.CreateInfrastructureService().GetCinemaHallByIdAsync(id);
            }
            catch (PersistenceException e)
            {
                Logger.Here().Error(e, "{CinemaHall} - not exist", nameof(GetCinemaHall));
                return BadRequestResponse("Given id is invalid");
            }
        }

        /// <summary>
        /// Delete cinema hall with the given id.
        /// </summary>
        /// <param name="id">Id of the cinema hall.</param>
        /// <returns>Status code.</returns>
        /// <response code="200">If the cinema hall was deleted.</response>
        /// <response code="400">If the cinema hall is not deletable.</response>
        [HttpDelete("cinemahall/{id}")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> DeleteCinemaHall(long id)
        {
            var infrastructureService = _service.CreateInfrastructureService();

            Logger.Here().Info(nameof(DeleteCinemaHall));
            if (!await infrastructureService.CinemaHallExistAsync(id))
            {
                Logger.Here().Error("{CinemaHall} - not exist", nameof(DeleteCinemaHall));
                return BadRequestResponse("Given id is invalid");
            }

            if (await _service.CreateReservationService().GetCountReservationsByCinemaHallAsync(id) > 0)
            {
                Logger.Here().Error("{CinemaHall} - contains reservations", nameof(DeleteCinemaHall));
                return BadRequestResponse("Given cinema hall contains reservations.");
            }

            var result = await infrastructureService.DeleteCinemaHallAsync(id);
            return GetDeletionStatus(result, "Given cinema hall id is not deletable.");
        }

        /// <summary>
        /// Update cinema hall with the given data.
        /// </summary>
        /// <param name="cinemaHall">Updated cinema hall.</param>
        /// <returns>Status code.</returns>
        /// <response code="200">If the cinema hall was updated.</response>
        /// <response code="400">If the cinema hall is not updateable.</response>
        [HttpPut("cinemahall")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> UpdateCinemaHall(CinemaHallDto cinemaHall)
        {
            var infrastructureService = _service.CreateInfrastructureService();

            Logger.Here().Info(nameof(UpdateCinemaHall));
            if (!await infrastructureService.CinemaHallExistAsync(cinemaHall.Id))
            {
                Logger.Here().Error("{CinemaHall} - does not exist", nameof(UpdateCinemaHall));
                return BadRequestResponse("Given cinema hall does not exist");
            }

            if (await _service.CreateReservationService().GetCountReservationsByCinemaHallAsync(cinemaHall.Id) > 0)
            {
                Logger.Here().Info("{CinemaHall} - contains reservations", nameof(DeleteCinemaHall));
                return BadRequestResponse("Given cinema hall contains reservations.");
            }

            var result = await infrastructureService.UpdateCinemaHallAsync(cinemaHall);
            return GetUpdatedStatus(result, "Given cinema hall is not updateable.");
        }

        /// <summary>
        /// Add cinema hall with the given data.
        /// </summary>
        /// <param name="cinemaHall">New cinema hall.</param>
        /// <returns>Created id.</returns>
        /// <response code="200">If the cinema hall was added.</response>
        /// <response code="400">If the cinema hall is not addable.</response>
        [HttpPost("cinemahall")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> AddCinemaHall(CinemaHallDto cinemaHall)
        {
            var infrastructureService = _service.CreateInfrastructureService();

            Logger.Here().Info(nameof(AddCinemaHall));

            var result = await infrastructureService.AddCinemaHallAsync(cinemaHall);
            return GetCreatedStatus(result, nameof(GetCinemaHall), "Given cinema hall is not addable.");
        }

        /// <summary>
        /// Get the seat layout from a given cinema hall.
        /// </summary>
        /// <param name="id">Cinema hall id.</param>
        /// <returns>Seat layout of the cinema hall.</returns>
        /// <response code="200">Seat layout if the given cinema hall id exist.</response>
        /// <response code="400">If the given cinema hall id not exist.</response>
        [HttpGet("seatlayout/{id}")]
        public async Task<ActionResult<IList<SeatDto>>> GetSeatLayout(long id)
        {
            var infrastructureService = _service.CreateInfrastructureService();
            Logger.Here().Info(nameof(GetCinemaHall));

            if (!await infrastructureService.CinemaHallExistAsync(id))
            {
                Logger.Here().Error("{CinemaHall} - does not exist", nameof(GetCinemaHall));
                return BadRequestResponse("Given id is invalid.");
            }

            var s = (await infrastructureService.GetLayout(id));
            return (await infrastructureService.GetLayout(id))
                .Cast<SeatDto>()
                .Where(s => s != null)
                .Select(s => new SeatDto
                {
                    Id = s.Id, RowVersion = s.RowVersion,
                    LayoutColumn = s.LayoutColumn, LayoutRow = s.LayoutRow, 
                    Number = s.Number, RowId = s.RowId, State = s.State
                })
                .ToList();
        }

        /// <summary>
        /// Lock a given seat.
        /// </summary>
        /// <param name="id">Id of the seat.</param>
        /// <returns>Status code.</returns>
        /// <response code="200">If the seat was locked.</response>
        /// <response code="400">If the seat is not lockable.</response>
        [HttpPut("lockseat/{id}")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<ActionResult> LockSeat(long id)
        {
            return await UpdateSeatLockedAsync(nameof(LockSeat), id, true);
        }

        /// <summary>
        /// Unlock a given seat.
        /// </summary>
        /// <param name="id">Id of the seat.</param>
        /// <returns>Status code.</returns>
        /// <response code="200">If the seat was unlocked.</response>
        /// <response code="400">If the seat is not unlock able.</response>
        [HttpPut("unlockseat/{id}")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<ActionResult> UnlockSeat(long id)
        {
            return await UpdateSeatLockedAsync(nameof(LockSeat), id, false);
        }

        /// <summary>
        /// Get rows from a given cinema hall id.
        /// </summary>
        /// <param name="id">Cinema hall id.</param>
        /// <returns>Rows from cinema hall.</returns>
        /// <response code="200">If the cinema hall id is valid.</response>
        /// <response code="400">If the cinema hall id is invalid.</response>
        [HttpGet("rows/{id}")]
        public async Task<ActionResult<IList<RowDto>>> GetRowsFromCinemaHall(long id)
        {
            var infrastructureService = _service.CreateInfrastructureService();
            Logger.Here().Info(nameof(GetRowsFromCinemaHall));

            if (!(await infrastructureService.CinemaHallExistAsync(id)))
            {
                Logger.Here().Error("{CinemaHall} - does not exist", nameof(GetRowsFromCinemaHall));
                return BadRequestResponse("Given cinema hall id is invalid.");
            }

            return (await infrastructureService.GetActiveRowsFromCinemaHallAsync(id)).ToList();
        }

        /// <summary>
        /// Adds a given row.
        /// </summary>
        /// <param name="row">New row.</param>
        /// <returns>Created id.</returns>
        /// <response code="200">If the row was added.</response>
        /// <response code="400">If the row was not added.</response>
        [HttpPost("row")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> AddRowAsync(RowDto row)
        {
            Logger.Here().Info("{AddAsyncRow} - {rowId}", nameof(AddRowAsync), row.Id);
            var result = await _service.CreateInfrastructureService().AddRowAsync(row);
            return GetCreatedStatus(result, nameof(GetRowAsync), "Given row is not addable.");
        }

        /// <summary>
        /// Deletes row with the given id.
        /// </summary>
        /// <param name="id">Id of the row.</param>
        /// <returns>Status code.</returns>
        /// <response code="200">If the row was deleted.</response>
        /// <response code="400">If the row was not deletable.</response>
        [HttpDelete("row/{id}")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> DeleteRow(long id)
        {
            var infrastructureService = _service.CreateInfrastructureService();

            Logger.Here().Info(nameof(DeleteRow));
            if (!await infrastructureService.RowExistAsync(id))
            {
                Logger.Here().Error("{DeleteRow} - does not exist", nameof(DeleteRow));
                return BadRequestResponse("Given id is invalid");
            }

            if ((await infrastructureService.GetActiveSeatsByRowIdAsync(id)).Any())
            {
                Logger.Here().Error("{DeleteRow} - contains seats", nameof(DeleteRow));
                return BadRequestResponse("Given id contains seats");
            }

            var cinemaHall = await infrastructureService.GetCinemaHallByRowIdAsync(id);
            if (await _service.CreateReservationService().GetCountReservationsByCinemaHallAsync(cinemaHall.Id) > 0)
            {
                Logger.Here().Error("{UpdateSeatLayout} - contains reservations", nameof(UpdateSeatLayout));
                return BadRequestResponse("Given cinema hall contains reservations.");
            }

            var result = await infrastructureService.DeleteRowAsync(id);
            return GetDeletionStatus(result, "Given row is not deletable!");
        }

        /// <summary>
        /// Updates row with the given data.
        /// </summary>
        /// <param name="row">Updated row.</param>
        /// <returns>Status code.</returns>
        /// <response code="200">If the row category was updated.</response>
        /// <response code="400">If the row category was not updateable.</response>
        [HttpPut("row")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<ActionResult> UpdateRow(RowDto row)
        {
            var infrastructureService = _service.CreateInfrastructureService();

            Logger.Here().Info(nameof(UpdateRow));
            if (!await infrastructureService.RowExistAsync(row.Id))
            {
                Logger.Here().Error("{UpdateRow} - does not exist", nameof(UpdateRow));
                return BadRequestResponse("Given row category does not exist");
            }

            if (await infrastructureService.UpdateRowAsync(row) != null)
            {
                return UpdatedResponse();
            }

            Logger.Here().Error("{UpdateRow} - is not updateable", nameof(UpdateRow));
            return BadRequestResponse("Given row category is not updateable.");
        }

        /// <summary>
        /// Gets row details from a given row id.
        /// </summary>
        /// <param name="id">Row id.</param>
        /// <returns>Row details.</returns>
        /// <response code="200">If the row id was valid.</response>
        /// <response code="400">If the row id was invalid.</response>
        [HttpGet("row/{id}")]
        public async Task<ActionResult<RowDto>> GetRowAsync(long id)
        {
            var infrastructureService = _service.CreateInfrastructureService();
            Logger.Here().Info(nameof(GetRowAsync));

            if (!await infrastructureService.RowExistAsync(id))
            {
                Logger.Here().Error("{GetRowAsync} - {Id} does not exist", nameof(GetRowAsync), id);
                return BadRequestResponse("Given row id is invalid.");
            }

            return await _service.CreateInfrastructureService().GetRowWithCategoryByIdAsync(id);
        }

        /// <summary>
        /// Gets row categories from a given cinema hall id.
        /// </summary>
        /// <param name="cinemaHallId">Cinema hall id.</param>
        /// <returns>Row categories from cinema hall.</returns>
        /// <response code="200">If the cinema hall id was valid.</response>
        /// <response code="400">If the cinema hall id was invalid.</response>
        [HttpGet("categoriesByCinemaHall")]
        public async Task<ActionResult<IList<RowCategoryDto>>> GetRowCategoriesFromCinemaHall(long cinemaHallId)
        {
            var infrastructureService = _service.CreateInfrastructureService();
            Logger.Here().Info(nameof(GetRowCategoriesFromCinemaHall));

            if (!(await infrastructureService.CinemaHallExistAsync(cinemaHallId)))
            {
                Logger.Here().Error("{GetRowCategoriesFromCinemaHall} - {Id} not exist",  nameof(GetRowCategoriesFromCinemaHall), cinemaHallId);
                return BadRequestResponse("Given cinema hall id is invalid.");
            }

            return (await infrastructureService.GetActiveRowCategoriesFromCinemaHallAsync(cinemaHallId)).ToList();
        }

        /// <summary>
        /// Gets row categories.
        /// </summary>
        /// <returns>Row categories.</returns>
        /// <response code="200">Row categories.</response>
        [HttpGet("categories")]
        public async Task<ActionResult<IList<RowCategoryDto>>> GetRowCategories()
        {
            Logger.Here().Info(nameof(GetRowCategories));
            return (await _service.CreateInfrastructureService().GetActiveRowCategoriesAsync()).ToList();
        }

        /// <summary>
        /// Gets a given row category.
        /// </summary>
        /// <param name="id">New row category.</param>
        /// <returns>Row category.</returns>
        /// <response code="200">If the row category id was valid.</response>
        /// <response code="400">If the row category id was invalid.</response>
        [HttpGet("category/{id}")]
        public async Task<RowCategoryDto> GetRowCategoryAsync(long id)
        {
            Logger.Here().Info("{GetRowCategoryAsync} - {id}", nameof(GetRowCategoryAsync) , id);
            return await _service.CreateInfrastructureService().GetRowCategoryByIdAsync(id);
        }

        /// <summary>
        /// Adds a given row category.
        /// </summary>
        /// <param name="rowCategory">New row category.</param>
        /// <returns>Created id.</returns>
        /// <response code="200">If the row category was added.</response>
        /// <response code="400">If the row category was not added.</response>
        [HttpPost("category")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> AddRowCategoryAsync(RowCategoryDto rowCategory)
        {
            Logger.Here().Info("{AddRowCategoryAsync} - {rowCategory}", nameof(AddRowCategoryAsync), rowCategory.Id);
            var result = await _service.CreateInfrastructureService().AddRowCategoryAsync(rowCategory);
            return GetCreatedStatus(result, nameof(GetRowCategoryAsync), "Given row category is not addable.");
        }

        /// <summary>
        /// Deletes row category with the given id.
        /// </summary>
        /// <param name="id">Id of the row category.</param>
        /// <returns>Status code.</returns>
        /// <response code="200">If the row category was deleted.</response>
        /// <response code="400">If the row category was not deletable.</response>
        [HttpDelete("category/{id}")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> DeleteRowCategory(long id)
        {
            var infrastructureService = _service.CreateInfrastructureService();

            Logger.Here().Info(nameof(DeleteRowCategory));
            if (!await infrastructureService.RowCategoryExistAsync(id))
            {
                Logger.Here().Error("{DeleteRowCategory} - {Id} does not exist", nameof(DeleteRowCategory), id);
                return BadRequestResponse("Given id is invalid");
            }

            if ((await infrastructureService.GetActiveSeatsByRowCategoryAsync(id)).Any())
            {
                Logger.Here().Error("{DeleteRowCategory} - row category contains referenced seats", nameof(DeleteRowCategory));
                return BadRequestResponse("Given id contains referenced seats");
            }

            var result = await infrastructureService.DeleteRowCategoryAsync(id);
            return GetDeletionStatus(result, "Given row category is not deletable.");
        }

        /// <summary>
        /// Updates row category with the given data.
        /// </summary>
        /// <param name="rowCategory">Updated row category.</param>
        /// <returns>Status code.</returns>
        /// <response code="200">If the row category was updated.</response>
        /// <response code="400">If the row category was not updateable.</response>
        [HttpPut("category")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<IActionResult> UpdateRowCategory(RowCategoryDto rowCategory)
        {
            var infrastructureService = _service.CreateInfrastructureService();

            Logger.Here().Info(nameof(UpdateRowCategory));
            if (!await infrastructureService.RowCategoryExistAsync(rowCategory.Id))
            {
                Logger.Here().Error("{UpdateRowCategory} - {Id} not exist", nameof(UpdateRowCategory), rowCategory.Id);
                return BadRequestResponse("Given row category do not exist");
            }

            var result = await infrastructureService.UpdateRowCategoryAsync(rowCategory);
            return GetUpdatedStatus(result, "Given row category is not updateable.");
        }

        /// <summary>
        /// Updates a cinema hall with the given seat layout.
        /// </summary>
        /// <param name="id">Cinema hall id.</param>
        /// <param name="seatLayout">New seat layout.</param>
        /// <returns>Status code.</returns>
        /// <response code="200">If the update was successful.</response>
        /// <response code="400">If the update was not possible.</response>
        [HttpPut("seatlayout/{id}")]
        [AuthorizeRole(ApolloRoles.Admin)]
        public async Task<ActionResult> UpdateSeatLayout(long id, IList<SeatDto> seatLayout)
        {
            var infrastructureService = _service.CreateInfrastructureService();

            Logger.Here().Info(nameof(UpdateSeatLayout));
            if (!await infrastructureService.CinemaHallExistAsync(id))
            {
                Logger.Here().Error("{UpdateSeatLayout} - {Id} not exist", nameof(UpdateSeatLayout), id);
                return BadRequestResponse("Given cinema hall do not exist");
            }

            if (await _service.CreateReservationService().GetCountReservationsByCinemaHallAsync(id) > 0)
            {
                Logger.Here().Error("{UpdateSeatLayout} - contains reservations", nameof(UpdateSeatLayout));
                return BadRequestResponse("Given cinema hall contains reservations.");
            }

            try
            {
                await infrastructureService.UpdateSeatLayout(id, seatLayout);
                return UpdatedResponse();
            }
            catch (PersistenceException e)
            {
                Logger.Here().Error(e, "{UpdateSeatLayout()} - is not updateable", nameof(UpdateSeatLayout));
                return BadRequestResponse("Seat layout is not updateable.");
            }
        }


        private async Task<ActionResult> UpdateSeatLockedAsync(string methodName, long seatId, bool locked)
        {
            var infrastructureService = _service.CreateInfrastructureService();
            Logger.Here().Info($"{methodName} - {seatId}");

            if (!await infrastructureService.SeatExistAsync(seatId))
            {
                Logger.Here().Error("{Method} - seat does not exist", methodName);
                return BadRequestResponse("Given seat is invalid");
            }

            if (await infrastructureService.UpdateSeatLockState(seatId, locked))
            {
                return UpdatedResponse();
            }

            var lockType = locked ? "lockable" : "unlock able";
            Logger.Here().Error("{Method} - not {lockType}", methodName);
            return BadRequestResponse($"Given seat is not {lockType}.");
        }
    }
}