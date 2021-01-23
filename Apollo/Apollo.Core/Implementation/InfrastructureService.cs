using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Core.Exception;
using Apollo.Core.Interfaces;
using Apollo.Domain.Entity;
using Apollo.UnitOfWork.Interfaces;
using Apollo.Util.Logger;
using static Apollo.Core.Dto.Mapper;
using static Apollo.Util.ValidationHelper;

namespace Apollo.Core.Implementation
{
    public class InfrastructureService : IInfrastructureService
    {
        private readonly IUnitOfWork _unitOfWork;

        private static readonly IApolloLogger<InfrastructureService> Logger =
            LoggerFactory.CreateLogger<InfrastructureService>();

        public InfrastructureService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<CinemaHallDto>> GetCinemaHallsAsync()
        {
            return (await _unitOfWork.RepositoryInfrastructure.GetCinemaHallsAsync())
                .Select(Map);
        }

        public async Task<CinemaHallDto> GetCinemaHallByIdAsync(long id)
        {
            return Map(await _unitOfWork.RepositoryInfrastructure.GetCinemaHallByIdAsync(id));
        }

        public async Task<IEnumerable<CinemaHallDto>> GetActiveCinemaHallsAsync()
        {
            return (await _unitOfWork.RepositoryInfrastructure.GetActiveCinemaHallsAsync())
                .Select(Map);
        }

        public async Task<IEnumerable<SeatDto>> GetSeatsWithRowAndCategoryAsync(long cinemaHallId)
        {
            return (await _unitOfWork.RepositoryInfrastructure.GetSeatsWithRowAndCategoryAsync(cinemaHallId))
                .Select(Map);
        }

        public async Task<IEnumerable<SeatDto>> GetActiveSeatsWithRowAndCategoryAsync(long cinemaHallId)
        {
            return (await _unitOfWork.RepositoryInfrastructure.GetActiveSeatsWithRowAndCategoryAsync(cinemaHallId))
                .Select(Map);
        }

        public async Task<IEnumerable<SeatDto>> GetActiveSeatsByRowCategoryAsync(long rowCategoryId)
        {
            return (await _unitOfWork.RepositoryInfrastructure.GetActiveSeatsWithRowByRowCategoryAsync(rowCategoryId))
                .Select(Map);
        }

        public async Task<IEnumerable<RowDto>> GetRowsFromCinemaHallAsync(long cinemaHallId)
        {
            return (await _unitOfWork.RepositoryInfrastructure.GetRowsFromCinemaHallAsync(cinemaHallId))
                .Select(Map);
        }

        public async Task<IEnumerable<RowDto>> GetActiveRowsFromCinemaHallAsync(long cinemaHallId)
        {
            return (await _unitOfWork.RepositoryInfrastructure.GetActiveRowsFromCinemaHallAsync(cinemaHallId))
                .Select(Map);
        }

        public async Task<IEnumerable<SeatDto>> GetActiveSeatsFromCinemaHallByCategoryAsync(long cinemaHallId,
            long categoryId)
        {
            return (await _unitOfWork.RepositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(cinemaHallId,
                    categoryId))
                .Select(Map);
        }

        public async Task<IEnumerable<SeatDto>> GetActiveSeatsByRowIdAsync(long id)
        {
            return (await _unitOfWork.RepositoryInfrastructure.GetActiveSeatsByRowIdAsync(id))
                .Select(Map);
        }

        public async Task<IEnumerable<SeatDto>> GetActiveSeatsFromCinemaHallByCategoriesAsync(long cinemaHallId,
            IEnumerable<long> categoryIds)
        {
            ValidateNull(Logger.Here(), categoryIds);
            return (await _unitOfWork.RepositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(cinemaHallId,
                    categoryIds))
                .Select(Map);
        }

        public async Task<IEnumerable<RowCategoryDto>> GetActiveRowCategoriesFromCinemaHallAsync(long cinemaHallId)
        {
            return (await _unitOfWork.RepositoryInfrastructure.GetActiveRowCategoriesFromCinemaHallAsync(cinemaHallId))
                .Select(Map);
        }

        public async Task<IEnumerable<RowCategoryDto>> GetActiveRowCategoriesAsync()
        {
            return (await _unitOfWork.RepositoryInfrastructure.GetActiveRowCategoriesAsync())
                .Select(Map);
        }

        public async Task<RowCategoryDto> GetRowCategoryByIdAsync(long id)
        {
            return Map(await _unitOfWork.RepositoryInfrastructure.GetRowCategoryByIdAsync(id));
        }

