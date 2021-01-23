using System;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Exception;
using Apollo.Repository.Interfaces;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Repository.Test
{
    public class RepositoryInfrastructureTest
    {
        private IRepositoryInfrastructure _repositoryInfrastructure;
        private RepositoryHelper _repositoryHelper;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryHelper = new RepositoryHelper();
            _repositoryHelper.FillTypes.Add(EntityType.CinemaHall);
            _repositoryHelper.FillTypes.Add(EntityType.Row);
            _repositoryHelper.FillTypes.Add(EntityType.RowCategory);
            _repositoryHelper.FillTypes.Add(EntityType.Seat);
            _repositoryInfrastructure = _repositoryHelper.RepositoryFactory.CreateRepositoryInfrastructure();
        }

        [SetUp]
        public async Task Setup()
        {
            await _repositoryHelper.SeedDatabase();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _repositoryHelper.TearDown();
        }

        [Test]
        public async Task Test_GetAllCinemaHalls()
        {
            var halls = (await _repositoryInfrastructure.GetCinemaHallsAsync())
                .OrderBy(hall => hall.Id)
                .ToList();

            halls.Should().HaveCount(_repositoryHelper.CinemaHalls.Count());
            for (int i = 1; i <= halls.Count; i++)
            {
                var hall = halls[i - 1];
                hall.Label.Should().Be($"Apollo {i}");
            }
        }

        [Test]
        public async Task Test_GetAllActiveCinemaHalls()
        {
            await _repositoryInfrastructure.DeleteCinemaHallAsync(2L);
            var halls = (await _repositoryInfrastructure.GetActiveCinemaHallsAsync())
                .OrderBy(hall => hall.Id)
                .ToList();

            halls.Should().HaveCount(_repositoryHelper.CinemaHalls.Count() - 1);
            for (int i = 1; i <= halls.Count; i++)
            {
                var hall = halls[i - 1];
                hall.Label.Should().Be($"Apollo {i}");
            }
        }

        [Test]
        public async Task Test_GetSeatsWithRowAndCategory_InvalidId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.GetSeatsWithRowAndCategoryAsync(500L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_GetSeatsWithRowAndCategory()
        {
            await _repositoryInfrastructure.DeleteSeatAsync(1L);
            var seats = (await _repositoryInfrastructure.GetSeatsWithRowAndCategoryAsync(1L))
                .OrderBy(seat => seat.Id)
                .ToList();

            seats.Should().HaveCount(_repositoryHelper.Seats.Count());

            var seatsPerRow = _repositoryHelper.Seats.Count(seat => seat.RowId == 1L);
            for (int i = 1; i <= seats.Count; i++)
            {
                var seat = seats[i - 1];
                long rowId = (i - 1) / seatsPerRow + 1;
                seat.RowId.Should().Be(rowId);

                seat.Row.Should().NotBeNull();
                seat.Row.Number.Should().Be((int) rowId);

                seat.Row.Category.Should().NotBeNull();
                seat.Row.Category.Name.Should().Be(rowId == 1 ? "Silver" : rowId == 2 ? "Gold" : "Diamond");
            }
        }

        [Test]
        public async Task Test_GetActiveSeatsWithRowAndCategory_InvalidId_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryInfrastructure.GetActiveSeatsWithRowAndCategoryAsync(500L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_GetActiveSeatsWithRowAndCategory()
        {
            var deleted = await _repositoryInfrastructure.DeleteSeatAsync(1L);
            var seats = (await _repositoryInfrastructure.GetActiveSeatsWithRowAndCategoryAsync(1L))
                .OrderBy(seat => seat.Id)
                .ToList();

            seats.Should().HaveCount(_repositoryHelper.Seats.Count() - deleted); //24 seats -  3 are locke and not active  - 1 is deleted = 20 seats are active
            seats.Select(seat => seat.Id).Should().NotContain(1L);

            var seatsPerRow = _repositoryHelper.Seats.Count(seat => seat.RowId == 1L);
            for (int i = 2; i <= seats.Count; i++)
            {
                var seat = seats[i - 2];
                long rowId = (i - 1) / seatsPerRow + 1;
                seat.RowId.Should().Be(rowId);

                seat.Row.Should().NotBeNull();
                seat.Row.Number.Should().Be((int) rowId);

                seat.Row.Category.Should().NotBeNull();
                seat.Row.Category.Name.Should().Be(rowId == 1 ? "Silver" : rowId == 2 ? "Gold" : "Diamond");
            }
        }

        [Test]
        public async Task Test_GetRowsFromCinemaHall_InvalidId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.GetRowsFromCinemaHallAsync(500L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_GetRowsFromCinemaHall()
        {
            await _repositoryInfrastructure.DeleteRowAsync(1L);
            var rows = (await _repositoryInfrastructure.GetRowsFromCinemaHallAsync(1L))
                .OrderBy(seat => seat.Id)
                .ToList();

            rows.Should().HaveCount(_repositoryHelper.Rows.Count(_ => _.CinemaHallId == 1L));
            for (int i = 1; i <= rows.Count; ++i)
            {
                var row = rows[i - 1];
                row.Id.Should().Be(i);
                row.Number.Should().Be(i);
                row.CategoryId.Should().Be(i <= 2L ? i : i + 1);
                row.CinemaHallId.Should().Be(1L);
            }
        }

        [Test]
        public async Task Test_GetActiveSeatsFromCinemaHallByCategoryAsync_InvalidCinemaHallId_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(500L, 1L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_GetActiveSeatsFromCinemaHallByCategoryAsync_NoCategoryId_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(1L, Enumerable.Empty<long>());
            await execution.Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task Test_GetActiveSeatsFromCinemaHallByCategoryAsync_InvalidCategoryId_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(500L, new[] {1L, 500L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_GetActiveSeatsFromCinemaHallByCategoryAsync_SingleRow()
        {
            var seats = (await _repositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(1L, 1L))
                .OrderBy(seat => seat.Id)
                .ToList();
            var rows = _repositoryHelper.Rows.Where(row => row.CategoryId == 1L).Select(row => row.Id);
            var expectedSeats = _repositoryHelper.Seats
                .Where(seat => rows.Contains(seat.RowId))
                .OrderBy(row => row.Id)
                .ToList();

            seats.Should().HaveCount(expectedSeats.Count);
            seats.Select(seat => seat.Id).Should().BeEquivalentTo(expectedSeats.Select(seat => seat.Id));
        }

        [Test]
        public async Task Test_GetActiveSeatsFromCinemaHallByCategoryAsync()
        {
            await _repositoryInfrastructure.DeleteRowAsync(1L);
            var seats =
                (await _repositoryInfrastructure.GetActiveSeatsFromCinemaHallByCategoryAsync(1L, new[] {1L, 2L}))
                .OrderBy(seat => seat.Id)
                .ToList();
            var rows = _repositoryHelper.Rows.Where(row => new[] {2L}.Contains(row.CategoryId)).Select(row => row.Id);
            var expectedSeats = _repositoryHelper.Seats
                .Where(seat => rows.Contains(seat.RowId))
                .OrderBy(row => row.Id)
                .ToList();

            seats.Should().HaveCount(expectedSeats.Count);
            seats.Select(seat => seat.Id).Should().BeEquivalentTo(expectedSeats.Select(seat => seat.Id));
        }

        [Test]
        public async Task Test_GetActiveRowCategoriesFromCinemaHall_InvalidCinemaHallId_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryInfrastructure.GetActiveRowCategoriesFromCinemaHallAsync(500L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_GetActiveRowCategoriesFromCinemaHall()
        {
            await _repositoryInfrastructure.DeleteRowCategoryAsync(1L);
            var rowCategories = (await _repositoryInfrastructure.GetActiveRowCategoriesFromCinemaHallAsync(1L))
                .OrderBy(category => category.Id)
                .ToList();

            var categories = _repositoryHelper.Rows
                .Where(row => row.CinemaHallId == 1L)
                .Select(_ => _.CategoryId);
            var expectedRows = _repositoryHelper.RowCategories
                .Where(category => category.Id != 1L && categories.Contains(category.Id))
                .OrderBy(category => category.Id)
                .ToList();

            rowCategories.Should().HaveCount(expectedRows.Count);
            rowCategories.Select(seat => seat.Id).Should().BeEquivalentTo(expectedRows.Select(seat => seat.Id));
        }

        [Test]
        public async Task Test_GetActiveRowCategories()
        {
            await _repositoryInfrastructure.DeleteRowCategoryAsync(1L);
            var rowCategories = (await _repositoryInfrastructure.GetActiveRowCategoriesAsync())
                .OrderBy(category => category.Id)
                .ToList();

            var expectedCategories = _repositoryHelper.RowCategories
                .Where(category => category.Id != 1L)
                .OrderBy(category => category.Id)
                .ToList();

            rowCategories.Should().HaveCount(expectedCategories.Count);
            rowCategories.Select(seat => seat.Id).Should().BeEquivalentTo(expectedCategories.Select(seat => seat.Id));
        }

        [Test]
        public async Task Test_AddCinemaHall_Duplicate_Should_Throw()
        {
            var cinemaHall = (await _repositoryInfrastructure.GetCinemaHallsAsync()).First(hall => hall.Id == 1L);
            Func<Task> execution = async () => await _repositoryInfrastructure.AddCinemaHallAsync(cinemaHall);
            await execution.Should().ThrowAsync<DuplicateEntryException>();
        }

        [Test]
        public async Task Test_AddCinemaHall_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.AddCinemaHallAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_AddCinemaHall()
        {
            var result = await _repositoryInfrastructure.AddCinemaHallAsync(new CinemaHall {Label = "New room"});

            result.Should().BeGreaterThan(0L);

            var cinemaHalls = (await _repositoryInfrastructure.GetCinemaHallsAsync()).ToList();

            cinemaHalls.Should().HaveCount(_repositoryHelper.CinemaHalls.Count() + 1);
            cinemaHalls.Select(hall => hall.Id).Should().Contain(result);
        }

        [Test]
        public async Task Test_AddRow_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.AddRowAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_AddRow_Duplicate_Should_Throw()
        {
            var row = (await _repositoryInfrastructure.GetRowsFromCinemaHallAsync(1L)).First(row => row.Id == 1L);
            Func<Task> execution = async () => await _repositoryInfrastructure.AddRowAsync(row);
            await execution.Should().ThrowAsync<DuplicateEntryException>();
        }

        [Test]
        public async Task Test_AddRow_With_Invalid_CinemaHallId_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryInfrastructure.AddRowAsync(new Row {Number = 1, CategoryId = 1L, CinemaHallId = 500L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddRow_With_Invalid_CategoryId_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryInfrastructure.AddRowAsync(new Row {Number = 1, CategoryId = 500L, CinemaHallId = 1L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddRow_With_Invalid_CinemaHallObject_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryInfrastructure.AddRowAsync(new Row
                    {Number = 1, CategoryId = 1L, CinemaHall = new CinemaHall {Id = 500L}});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddRow_With_Invalid_CategoryObject_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryInfrastructure.AddRowAsync(new Row
                    {Number = 1, Category = new RowCategory {Id = 500L}, CinemaHallId = 1L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddRow_With_No_CinemaHall_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryInfrastructure.AddRowAsync(new Row {Number = 1, CategoryId = 1L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddRow_With_Not_Category_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryInfrastructure.AddRowAsync(new Row
                    {Number = 1, Category = new RowCategory {Id = 500L}, CinemaHallId = 1L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddRow()
        {
            var result =
                await _repositoryInfrastructure.AddRowAsync(new Row {CategoryId = 1L, CinemaHallId = 1L, Number = 150});

            result.Should().BeGreaterThan(0L);

            var rows = (await _repositoryInfrastructure.GetRowsFromCinemaHallAsync(1L)).ToList();

            rows.Should().HaveCount(_repositoryHelper.Rows.Count() + 1);
            rows.Select(row => row.Id).Should().Contain(result);
            rows.First(row => row.Id == result).Number.Should().Be(150);
        }

        [Test]
        public async Task Test_AddRowCategory_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.AddRowCategoryAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_AddRowCategory_Duplicate_Should_Throw()
        {
            var rowCategory =
                (await _repositoryInfrastructure.GetActiveRowCategoriesFromCinemaHallAsync(1L)).First(category =>
                    category.Id == 1L);
            Func<Task> execution = async () => await _repositoryInfrastructure.AddRowCategoryAsync(rowCategory);
            await execution.Should().ThrowAsync<DuplicateEntryException>();
        }

        [Test]
        public async Task Test_AddRowCategory()
        {
            var result =
                await _repositoryInfrastructure.AddRowCategoryAsync(new RowCategory {Name = "Super Seat", PriceFactor = 2});

            result.Should().BeGreaterThan(0L);

            var rowCategories = (await _repositoryInfrastructure.GetActiveRowCategoriesAsync()).ToList();
            var expectedCategories = _repositoryHelper.RowCategories.ToList();

            rowCategories.Should().HaveCount(expectedCategories.Count + 1);
            rowCategories.Select(category => category.Id).Should().Contain(result);
        }
        
        [Test]
        public async Task Test_AddSeat_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.AddSeatAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_AddSeat_Duplicate_Should_Throw()
        {
            var seat =
                (await _repositoryInfrastructure.GetActiveSeatsWithRowAndCategoryAsync(1L))
                .First(seat => seat.Id == 1L);
            Func<Task> execution = async () => await _repositoryInfrastructure.AddSeatAsync(seat);
            await execution.Should().ThrowAsync<DuplicateEntryException>();
        }

        [Test]
        public async Task Test_AddSeat_With_Invalid_RowId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.AddSeatAsync(new Seat
            {
                LayoutColumn = 1, LayoutRow = 2, Locked = false, RowId = 500L
            });
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddSeat_With_Invalid_RowObject_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.AddSeatAsync(new Seat
            {
                LayoutColumn = 1,
                LayoutRow = 2,
                Locked = false,
                Row = new Row {Id = 500L}
            });
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddSeat_With_No_Row_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.AddSeatAsync(new Seat
            {
                LayoutColumn = 1,
                LayoutRow = 2,
                Locked = false
            });
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddSeat()
        {
            var result = await _repositoryInfrastructure.AddSeatAsync(new Seat
            {
                LayoutColumn = 1,
                LayoutRow = 2,
                Locked = true,
                Row = new Row {Id = 1L}
            });

            result.Should().BeGreaterThan(0L);

            var seats = (await _repositoryInfrastructure.GetSeatsWithRowAndCategoryAsync(1L)).ToList();

            seats.Should().HaveCount(_repositoryHelper.Seats.Count() + 1);
            seats.Select(seat => seat.Id).Should().Contain(result);

            var seat = seats.First(seat => seat.Id == result);
            seat.LayoutColumn.Should().Be(1);
            seat.LayoutRow.Should().Be(2);
            seat.Locked.Should().Be(true);
        }

        [Test]
        public async Task Test_DeleteCinemaHall_WithInvalid_Id_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.DeleteCinemaHallAsync(500L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_DeleteCinemaHall()
        {
            var result = await _repositoryInfrastructure.DeleteCinemaHallAsync(1L);
            result.Should().Be(1);

            var cinemaHalls = (await _repositoryInfrastructure.GetActiveCinemaHallsAsync()).ToList();

            cinemaHalls.Should().HaveCount(_repositoryHelper.CinemaHalls.Count() - 1);
            cinemaHalls.Select(_ => _.Id).Should().NotContain(1L);
        }

        [Test]
        public async Task Test_DeleteCinemaHall_DeleteAlso_Rows()
        {
            await _repositoryInfrastructure.DeleteCinemaHallAsync(1L);
            var rows =
                (await _repositoryInfrastructure.GetActiveRowCategoriesFromCinemaHallAsync(1L)).ToList();

            rows.Should().HaveCount(0);
        }

        [Test]
        public async Task Test_DeleteCinemaHall_Filter_RowCategories()
        {
            await _repositoryInfrastructure.DeleteCinemaHallAsync(1L);
            var rows =
                (await _repositoryInfrastructure.GetActiveRowCategoriesFromCinemaHallAsync(1L)).ToList();
            
            rows.Should().HaveCount(0);
        }

        [Test]
        public async Task Test_DeleteCinemaHall_DeleteAlso_Seats()
        {
            await _repositoryInfrastructure.DeleteCinemaHallAsync(1L);
            var seats = (await _repositoryInfrastructure.GetActiveSeatsWithRowAndCategoryAsync(1L)).ToList();

            seats.Should().HaveCount(0);
        }

        [Test]
        public async Task Test_DeleteRow_WithInvalid_Id_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.DeleteRowAsync(500L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_DeleteRow()
        {
            var result = await _repositoryInfrastructure.DeleteRowAsync(1L);
            result.Should().Be(1);

            var rows = (await _repositoryInfrastructure.GetActiveSeatsWithRowAndCategoryAsync(1L))
                .Select(_ => _.RowId)
                .Distinct()
                .ToList();

            rows.Should().HaveCount(_repositoryHelper.Rows.Count(_ => _.CinemaHallId == 1L) - 1);
            rows.Should().NotContain(1L);
        }

        [Test]
        public async Task Test_DeleteRow_DeleteAlso_Seats()
        {
            var result = await _repositoryInfrastructure.DeleteRowAsync(1L);
            result.Should().Be(1);

            var seats = (await _repositoryInfrastructure.GetActiveSeatsWithRowAndCategoryAsync(1L)).ToList();
            var expected = _repositoryHelper.Seats.Count(_ => _.RowId != 1L);
            seats.Should().HaveCount(expected);
        }

        [Test]
        public async Task Test_DeleteRowCategory_WithInvalid_Id_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.DeleteRowCategoryAsync(500L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_DeleteRowCategory()
        {
            var result = await _repositoryInfrastructure.DeleteRowCategoryAsync(1L);
            result.Should().Be(1);

            var rowCategories = (await _repositoryInfrastructure.GetActiveRowCategoriesAsync()).ToList();

            rowCategories.Should().HaveCount(_repositoryHelper.RowCategories.Count() - 1);
            rowCategories.Select(_ => _.Id).Should().NotContain(1L);
        }

        [Test]
        public async Task Test_DeleteSeat_WithInvalid_Id_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.DeleteSeatAsync(500L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_DeleteSeat()
        {
            var result = await _repositoryInfrastructure.DeleteSeatAsync(1L);
            result.Should().Be(1);

            var seats = (await _repositoryInfrastructure.GetActiveSeatsWithRowAndCategoryAsync(1L)).ToList();

            var rows = _repositoryHelper.Rows.Where(_ => _.CinemaHallId == 1L).Select(_ => _.Id).Distinct();
            seats.Should().HaveCount(_repositoryHelper.Seats.Count(_ => rows.Contains(_.RowId)) - 1);
            seats.Any(s => s.Id == 1L).Should().BeFalse();
        }
        
        [Test]
        public async Task Test_UpdateCinemaHall_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.UpdateCinemaHallAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_UpdateCinemaHall_InvalidId_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryInfrastructure.UpdateCinemaHallAsync(new CinemaHall {Id = 500L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateCinemaHall()
        {
            var hall = (await _repositoryInfrastructure.GetActiveCinemaHallsAsync()).First(_ => _.Id == 1L);
            hall.Label = "Updated hall";

            var result = await _repositoryInfrastructure.UpdateCinemaHallAsync(hall);
            result.Should().Be(1);

            var resultHalls = (await _repositoryInfrastructure.GetCinemaHallsAsync()).ToList();
            resultHalls.Select(_ => _.Id).Should().Contain(1L);
            resultHalls.First(_ => _.Id == 1L).Equals(hall).Should().BeTrue();
        }

        [Test]
        public async Task Test_UpdateRow_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.UpdateRowAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_UpdateRow_InvalidId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.UpdateRowAsync(new Row {Id = 500L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateRow()
        {
            var row = (await _repositoryInfrastructure.GetRowsFromCinemaHallAsync(1L)).First(_ => _.Id == 1L);
            row.CinemaHallId = 2L;
            row.CategoryId = 4L;
            row.Number = 150;

            var result = await _repositoryInfrastructure.UpdateRowAsync(row);
            result.Should().Be(1);

            var resultRows = (await _repositoryInfrastructure.GetRowsFromCinemaHallAsync(2L)).ToList();
            resultRows.Select(_ => _.Id).Should().Contain(1L);
            resultRows.First(_ => _.Id == 1L).Equals(row).Should().BeTrue();
        }

        [Test]
        public async Task Test_UpdateRowCategory_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.UpdateRowCategoryAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_UpdateRowCategory_InvalidId_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryInfrastructure.UpdateRowCategoryAsync(new RowCategory {Id = 500L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateRowCategory()
        {
            var rowCategory = (await _repositoryInfrastructure.GetActiveRowCategoriesAsync()).First(_ => _.Id == 1L);
            rowCategory.Name = "Best";
            rowCategory.PriceFactor = 5;

            var result = await _repositoryInfrastructure.UpdateRowCategoryAsync(rowCategory);
            result.Should().Be(1);

            var resultRowCategories = (await _repositoryInfrastructure.GetActiveRowCategoriesAsync()).ToList();
            resultRowCategories.Select(_ => _.Id).Should().Contain(1L);
            resultRowCategories.First(_ => _.Id == 1L).Equals(rowCategory).Should().BeTrue();
        }

        [Test]
        public async Task Test_UpdateSeat_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.UpdateSeatAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_UpdateSeat_InvalidId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryInfrastructure.UpdateSeatAsync(new Seat {Id = 500L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateSeat()
        {
            var seat = (await _repositoryInfrastructure.GetSeatsWithRowAndCategoryAsync(1L)).First(_ => _.Id == 1L);
            seat.RowId = 3L;
            seat.LayoutColumn = 150;
            seat.LayoutRow = 75;
            seat.Locked = true;

            var result = await _repositoryInfrastructure.UpdateSeatAsync(seat);
            result.Should().Be(1);

            var resultSeat = (await _repositoryInfrastructure.GetSeatsWithRowAndCategoryAsync(1L)).ToList();
            resultSeat.Select(_ => _.Id).Should().Contain(1L);
            resultSeat.First(_ => _.Id == 1L).Equals(seat).Should().BeTrue();
        }

        [Test]
        public async Task GetCinemaHallByIdAsync_ShouldReturnCinemaHall()
        {
            const long cinemaHallId = 1L;
            var cinemaHall = await _repositoryInfrastructure.GetCinemaHallByIdAsync(cinemaHallId);
            cinemaHall.Should().NotBeNull();
            cinemaHall.Id.Should().Be(cinemaHallId);
            cinemaHall.Label.Should().Be("Apollo 1");
        }


        [Test]
        public async Task GetCinemaHallByIdAsync_ShouldReturnNull()
        {
            const long cinemaHallId = 99999L;
            Func<Task> getCinemaHallById = async () => await _repositoryInfrastructure.GetCinemaHallByIdAsync(cinemaHallId);
            await getCinemaHallById.Should().ThrowExactlyAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task GetSeatByIdAsync_ShouldReturnSeat()
        {
            const long seatId = 13L;
            var seat = await _repositoryInfrastructure.GetSeatByIdAsync(seatId);
            seat.Should().NotBeNull();
            seat.Id.Should().Be(seatId);
        }


        [Test]
        public async Task GetSeatByIdAsync_ShouldReturnNull()
        {
            const long seatId = 3333L;
            Func<Task> getSeatByIdAsync = async () => await _repositoryInfrastructure.GetSeatByIdAsync(seatId);
            await getSeatByIdAsync.Should().ThrowExactlyAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task GetRowByIdAsync_ShouldReturnRow()
        {
            const long rowId = 1L;
            var row = await _repositoryInfrastructure.GetRowByIdAsync(rowId);
            row.Should().NotBeNull();
            row.Id.Should().Be(rowId);
        }

        [Test]
        public async Task GetRowByIdAsync_ShouldReturnNull()
        {
            const long rowId = 1337L;
            Func<Task> getRowByIdAsync = async () => await _repositoryInfrastructure.GetRowByIdAsync(rowId);
            await getRowByIdAsync.Should().ThrowExactlyAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task GetRowCategoryByIdAsync_ShouldReturnNull()
        {
            const long rowCategoryId = 112365L;
            Func<Task> getRowCategoryByIdAsync = async () => await _repositoryInfrastructure.GetCinemaHallByIdAsync(rowCategoryId);
            await getRowCategoryByIdAsync.Should().ThrowExactlyAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task GetRowCategoryByIdAsync_ShouldReturnRowCategory()
        {
            const long rowCategoryId = 1L;
            var rowCategory = await _repositoryInfrastructure.GetRowCategoryByIdAsync(rowCategoryId);
            rowCategory.Should().NotBeNull();
            rowCategory.Name.Should().Be("Silver");
            rowCategory.PriceFactor.Should().Be(1.0);
        }
    }
}