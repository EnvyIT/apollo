using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Repository.Interfaces
{
    public interface IRepositoryInfrastructure : IRepository
    {
        /*
         * Select
         */
        Task<CinemaHall> GetCinemaHallByIdAsync(long cinemaHallId);
        Task<IEnumerable<CinemaHall>> GetCinemaHallsAsync();
        Task<IEnumerable<CinemaHall>> GetActiveCinemaHallsAsync();
        Task<IEnumerable<Seat>> GetSeatsAsync(long cinemaHallId);
        Task<IEnumerable<Seat>> GetSeatsWithRowAndCategoryAsync(long cinemaHallId);
        Task<IEnumerable<Seat>> GetActiveSeatsWithRowAndCategoryAsync(long cinemaHallId);
        Task<IEnumerable<Seat>> GetActiveSeatsWithRowByRowCategoryAsync(long rowCategoryId);
        Task<Seat> GetSeatByIdAsync(long seatId);
        Task<IEnumerable<Row>> GetRowsFromCinemaHallAsync(long cinemaHallId);
        Task<IEnumerable<Row>> GetActiveRowsFromCinemaHallAsync(long cinemaHallId);
        Task<Row> GetRowByIdAsync(long rowId);
        Task<IEnumerable<Seat>> GetActiveSeatsFromCinemaHallByCategoryAsync(long cinemaHallId, long categoryId);
        Task<IEnumerable<Seat>> GetActiveSeatsFromCinemaHallByCategoryAsync(long cinemaHallId, IEnumerable<long> categoryIds);
        Task<IEnumerable<Seat>> GetActiveSeatsByRowIdAsync(long rowId);
        Task<IEnumerable<RowCategory>> GetActiveRowCategoriesFromCinemaHallAsync(long cinemaHallId);
        Task<IEnumerable<RowCategory>> GetActiveRowCategoriesAsync();
        Task<RowCategory> GetRowCategoryByIdAsync(long rowCategoryId);
        Task<Row> GetRowWithCategoryByIdAsync(long rowId);
        Task<CinemaHall> GetCinemaHallByRowIdAsync(long rowId);
        Task<bool> CinemaHallExistAsync(long id);
        Task<bool> SeatExistAsync(long id);
        Task<bool> RowExistAsync(long id);
        Task<bool> RowCategoryExistAsync(long id);


        /*
         * Add
         */
        Task<long> AddCinemaHallAsync(CinemaHall cinemaHall);
        Task<long> AddRowAsync(Row row);
        Task<long> AddRowCategoryAsync(RowCategory category);
        Task<long> AddSeatAsync(Seat seat);
        /*
         * Delete
         */
        Task<int> DeleteCinemaHallAsync(long cinemaHallId);
        Task<int> DeleteRowAsync(long rowId);
        Task<int> DeleteRowCategoryAsync(long rowCategoryId);
        Task<int> DeleteSeatAsync(long seatId);

        /*
         * Update
         */
        Task<int> UpdateCinemaHallAsync(CinemaHall cinemaHall);
        Task<int> UpdateRowAsync(Row row);
        Task<int> UpdateRowCategoryAsync(RowCategory category);
        Task<int> UpdateSeatAsync(Seat seat);
        Task<int> UpdateSeatLockStateAsync(long seatId, bool locked);
    }
}