        public async Task<RowDto> GetRowWithCategoryByIdAsync(long rowId)
        {
            return Map(await _unitOfWork.RepositoryInfrastructure.GetRowWithCategoryByIdAsync(rowId));
        }

        public async Task<CinemaHallDto> GetCinemaHallByRowIdAsync(long id)
        {
            return Map(await _unitOfWork.RepositoryInfrastructure.GetCinemaHallByRowIdAsync(id));
        }

        public async Task<bool> CinemaHallExistAsync(long id)
        {
            return await _unitOfWork.RepositoryInfrastructure.CinemaHallExistAsync(id);
        }

        public async Task<bool> SeatExistAsync(long id)
        {
            return await _unitOfWork.RepositoryInfrastructure.SeatExistAsync(id);
        }

        public async Task<bool> RowExistAsync(long id)
        {
            return await _unitOfWork.RepositoryInfrastructure.RowExistAsync(id);
        }

        public async Task<bool> RowCategoryExistAsync(long id)
        {
            return await _unitOfWork.RepositoryInfrastructure.RowCategoryExistAsync(id);
        }

        public async Task<CinemaHallDto> AddCinemaHallAsync(CinemaHallDto cinemaHall)
        {
            ValidateNull(Logger.Here(), cinemaHall);
            var newId = await _unitOfWork.RepositoryInfrastructure.AddCinemaHallAsync(Map(cinemaHall));
            if (newId > 0L)
            {
                cinemaHall.Id = newId;
            }

            return newId > 0L ? cinemaHall : null;
        }

        public async Task<RowDto> AddRowAsync(RowDto row)
        {
            ValidateNull(Logger.Here(), row);
            var rows = await GetRowsFromCinemaHallAsync(row.CinemaHallId);
            if (rows.Any(r => r.Number == row.Number))
            {
                var exception = new PersistenceException(nameof(AddRowAsync));
                Logger.Error(exception, "{Row} with {number} already exists", row, row.Number);
                throw exception;
            }

            var id = await _unitOfWork.RepositoryInfrastructure.AddRowAsync(Map(row));
            row.Id = id;
            return row;
        }

        public async Task<RowCategoryDto> AddRowCategoryAsync(RowCategoryDto category)
        {
            ValidateNull(Logger.Here(), category);
            var newId = await _unitOfWork.RepositoryInfrastructure.AddRowCategoryAsync(Map(category));
            if (newId > 0L)
            {
                category.Id = newId;
            }

            return newId > 0L ? category : null;
        }

        public async Task<SeatDto> AddSeatAsync(SeatDto seat)
        {
            ValidateNull(Logger.Here(), seat);
            var cinemaHall = await _unitOfWork.RepositoryInfrastructure.GetCinemaHallByIdAsync(seat.Row.CinemaHallId);
            ValidateNull(Logger.Here(), cinemaHall);
            ValidateLessOrEquals<InfrastructureService, long>(Logger.Here(), seat.LayoutColumn, cinemaHall.SizeColumn);
            ValidateLessOrEquals<InfrastructureService, long>(Logger.Here(), seat.LayoutRow, cinemaHall.SizeRow);

            var row = await _unitOfWork.RepositoryInfrastructure.GetRowByIdAsync(seat.RowId);
            var seats = await _unitOfWork.RepositoryInfrastructure.GetSeatsWithRowAndCategoryAsync(row.CinemaHallId);
            if (seats.Any(s => s.LayoutColumn == seat.LayoutColumn && s.LayoutRow == seat.LayoutRow))
            {
                var exception = new PersistenceException(nameof(AddSeatAsync));
                Logger.Error(exception, "{Seat} with same layout already exists", seat);
                throw exception;
            }

            await _unitOfWork.RepositoryInfrastructure.AddSeatAsync(Map(seat));
            return seat;
        }

        public async Task<bool> DeleteCinemaHallAsync(long cinemaHallId)
        {
            var result = true;

            var cinemaHall = await _unitOfWork.RepositoryInfrastructure.GetCinemaHallByIdAsync(cinemaHallId);
            var rows = (await _unitOfWork.RepositoryInfrastructure.GetRowsFromCinemaHallAsync(cinemaHallId)).ToList();
            if (cinemaHall == null)
            {
                return false;
            }

            await _unitOfWork.Transaction().PerformBlock(async () =>
            {
                result = result && await DeleteAllRows(rows); //implicit deletion of seats
                result = result && await _unitOfWork.RepositoryInfrastructure.DeleteCinemaHallAsync(cinemaHallId) > 0;
            }).Commit();

            return result;
        }

