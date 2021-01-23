using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Exception;
using Apollo.Persistence.Util;
using Apollo.Repository.Interfaces;
using Apollo.Util.Logger;

namespace Apollo.Repository.Implementation
{
    public class RepositoryInfrastructure : Repository, IRepositoryInfrastructure
    {
        private readonly IApolloLogger<RepositoryInfrastructure> Logger = LoggerFactory.CreateLogger<RepositoryInfrastructure>();

        private readonly IRowDao _rowDao;
        private readonly IRowCategoryDao _rowCategoryDao;
        private readonly ISeatDao _seatDao;
        private readonly ICinemaHallDao _cinemaHallDao;

        public RepositoryInfrastructure(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _rowDao = DaoFactory.CreateRowDao();
            _rowCategoryDao = DaoFactory.CreateRowCategoryDao();
            _seatDao = DaoFactory.CreateSeatDao();
            _cinemaHallDao = DaoFactory.CreateCinemaHallDao();
        }

        public async Task<CinemaHall> GetCinemaHallByIdAsync(long cinemaHallId)
        {
            await ValidateId(_cinemaHallDao, cinemaHallId);
            return await _cinemaHallDao.SelectSingleByIdAsync(cinemaHallId);
        }

        public Task<IEnumerable<CinemaHall>> GetCinemaHallsAsync()
        {
            return _cinemaHallDao.SelectAsync();
        }

        public Task<IEnumerable<CinemaHall>> GetActiveCinemaHallsAsync()
        {
            return _cinemaHallDao.SelectActiveAsync();
        }

        public async Task<IEnumerable<Seat>> GetSeatsAsync(long cinemaHallId)
        {
            return await _seatDao.SelectSeatsFromCinemaHallAsync(cinemaHallId);
        }

        public async Task<IEnumerable<Seat>> GetSeatsWithRowAndCategoryAsync(long cinemaHallId)
        {
            await ValidateId(_cinemaHallDao, cinemaHallId);
            return await _seatDao.SelectWithRowAndCategoryByCinemaHallAsync(cinemaHallId);
        }

        public async Task<IEnumerable<Seat>> GetActiveSeatsWithRowAndCategoryAsync(long cinemaHallId)
        {
            await ValidateId(_cinemaHallDao, cinemaHallId);
            return await _seatDao.SelectActiveWithRowAndCategoryByCinemaHallAsync(cinemaHallId);
        }

        public async Task<IEnumerable<Seat>> GetActiveSeatsWithRowByRowCategoryAsync(long rowCategoryId)
        {
            await ValidateId(_rowCategoryDao, rowCategoryId);
            return await _seatDao.SelectActiveSeatsWithRowByRowCategoryAsync(rowCategoryId);
        }

        public async Task<Seat> GetSeatByIdAsync(long seatId)
        {
            await ValidateId(_seatDao, seatId);
            return await _seatDao.SelectSingleByIdAsync(seatId);
        }

        public async Task<IEnumerable<Row>> GetRowsFromCinemaHallAsync(long cinemaHallId)
        {
            await ValidateId(_cinemaHallDao, cinemaHallId);
            return await _rowDao.SelectRowsFromCinemaHallAsync(cinemaHallId);
        }

        public async Task<IEnumerable<Row>> GetActiveRowsFromCinemaHallAsync(long cinemaHallId)
        {
            await ValidateId(_cinemaHallDao, cinemaHallId);
            return await _rowDao.SelectActiveRowsFromCinemaHallAsync(cinemaHallId);
        }

        public async Task<Row> GetRowByIdAsync(long rowId)
        {
            await ValidateId(_rowDao, rowId);
            return await _rowDao.SelectSingleByIdAsync(rowId);
        }

        public async Task<IEnumerable<Seat>> GetActiveSeatsFromCinemaHallByCategoryAsync(long cinemaHallId,
            long categoryId)
        {
            await ValidateId(_cinemaHallDao, cinemaHallId);
            await ValidateId(_rowCategoryDao, categoryId);
            return await _seatDao.SelectActiveSeatsFromCinemaHallByCategoryAsync(cinemaHallId, categoryId);
        }

        public async Task<IEnumerable<Seat>> GetActiveSeatsFromCinemaHallByCategoryAsync(long cinemaHallId,
            IEnumerable<long> categoryIds)
        {
            ValidateCollection(categoryIds);
            await ValidateId(_cinemaHallDao, cinemaHallId);

            var categoryIdList = categoryIds.ToList();
            await ValidateIds(_rowCategoryDao, categoryIdList);

            return await _seatDao.SelectActiveSeatsFromCinemaHallByCategoryAsync(cinemaHallId, categoryIdList);
        }

        public async Task<IEnumerable<Seat>> GetActiveSeatsByRowIdAsync(long rowId)
        {
            await ValidateId(_rowDao, rowId);
            return await _seatDao.SelectActiveSeatsByIdAsync(rowId);
        }

        public async Task<IEnumerable<RowCategory>> GetActiveRowCategoriesFromCinemaHallAsync(long cinemaHallId)
        {
            await ValidateId(_cinemaHallDao, cinemaHallId);
            return await _rowDao.SelectActiveRowCategoriesFromCinemaHallAsync(cinemaHallId);
        }

        public Task<IEnumerable<RowCategory>> GetActiveRowCategoriesAsync()
        {
            return _rowCategoryDao.SelectActiveAsync();
        }

        public async Task<RowCategory> GetRowCategoryByIdAsync(long rowCategoryId)
        {
            await ValidateId(_rowCategoryDao, rowCategoryId);
            return await _rowCategoryDao.SelectSingleByIdAsync(rowCategoryId);
        }

