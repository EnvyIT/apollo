using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Core.Exception;
using Apollo.Core.Interfaces;
using Apollo.Domain.Entity;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using static Apollo.Core.Dto.Mapper;

namespace Apollo.Core.Test
{
   public  class InfrastructureServiceTest
    {
        private MockingHelper _mockingHelper;
        private IInfrastructureService _infrastructureService;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _mockingHelper = new MockingHelper();
            _infrastructureService = _mockingHelper.ServiceFactory.Object.CreateInfrastructureService();
        }


        [Test]
        public async Task GetAllCinemaHalls_ShouldReturnAllMappedCinemaHalls()
        {
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetCinemaHallsAsync())
                .ReturnsAsync(CreateCinemaHalls);

            var result = (await _infrastructureService.GetCinemaHallsAsync()).ToList();

            result.Should().HaveCount(2);
            result[0].Label.Should().Be("Apollo 1");
            result[1].Label.Should().Be("Apollo 2");
        }


        [Test]
        public async Task GetActiveCinemaHalls_ShouldReturnAllMappedCinemaHalls()
        {
            var singleCinemaHall = CreateCinemaHalls().Where(c => c.Id == 1L);
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetActiveCinemaHallsAsync())
                .ReturnsAsync(singleCinemaHall);

            var result = (await _infrastructureService.GetActiveCinemaHallsAsync()).ToList();

            result.Should().HaveCount(1);
            result[0].Label.Should().Be("Apollo 1");
        }

        [Test]
        public async Task GetSeatsWithRowAndCategory_ShouldReturnedMappedSeatsRowsAndCategories()
        {
            var seatWithId13 = CreateSeats().Where(s => s.Id == 13L);
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(13L))
                .ReturnsAsync(seatWithId13);

            var result = (await _infrastructureService.GetSeatsWithRowAndCategoryAsync(13L)).ToList();

            result.Should().HaveCount(1);
            result[0].Id.Should().Be(13L);
            result[0].Number.Should().Be(15);
            result[0].LayoutColumn.Should().Be(6);
            result[0].LayoutRow.Should().Be(1);
            result[0].State.Should().Be(SeatState.Free);
            result[0].RowId.Should().Be(2L);
            result[0].Row.Should().NotBeNull();
            result[0].Row.Id.Should().Be(2L);
            result[0].Row.Category.Should().NotBeNull();
            result[0].Row.Category.Id.Should().Be(2L);
            result[0].Row.Category.Name.Should().Be("Gold");
            result[0].Row.Category.PriceFactor.Should().Be(1.25);
        }

        [Test]
        public async Task GetActiveSeatsWithRowAndCategory_ShouldReturnedMappedSeatsRowsAndCategories()
        {
            var seatWithId13 = CreateSeats().Where(s => s.Id == 13L);
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetActiveSeatsWithRowAndCategoryAsync(13L))
                .ReturnsAsync(seatWithId13);

            var result = (await _infrastructureService.GetActiveSeatsWithRowAndCategoryAsync(13L)).ToList();

            result.Should().HaveCount(1);
            result[0].Id.Should().Be(13L);
            result[0].Number.Should().Be(15);
            result[0].LayoutColumn.Should().Be(6);
            result[0].LayoutRow.Should().Be(1);
            result[0].State.Should().Be(SeatState.Free);
            result[0].RowId.Should().Be(2L);
            result[0].Row.Should().NotBeNull();
            result[0].Row.Id.Should().Be(2L);
            result[0].Row.Category.Should().NotBeNull();
            result[0].Row.Category.Id.Should().Be(2L);
            result[0].Row.Category.Name.Should().Be("Gold");
            result[0].Row.Category.PriceFactor.Should().Be(1.25);
        }

 
        [Test]
        public async Task GetRowsFromCinemaHallAsync_ShouldReturnedMappedRows()
        {
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetRowsFromCinemaHallAsync(1L))
                .ReturnsAsync(CreateRows);

            var result = (await _infrastructureService.GetRowsFromCinemaHallAsync(1L)).ToList();

            result.Should().HaveCount(3);
            result[0].Id.Should().Be(1L);
            result[0].CinemaHallId.Should().Be(1L);
            result[0].Number.Should().Be(1);
            result[0].CategoryId.Should().Be(1L);
            result[0].Category.Should().NotBeNull();

            result[1].Id.Should().Be(2L);
            result[1].CinemaHallId.Should().Be(1L);
            result[1].Number.Should().Be(2);
            result[1].CategoryId.Should().Be(2L);
            result[1].Category.Should().NotBeNull();

            result[2].Id.Should().Be(3L);
            result[2].CinemaHallId.Should().Be(1L);
            result[2].Number.Should().Be(3);
            result[2].CategoryId.Should().Be(4L);
            result[2].Category.Should().NotBeNull();
        }

        [Test]
        public async Task GetActiveSeatsFromCinemaHallByCategoryAsync_ShouldReturnedMappedSeats()
        {
            var seats = CreateSeats().Where(s => s.Row.CategoryId == 2L && s.Row.CinemaHallId == 1L);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetActiveSeatsFromCinemaHallByCategoryAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(seats);

            var result = (await _infrastructureService.GetActiveSeatsFromCinemaHallByCategoryAsync(1L, 2L)).ToList();

            result.Should().HaveCount(8);
            result[4].Id.Should().Be(11L);
            result[4].Number.Should().Be(13);
            result[4].LayoutColumn.Should().Be(4);
            result[4].LayoutRow.Should().Be(1);
            result[4].State.Should().Be(SeatState.Free);
            result[4].RowId.Should().Be(2L);
            result[4].Row.Should().NotBeNull();
            result[4].Row.Id.Should().Be(2L);
            result[4].Row.Category.Should().NotBeNull();
            result[4].Row.Category.Id.Should().Be(2L);
            result[4].Row.Category.Name.Should().Be("Gold");
            result[4].Row.Category.PriceFactor.Should().Be(1.25);
        }

        [Test]
        public async Task GetActiveSeatsFromCinemaHallByCategoriesAsync_ShouldReturnedMappedSeats()
        {
            var seats = CreateSeats().Where(s => s.Row.CategoryId == 2L && s.Row.CinemaHallId == 1L);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetActiveSeatsFromCinemaHallByCategoryAsync(It.IsAny<long>(), It.IsAny<IEnumerable<long>>()))
                .ReturnsAsync(seats);

            var result = (await _infrastructureService.GetActiveSeatsFromCinemaHallByCategoriesAsync(1L, new List<long>{ 2L , 3L })).ToList();

            result.Should().HaveCount(8);
            result[7].Id.Should().Be(14L);
            result[7].Number.Should().Be(16);
            result[7].LayoutColumn.Should().Be(7);
            result[7].LayoutRow.Should().Be(1);
            result[7].State.Should().Be(SeatState.Free);
            result[7].RowId.Should().Be(2L);
            result[7].Row.Should().NotBeNull();
            result[7].Row.Id.Should().Be(2L);
            result[7].Row.Category.Should().NotBeNull();
            result[7].Row.Category.Id.Should().Be(2L);
            result[7].Row.Category.Name.Should().Be("Gold");
            result[7].Row.Category.PriceFactor.Should().Be(1.25);
        }

        [Test]
        public async Task GetActiveRowCategoriesFromCinemaHallAsync_ShouldReturnedMappedRowCategories()
        {
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetActiveRowCategoriesFromCinemaHallAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateRowCategories);

            var result = (await _infrastructureService.GetActiveRowCategoriesFromCinemaHallAsync(1L)).ToList();

            result.Should().HaveCount(4);
            result[0].Name.Should().Be("Silver");
            result[1].Name.Should().Be("Gold");
            result[2].Name.Should().Be("Platinum");
            result[3].Name.Should().Be("Diamond");
            result[0].PriceFactor.Should().Be(1.0);
            result[1].PriceFactor.Should().Be(1.25);
            result[2].PriceFactor.Should().Be(1.75);
            result[3].PriceFactor.Should().Be(2.0);
        }

        [Test]
        public async Task GetActiveRowCategoriesAsync_ShouldReturn2MappedRowCategories()
        {

            var activeCategories = CreateRowCategories().Where(c => c.Id < 3L);
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetActiveRowCategoriesAsync())
                .ReturnsAsync(activeCategories);

            var result = (await _infrastructureService.GetActiveRowCategoriesAsync()).ToList();

            result.Should().HaveCount(2);
            result[0].Name.Should().Be("Silver");
            result[1].Name.Should().Be("Gold");
            result[0].PriceFactor.Should().Be(1.0);
            result[1].PriceFactor.Should().Be(1.25);
        }


        [Test]
        public async Task AddCinemaHall()
        {
            var cinemaHallDto = CreateCinemaHalls().Select(Map).Single(m => m.Id == 1L);
            var cinemaHall = Map(cinemaHallDto);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.AddCinemaHallAsync(cinemaHall))
                .ReturnsAsync(1L);

            _mockingHelper.RepositoryInfrastructure.Setup(_ => _.GetCinemaHallByIdAsync(1L))
                .ReturnsAsync(cinemaHall);


            var result = await _infrastructureService.AddCinemaHallAsync(cinemaHallDto);
            result.Should().BeEquivalentTo(cinemaHallDto);
        }

        [Test]
        public async Task AddRowAsync()
        {
            var rowDto = CreateRows().Select(Map).Single(m => m.Id == 1L);
            var row = Map(rowDto);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.AddRowAsync(row))
                .ReturnsAsync(1L);

            _mockingHelper.RepositoryInfrastructure.Setup(_ => _.GetRowByIdAsync(1L))
                .ReturnsAsync(row);

            var result = await _infrastructureService.AddRowAsync(rowDto);
            result.Should().BeEquivalentTo(rowDto);
        }

        [Test]
        public async Task AddRowWithDuplicateLayoutAsync()
        {
            var rowDto = CreateRows().Select(Map).Single(m => m.Id == 1L);
            var row = Map(rowDto);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.AddRowAsync(row))
                .ReturnsAsync(1L);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetRowsFromCinemaHallAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateRows);

            _mockingHelper.RepositoryInfrastructure.Setup(_ => _.GetRowByIdAsync(1L))
                .ReturnsAsync(row);

            Func<Task<RowDto>> addRowAsync = async () => await _infrastructureService.AddRowAsync(rowDto);
            await addRowAsync.Should().ThrowExactlyAsync<PersistenceException>();
        }


        [Test]
        public async Task AddRowCategoryAsync()
        {
            var rowCategoryDto = CreateRowCategories().Select(Map).Single(m => m.Id == 1L);
            var rowCategory = Map(rowCategoryDto);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.AddRowCategoryAsync(rowCategory))
                .ReturnsAsync(1L);

            _mockingHelper.RepositoryInfrastructure.Setup(_ => _.GetRowCategoryByIdAsync(1L))
                .ReturnsAsync(rowCategory);

            var result = await _infrastructureService.AddRowCategoryAsync(rowCategoryDto);
            result.Should().BeEquivalentTo(rowCategoryDto);
        }


        [Test]
        public async Task AddSeatAsync()
        {
            var seatDto = CreateSeats().Select(Map).Single(m => m.Id == 1L);
            var seat = Map(seatDto);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.AddSeatAsync(seat))
                .ReturnsAsync(1L);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetRowByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateRows().First());

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetCinemaHallByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateCinemaHalls().First());

            _mockingHelper.RepositoryInfrastructure.Setup(_ => _.GetSeatByIdAsync(1L))
                .ReturnsAsync(seat);

            var result = await _infrastructureService.AddSeatAsync(seatDto);
            result.Should().BeEquivalentTo(seatDto);
        }

        [Test]
        public async Task AddSeatColumnOutOfRange_ShouldThrowArgumentOutOfRangeExceptionAsync()
        {
            var seatDto = CreateSeats().Select(Map).Single(m => m.Id == 1L);
            seatDto.LayoutColumn = 100;
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetCinemaHallByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateCinemaHalls().First());

            Func<Task> addSeatAsync = async () => await _infrastructureService.AddSeatAsync(seatDto);
            await addSeatAsync.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>();
        }

        [Test]
        public async Task AddSeatRowOutOfRange_ShouldThrowArgumentOutOfRangeExceptionAsync()
        {
            var seatDto = CreateSeats().Select(Map).Single(m => m.Id == 1L);
            seatDto.LayoutRow = 100;
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetCinemaHallByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateCinemaHalls().First());

            Func<Task> addSeatAsync = async () => await _infrastructureService.AddSeatAsync(seatDto);
            await addSeatAsync.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>();
        }

        [Test]
        public async Task AddSeatWithDuplicateLayoutAsync()
        {
            var seatDto = CreateSeats().Select(Map).Single(m => m.Id == 1L);
            var seat = Map(seatDto);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.AddSeatAsync(seat))
                .ReturnsAsync(1L);


            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetRowByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateRows().First());


            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats);

            _mockingHelper.RepositoryInfrastructure.Setup(_ => _.GetSeatByIdAsync(1L))
                .ReturnsAsync(seat);

            Func<Task<SeatDto>> addSeatAsync = async () => await _infrastructureService.AddSeatAsync(seatDto);
            await addSeatAsync.Should().ThrowExactlyAsync<PersistenceException>();
        }

        [Test]
        public async Task DeleteCinemaHallAsync()
        {
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.DeleteCinemaHallAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetCinemaHallByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateCinemaHalls().First());

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats().Take(3));

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetRowByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateRows().First());

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.DeleteRowAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.DeleteSeatAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            var result = await _infrastructureService.DeleteCinemaHallAsync(1L);
            result.Should().BeTrue();
        }

        [Test]
        public async Task DeleteCinemaHall_CinemaHallNull_ShouldReturnFalse()
        {
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.DeleteCinemaHallAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetCinemaHallByIdAsync(It.IsAny<long>()))
                .ReturnsAsync((CinemaHall)null);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats().Take(3));

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetRowByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateRows().First());

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.DeleteRowAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.DeleteSeatAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            var result = await _infrastructureService.DeleteCinemaHallAsync(1L);
            result.Should().BeFalse();
        }

        [Test]
        public async Task DeleteRowAsync()
        {
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats().Take(3));

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetRowByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateRows().First());

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.DeleteRowAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.DeleteSeatAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            var result = await _infrastructureService.DeleteRowAsync(1L);
            result.Should().BeTrue();
        }

        [Test]
        public async Task DeleteRow_RowNull_ShouldReturnFalse()
        {
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats().Take(3));

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetRowByIdAsync(It.IsAny<long>()))
                .ReturnsAsync((Row)null);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.DeleteRowAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.DeleteSeatAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            var result = await _infrastructureService.DeleteRowAsync(1L);
            result.Should().BeFalse();
        }

        [Test]
        public async Task DeleteRowCategoryAsync()
        {
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.DeleteRowCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            var result = await _infrastructureService.DeleteRowCategoryAsync(1L);
            result.Should().BeTrue();
        }

        [Test]
        public async Task DeleteSeatAsync()
        {
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.DeleteSeatAsync(It.IsAny<long>()))
                .ReturnsAsync(1);

            var result = await _infrastructureService.DeleteSeatAsync(1L);
            result.Should().BeTrue();
        }

        [Test]
        public async Task UpdateCinemaHallAsync()
        {
            var cinemaHall = CreateCinemaHalls().First();
            var cinemaHallDto = Map(cinemaHall);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.UpdateCinemaHallAsync(It.IsAny<CinemaHall>()))
                .ReturnsAsync(1);

            var result = await _infrastructureService.UpdateCinemaHallAsync(cinemaHallDto);
            result.Should().BeEquivalentTo(cinemaHallDto);
        }

        [Test]
        public async Task UpdateRowAsync()
        {
            var row = CreateRows().First();
            var rowDto = Map(row);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.UpdateRowAsync(It.IsAny<Row>()))
                .ReturnsAsync(1);

            var result = await _infrastructureService.UpdateRowAsync(rowDto);
            result.Should().BeEquivalentTo(rowDto);
        }

        [Test]
        public async Task UpdateRowCategoryAsync()
        {
            var rowCategory = CreateRowCategories().First();
            var rowCategoryDto = Map(rowCategory);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.UpdateRowCategoryAsync(It.IsAny<RowCategory>()))
                .ReturnsAsync(1);

            var result = await _infrastructureService.UpdateRowCategoryAsync(rowCategoryDto);
            result.Should().BeEquivalentTo(rowCategoryDto);
        }

        [Test]
        public async Task UpdateSeatAsync()
        {
            var seat = CreateSeats().First();
            var seatDto = Map(seat);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.UpdateSeatAsync(It.IsAny<Seat>()))
                .ReturnsAsync(1);

            var result = await _infrastructureService.UpdateSeatAsync(seatDto);
            result.Should().BeEquivalentTo(seatDto);
        }


        [Test]
        public async Task GetLayoutForCinemaHall_ShouldReturnFullLayout()
        {
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetActiveSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetCinemaHallByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateCinemaHalls().First());

            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetFreeSeatsAsync(It.IsAny<Schedule>()))
                .ReturnsAsync(CreateSeats().Where(s => !new List<long>{ 19L, 21L }.Contains(s.Id)));

            var schedule = new ScheduleDto
            {
                StartTime = DateTime.UtcNow,
                Id = 1L,
                CinemaHall = new CinemaHallDto
                {
                    Id = 1L,
                    Label = "Apollo 1"
                },
                CinemaHallId = 1L,
                Movie = new MovieDto
                {
                    Trailer = "",
                    Id = 1L,
                    Description = "A movie",
                    Duration = 123412,
                    Genre = new GenreDto
                    {
                        Id = 1L,
                        Name = "A Genre"
                    },
                    Image = new byte[] {1, 2, 3, 4, 5},
                    MovieActor = new MovieActorDto
                    {
                        Id = 1L
                    },
                    Title = "A Title"
                },
                MovieId = 1L,
                Price = 12.5M
            };

            var result = await _infrastructureService.GetLayout(schedule);
            result.GetLength(1).Should().Be(8);
            result.GetLength(0).Should().Be(3);

            AssertLayout(result);
        }

        [Test]
        public async Task GetLayoutForCinemaHall_ShouldReturnFullLayout2()
        {
            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetSeatsWithRowAndCategoryAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateSeats);

            _mockingHelper.RepositoryInfrastructure
                .Setup(_ => _.GetCinemaHallByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(CreateCinemaHalls().Last());

            _mockingHelper.RepositoryTicket
                .Setup(_ => _.GetFreeSeatsAsync(It.IsAny<Schedule>()))
                .ReturnsAsync(CreateSeats().Where(s => !new List<long> { 19L, 21L }.Contains(s.Id)));

            var schedule = new ScheduleDto
            {
                StartTime = DateTime.UtcNow,
                Id = 1L,
                CinemaHall = new CinemaHallDto
                {
                    Id = 1L,
                    Label = "Apollo 1"
                },
                CinemaHallId = 1L,
                Movie = new MovieDto
                {
                    Trailer = "",
                    Id = 1L,
                    Description = "A movie",
                    Duration = 123412,
                    Genre = new GenreDto
                    {
                        Id = 1L,
                        Name = "A Genre"
                    },
                    Image = new byte[] { 1, 2, 3, 4, 5 },
                    MovieActor = new MovieActorDto
                    {
                        Id = 1L
                    },
                    Title = "A Title"
                },
                MovieId = 1L,
                Price = 12.5M
            };

            var result = await _infrastructureService.GetLayout(schedule);
            result.GetLength(1).Should().Be(20);
            result.GetLength(0).Should().Be(20);
        }


        private static void AssertLayout(SeatDto[,] result)
        {
            const int offset = 2;
            var max = result.GetLength(1);
            for (var i = 0; i < result.GetLength(0); i++)
            {
                for (var j = 0; j < result.GetLength(1); j++)
                {
                    var calculatedId = i * max + (j + 1);
                    
                    if (new List<long> { 1L, 2L }.Contains(calculatedId))
                    {
                        result[i, j].Should().BeNull();
                    }
                    else
                    {
                        var id = calculatedId - offset;
                        result[i, j].Id.Should().Be(id);
                        if (new List<long> { 18L, 20L, 22L }.Contains(id))
                        {
                            result[i, j].State.Should().Be(SeatState.Locked);
                        }
                        else if (new List<long> { 19L, 21L }.Contains(id))
                        {
                            result[i, j].State.Should().Be(SeatState.Occupied);
                        }
                        else
                        {
                            result[i, j].State.Should().Be(SeatState.Free);
                        }
                        result[i, j].LayoutColumn.Should().Be(j);
                        result[i, j].LayoutRow.Should().Be(i);
                    }
                }
            }
        }

        private static IEnumerable<Seat> CreateSeats()
        {
            return new List<Seat>
            {
                // seat at (0,0) and (1, 0) are missing -> therefore they should be null in layout
                new Seat {Id = 1L,  Number = 3, LayoutColumn = 2, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 2L,  Number = 4, LayoutColumn = 3, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 3L,  Number = 5, LayoutColumn = 4, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 4L,  Number = 6, LayoutColumn = 5, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 5L,  Number = 7, LayoutColumn = 6, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},
                new Seat {Id = 6L,  Number = 8, LayoutColumn = 7, LayoutRow = 0, Locked = false, RowId = 1L, Row = new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}}},

                new Seat {Id = 7L,  Number = 9, LayoutColumn = 0, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 8L , Number = 10, LayoutColumn = 1, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 9L,  Number = 11, LayoutColumn = 2, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 10L,  Number = 12, LayoutColumn = 3, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 11L,  Number = 13, LayoutColumn = 4, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 12L,  Number = 14, LayoutColumn = 5, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 13L,  Number = 15, LayoutColumn = 6, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},
                new Seat {Id = 14L,  Number = 16, LayoutColumn = 7, LayoutRow = 1, Locked = false, RowId = 2L, Row = new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}}},

                new Seat {Id = 15L,  Number = 20, LayoutColumn = 0, LayoutRow = 2, Locked = false, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 16L , Number = 21, LayoutColumn = 1, LayoutRow = 2, Locked = false, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 17L,  Number = 22, LayoutColumn = 2, LayoutRow = 2, Locked = false, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 18L,  Number = 23, LayoutColumn = 3, LayoutRow = 2, Locked = true, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 19L,  Number = 24, LayoutColumn = 4, LayoutRow = 2, Locked = false, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 20L,  Number = 25, LayoutColumn = 5, LayoutRow = 2, Locked = true, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 21L,  Number = 26, LayoutColumn = 6, LayoutRow = 2, Locked = false, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
                new Seat {Id = 22L,  Number = 27, LayoutColumn = 7, LayoutRow = 2, Locked = true, RowId = 3L, Row = new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}},
            };
        }

        private static IEnumerable<Row> CreateRows()
        {
            return new List<Row>
            {
                new Row { Id = 1L, Number = 1, CategoryId = 1L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category = new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0}},
                new Row { Id = 2L, Number = 2, CategoryId = 2L, CinemaHallId = 1L, CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"} , Category = new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25}},
                new Row { Id = 3L, Number = 3, CategoryId = 4L, CinemaHallId = 1L , CinemaHall =   new CinemaHall {Id = 1L, Label = "Apollo 1"}, Category =  new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}}
            };

        }

        private static IEnumerable<RowCategory> CreateRowCategories()
        {
            return new List<RowCategory>
            {
                new RowCategory {Id = 1L,  Name = "Silver", PriceFactor = 1.0},
                new RowCategory {Id = 2L,  Name = "Gold", PriceFactor = 1.25},
                new RowCategory {Id = 3L,  Name = "Platinum", PriceFactor = 1.75},
                new RowCategory {Id = 4L,  Name = "Diamond", PriceFactor = 2.0}
            };
        }

        private static IEnumerable<CinemaHall> CreateCinemaHalls()
        {
            return new List<CinemaHall>
            {
                new CinemaHall {Id = 1L, Label = "Apollo 1", SizeColumn = 8, SizeRow = 3},
                new CinemaHall {Id = 2L, Label = "Apollo 2", SizeColumn = 20, SizeRow = 20}
            };
        }



    }
}