        private async Task<bool> DeleteAllRows(IEnumerable<Row> rows)
        {
            ValidateNull(Logger, rows);
            var deleted = true;
            foreach (var row in rows)
            {
                deleted = deleted && await DeleteRowAsync(row.Id);
            }

            return deleted;
        }

        public async Task<bool> DeleteRowAsync(long rowId)
        {
            var result = true;

            var row = await _unitOfWork.RepositoryInfrastructure.GetRowByIdAsync(rowId);
            if (row == null)
            {
                return false;
            }

            var seats = (await _unitOfWork.RepositoryInfrastructure.GetSeatsWithRowAndCategoryAsync(row.CinemaHallId))
                .ToList();

            await _unitOfWork.Transaction().PerformBlock(async () =>
            {
                result = result && await DeleteAllSeats(seats);
                result = result && (await _unitOfWork.RepositoryInfrastructure.DeleteRowAsync(row.Id) > 0);
            }).Commit();

            return result;
        }

        private async Task<bool> DeleteAllSeats(IEnumerable<Seat> seats)
        {
            ValidateNull(Logger, seats);
            var deleted = true;
            foreach (var seat in seats)
            {
                deleted = deleted && await DeleteSeatAsync(seat.Id);
            }

            return deleted;
        }

        public async Task<bool> DeleteRowCategoryAsync(long rowCategoryId)
        {
            return (await _unitOfWork.RepositoryInfrastructure.DeleteRowCategoryAsync(rowCategoryId)) > 0;
        }

        public async Task<bool> DeleteSeatAsync(long seatId)
        {
            return (await _unitOfWork.RepositoryInfrastructure.DeleteSeatAsync(seatId)) > 0;
        }

        public async Task<CinemaHallDto> UpdateCinemaHallAsync(CinemaHallDto cinemaHallDto)
        {
            ValidateNull(Logger.Here(), cinemaHallDto);
            var cinemaHall = Map(cinemaHallDto);
            return (await _unitOfWork.RepositoryInfrastructure.UpdateCinemaHallAsync(cinemaHall)) > 0
                ? cinemaHallDto
                : null;
        }

        public async Task<RowDto> UpdateRowAsync(RowDto rowDto)
        {
            ValidateNull(Logger.Here(), rowDto);
            var row = Map(rowDto);
            return (await _unitOfWork.RepositoryInfrastructure.UpdateRowAsync(row)) > 0 ? rowDto : null;
        }

        public async Task<RowCategoryDto> UpdateRowCategoryAsync(RowCategoryDto categoryDto)
        {
            ValidateNull(Logger.Here(), categoryDto);
            var category = Map(categoryDto);
            return (await _unitOfWork.RepositoryInfrastructure.UpdateRowCategoryAsync(category)) > 0
                ? categoryDto
                : null;
        }

        public async Task<SeatDto> UpdateSeatAsync(SeatDto seatDto)
        {
            ValidateNull(Logger.Here(), seatDto);
            var seat = Map(seatDto);
            return (await _unitOfWork.RepositoryInfrastructure.UpdateSeatAsync(seat)) > 0 ? seatDto : null;
        }

        public async Task<bool> UpdateSeatLockState(long seatId, bool locked)
        {
            return await _unitOfWork.RepositoryInfrastructure.UpdateSeatLockStateAsync(seatId, locked) > 0;
        }


        public async Task<SeatDto[,]> GetLayout(ScheduleDto schedule)
        {
            ValidateNull(Logger.Here(), schedule);
            var seats =
                (await _unitOfWork.RepositoryInfrastructure.GetActiveSeatsWithRowAndCategoryAsync(schedule.CinemaHallId))
                .Select(Map)
                .ToList();

            var cinemaHall = await _unitOfWork.RepositoryInfrastructure.GetCinemaHallByIdAsync(schedule.CinemaHallId);
            ValidateNull(Logger.Here(), cinemaHall);

            var freeSeats = (await _unitOfWork.RepositoryTicket.GetFreeSeatsAsync(Map(schedule))).ToList();
            var maxRow = cinemaHall.SizeRow;
            var maxColumn = cinemaHall.SizeColumn;

            var layout = new SeatDto[maxRow, maxColumn];
            for (var row = 0; row < maxRow; ++row)
            {
                for (var column = 0; column < maxColumn; ++column)
                {
                    var seat = seats.SingleOrDefault(s => s.LayoutColumn == column && s.LayoutRow == row);
                    if (seat != null && seat.State != SeatState.Locked)
                    {
                        seat.State = freeSeats.Any(s => s.Id == seat.Id) ? SeatState.Free : SeatState.Occupied;
                    }

                    layout[row, column] = seat;
                }
            }

            return layout;
        }