        public async Task<Row> GetRowWithCategoryByIdAsync(long rowId)
        {
            await ValidateId(_rowDao, rowId);
            return await _rowDao.SelectSingleRowWithCategoryAsync(rowId);
        }
        
        public async Task<CinemaHall> GetCinemaHallByRowIdAsync(long rowId)
        {
            await ValidateId(_rowDao, rowId);
            return await _rowDao.GetCinemaHallByRowIdAsync(rowId);
        }

        public async Task<bool> CinemaHallExistAsync(long id)
        {
            return await _cinemaHallDao.ExistAndNotDeletedByIdAsync(id);
        }

        public async Task<bool> SeatExistAsync(long id)
        {
            return await _seatDao.ExistAndNotDeletedByIdAsync(id);
        }

        public async Task<bool> RowExistAsync(long id)
        {
            return await _rowDao.ExistAndNotDeletedByIdAsync(id);
        }

        public async Task<bool> RowCategoryExistAsync(long id)
        {
            return await _rowCategoryDao.ExistAndNotDeletedByIdAsync(id);
        }

        public async Task<long> AddCinemaHallAsync(CinemaHall cinemaHall)
        {
            ValidateNotNull(cinemaHall);
            if (await _cinemaHallDao.ExistByIdAsync(cinemaHall.Id))
            {
                var duplicateEntryException = new DuplicateEntryException(cinemaHall.Id, "CinemaHall already exists!");
                Logger.Error(duplicateEntryException, "{CinemaHall} already exists", cinemaHall);
                throw duplicateEntryException;
            }

            return (await _cinemaHallDao.FluentInsert(cinemaHall).ExecuteAsync()).First();
        }

        public async Task<long> AddRowAsync(Row row)
        {
            ValidateNotNull(row);
            if (await _rowDao.ExistByIdAsync(row.Id))
            {
                var duplicateEntryException = new DuplicateEntryException(row.Id, "Row already exists!");
                Logger.Error(duplicateEntryException, "{Row} already exists", row);
                throw duplicateEntryException;
            }

            if (row.CinemaHall != null && row.CinemaHallId <= 0)
            {
                row.CinemaHallId = row.CinemaHall.Id;
            }
            if (row.Category != null && row.CategoryId <= 0)
            {
                row.CategoryId = row.Category.Id;
            }

            await ValidateId(_cinemaHallDao, row.CinemaHallId);
            await ValidateId(_rowCategoryDao, row.CategoryId);

            return (await _rowDao.FluentInsert(row).ExecuteAsync()).First();
        }

        public async Task<long> AddRowCategoryAsync(RowCategory category)
        {
            ValidateNotNull(category);
            if (await _rowCategoryDao.ExistByIdAsync(category.Id))
            {
                var duplicateEntryException = new DuplicateEntryException(category.Id, "Category already exists!");
                Logger.Error(duplicateEntryException, "{Category} already exists", category);
                throw duplicateEntryException;
            }

            return (await _rowCategoryDao.FluentInsert(category).ExecuteAsync()).First();
        }

        public async Task<long> AddSeatAsync(Seat seat)
        {
            ValidateNotNull(seat);
            if (await _seatDao.ExistByIdAsync(seat.Id))
            { 
                var duplicateEntryException = new DuplicateEntryException(seat.Id, "Seat already exist!");
                Logger.Error(duplicateEntryException, "{Seat} already exists", seat);
                throw duplicateEntryException;
            }
            if (seat.Row != null && seat.RowId <= 0)
            {
                seat.RowId = seat.Row.Id;
            }

            await ValidateId(_rowDao, seat.RowId);

            return (await _seatDao.FluentInsert(seat).ExecuteAsync()).First();
        }

        public async Task<int> DeleteCinemaHallAsync(long cinemaHallId)
        {
            await ValidateId(_cinemaHallDao, cinemaHallId);
            return await _cinemaHallDao.FluentSoftDeleteById(cinemaHallId);
        }

        public async Task<int> DeleteRowAsync(long rowId)
        {
            await ValidateId(_rowDao, rowId);
            return await _rowDao.FluentSoftDeleteById(rowId);
        }

        public async Task<int> DeleteRowCategoryAsync(long rowCategoryId)
        {
            await ValidateId(_rowCategoryDao, rowCategoryId);
            return await _rowCategoryDao.FluentSoftDeleteById(rowCategoryId);
        }

        public async Task<int> DeleteSeatAsync(long seatId)
        {
            await ValidateId(_seatDao, seatId);
            return await _seatDao.FluentSoftDeleteById(seatId);
        }

        public async Task<int> UpdateCinemaHallAsync(CinemaHall cinemaHall)
        {
            ValidateNotNull(cinemaHall);
            await ValidateId(_cinemaHallDao, cinemaHall.Id);
            return await _cinemaHallDao.FluentUpdate(cinemaHall).ExecuteAsync();
        }

        public async Task<int> UpdateRowAsync(Row row)
        {
            ValidateNotNull(row);
            await ValidateId(_rowDao, row.Id);
            return await _rowDao.FluentUpdate(row).ExecuteAsync();
        }

        public async Task<int> UpdateRowCategoryAsync(RowCategory category)
        {
            ValidateNotNull(category);
            await ValidateId(_rowCategoryDao, category.Id);
            return await _rowCategoryDao.FluentUpdate(category).ExecuteAsync();
        }

        public async Task<int> UpdateSeatAsync(Seat seat)
        {
            ValidateNotNull(seat);
            await ValidateId(_seatDao, seat.Id);
            return await _seatDao.FluentUpdate(seat).ExecuteAsync();
        }

        public async Task<int> UpdateSeatLockStateAsync(long seatId, bool locked)
        {
            await ValidateId(_seatDao, seatId);
            return await _seatDao.UpdateSeatLockStateAsync(seatId, locked);
        }
    }
}