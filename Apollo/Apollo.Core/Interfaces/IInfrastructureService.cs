using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Core.Dto;

namespace Apollo.Core.Interfaces
{
    public interface IInfrastructureService
    {
        Task<IEnumerable<CinemaHallDto>> GetCinemaHallsAsync();
        Task<CinemaHallDto> GetCinemaHallByIdAsync(long id);
        Task<IEnumerable<CinemaHallDto>> GetActiveCinemaHallsAsync();
        Task<IEnumerable<SeatDto>> GetSeatsWithRowAndCategoryAsync(long cinemaHallId);
        Task<IEnumerable<SeatDto>> GetActiveSeatsWithRowAndCategoryAsync(long cinemaHallId);
        Task<IEnumerable<SeatDto>> GetActiveSeatsByRowCategoryAsync(long rowCategoryId);
        Task<IEnumerable<RowDto>> GetRowsFromCinemaHallAsync(long cinemaHallId);
        Task<IEnumerable<RowDto>> GetActiveRowsFromCinemaHallAsync(long cinemaHallId);
        Task<IEnumerable<SeatDto>> GetActiveSeatsFromCinemaHallByCategoryAsync(long cinemaHallId, long categoryId);
        Task<IEnumerable<SeatDto>> GetActiveSeatsByRowIdAsync(long id);
        Task<IEnumerable<SeatDto>> GetActiveSeatsFromCinemaHallByCategoriesAsync(long cinemaHallId, IEnumerable<long> categoryIds);
        Task<IEnumerable<RowCategoryDto>> GetActiveRowCategoriesFromCinemaHallAsync(long cinemaHallId);
        Task<IEnumerable<RowCategoryDto>> GetActiveRowCategoriesAsync();
        Task<RowCategoryDto> GetRowCategoryByIdAsync(long id);
        Task<RowDto> GetRowWithCategoryByIdAsync(long rowId);
        Task<CinemaHallDto> GetCinemaHallByRowIdAsync(long id);
        Task<bool> CinemaHallExistAsync(long id);
        Task<bool> SeatExistAsync(long id);
        Task<bool> RowExistAsync(long id);
        Task<bool> RowCategoryExistAsync(long id);

        Task<CinemaHallDto> AddCinemaHallAsync(CinemaHallDto cinemaHall);
        Task<RowDto> AddRowAsync(RowDto row);
        Task<RowCategoryDto> AddRowCategoryAsync(RowCategoryDto category);
        Task<SeatDto> AddSeatAsync(SeatDto seat);
 
        Task<bool> DeleteCinemaHallAsync(long cinemaHallId);
        Task<bool> DeleteRowAsync(long rowId);
        Task<bool> DeleteRowCategoryAsync(long rowCategoryId);
        Task<bool> DeleteSeatAsync(long seatId);

   
        Task<CinemaHallDto> UpdateCinemaHallAsync(CinemaHallDto cinemaHall);
        Task<RowDto> UpdateRowAsync(RowDto row);
        Task<RowCategoryDto> UpdateRowCategoryAsync(RowCategoryDto category);
        Task<SeatDto> UpdateSeatAsync(SeatDto seat);
        Task<bool> UpdateSeatLockState(long seatId, bool locked);
        Task UpdateSeatLayout(long cinemaHallId, IEnumerable<SeatDto> seats);

        Task<SeatDto[,]> GetLayout(ScheduleDto schedule);
        Task<SeatDto[,]> GetLayout(long cinemaHallId);
    }
}