        public async Task<SeatDto[,]> GetLayout(long cinemaHallId)
        {
            var seats = (await _unitOfWork.RepositoryInfrastructure.GetActiveSeatsWithRowAndCategoryAsync(cinemaHallId))
                .Select(Map)
                .ToList();

            var cinemaHall = await _unitOfWork.RepositoryInfrastructure.GetCinemaHallByIdAsync(cinemaHallId);
            ValidateNull(Logger.Here(), cinemaHall);

            var maxRow = cinemaHall.SizeRow;
            var maxColumn = cinemaHall.SizeColumn;

            var layout = new SeatDto[maxRow, maxColumn];
            for (var row = 0; row < maxRow; ++row)
            {
                for (var column = 0; column < maxColumn; ++column)
                {
                    var seat = seats.SingleOrDefault(s => s.LayoutColumn == column && s.LayoutRow == row);
                    layout[row, column] = seat;
                }
            }

            return layout;
        }

        public async Task UpdateSeatLayout(long cinemaHallId, IEnumerable<SeatDto> seats)
        {
            var cinemaHall = await _unitOfWork.RepositoryInfrastructure.GetCinemaHallByIdAsync(cinemaHallId);
            ValidateNull(Logger.Here(), cinemaHall);

            var seatList = seats.ToList();
            ValidateSeatLayout(cinemaHall, seatList);

            var currentSeats =
                (await _unitOfWork.RepositoryInfrastructure.GetSeatsWithRowAndCategoryAsync(cinemaHallId)).ToList();
            ValidateNull(Logger.Here(), currentSeats);

            var rows = await _unitOfWork.RepositoryInfrastructure.GetRowsFromCinemaHallAsync(cinemaHallId);
            ValidateNull(Logger.Here(), rows);

            if (seatList.Any(s => !rows.Select(r => r.Id).Contains(s.RowId)))
            {
                throw new PersistenceException("Invalid row given!");
            }

            var currentIds = currentSeats.Select(s => s.Id).ToList();
            var seatIds = seatList.Select(s => s.Id).ToList();

            var updated = seatList
                .Where(s => seatIds.Where(id => currentIds.Contains(id)).Contains(s.Id))
                .Where(seat => !SeatIsEqual(seat, currentSeats.First(s => s.Id == seat.Id))).ToList();
            var removed = currentSeats.Where(s => currentIds.Except(seatIds).Contains(s.Id)).ToList();
            var added = seatList.Where(s => seatIds.Except(currentIds).Contains(s.Id)).ToList();

            await _unitOfWork.Transaction().PerformBlock(async () =>
            {
                foreach (var seat in updated)
                {
                    if (await _unitOfWork.RepositoryInfrastructure.UpdateSeatAsync(Map(seat)) != 1)
                    {
                        throw new PersistenceException($"Unable to update seat with id {seat.Id}");
                    }
                }

                foreach (var seat in removed)
                {
                    await _unitOfWork.RepositoryInfrastructure.DeleteSeatAsync(seat.Id);
                }

                foreach (var seat in added)
                {
                    await _unitOfWork.RepositoryInfrastructure.AddSeatAsync(Map(seat));
                }
            }).Commit();
        }

        private static bool SeatIsEqual(SeatDto seat1, Seat seat2)
        {
            return seat1.Number == seat2.Number &&
                   seat1.LayoutColumn == seat2.LayoutColumn &&
                   seat1.LayoutRow == seat2.LayoutRow &&
                   seat1.RowId == seat2.RowId;
        }

        private static void ValidateSeatLayout(CinemaHall cinemaHall, IEnumerable<SeatDto> seats)
        {
            var seatList = seats.ToList();

            if (seatList.Any(s1 =>
                seatList.Count(s2 => s1.LayoutColumn == s2.LayoutColumn && s1.LayoutRow == s2.LayoutRow) > 1))
            {
                throw new ArgumentException($"Duplicate coordinates given!");
            }

            if (seatList.Any(seat => seat.LayoutRow < 0 || seat.LayoutRow >= cinemaHall.SizeRow ||
                                     seat.LayoutColumn < 0 || seat.LayoutColumn >= cinemaHall.SizeColumn))
            {
                throw new ArgumentException("Invalid layout coordinates given!");
            }
        }
    }
}