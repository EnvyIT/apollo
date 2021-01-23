using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Util;
using Apollo.Util.Logger;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Apollo.Api.Test
{
    class ManageSeatsTest : BaseApiTest
    {
        private const string BaseUrl = "infrastructure";
        private const string EndpointSeatLayout = "seatlayout";
        private const string EndpointLockSeat = "lockseat";
        private const string EndpointUnlockSeat = "unlockseat";

        private static readonly IApolloLogger<ManageMoviesTest> Logger =
            LoggerFactory.CreateLogger<ManageMoviesTest>();

        [Test]
        public async Task Test_Workflow_SeatLayout_Valid()
        {
            var cinemaHall = DataSeeder.CinemaHalls.Last();
            var cinemaHallId = cinemaHall.Id;
            var rows = DataSeeder.Rows.Where(r => r.CinemaHallId == cinemaHallId).ToList();
            var seats = DataSeeder.Seats.Where(s => rows.Select(r => r.Id).Contains(s.RowId));

            Logger.Info($"Load seat layout for cinema hall: {cinemaHall.ToPrettyString()} ...");
            var response = await Get(BaseUrl, $"{EndpointSeatLayout}/{cinemaHallId}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var seatLayout = JsonConvert.DeserializeObject<IEnumerable<SeatDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();
            seatLayout.Should().HaveCount(seats.Count());
            Logger.Info(seatLayout.ToPrettyString());

            IList<SeatDto> updatedLayout = new List<SeatDto>
            {
                new SeatDto
                {
                    Number = 120, RowId = rows.First().Id, LayoutColumn = cinemaHall.SizeColumn / 2,
                    LayoutRow = cinemaHall.SizeRow - 1
                },
                new SeatDto
                {
                    Number = 121, RowId = rows.Last().Id, LayoutColumn = cinemaHall.SizeColumn / 2 + 1,
                    LayoutRow = cinemaHall.SizeRow - 2
                }
            };
            var factor = seatLayout.Count / 3;
            for (int i = factor; i < seatLayout.Count; i++)
            {
                if (i < 2 * factor)
                {
                    seatLayout[i].Number = 1000 + i;
                }

                updatedLayout.Add(seatLayout[i]);
            }

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info($"Update seat layout: ${updatedLayout.ToPrettyString()} ...");
            response = await Put(BaseUrl, $"{EndpointSeatLayout}/{cinemaHallId}", updatedLayout);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info($"Load seat layout for cinema hall: {cinemaHall.ToPrettyString()} ...");
            response = await Get(BaseUrl, $"{EndpointSeatLayout}/{cinemaHallId}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            seatLayout = JsonConvert.DeserializeObject<IEnumerable<SeatDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();

            seatLayout.Should().HaveCount(updatedLayout.Count);
            var orderedResult = seatLayout.OrderBy(s => s.RowId).ThenBy(s => s.Number).ToList();
            var orderedLayout = updatedLayout.OrderBy(s => s.RowId).ThenBy(s => s.Number).ToList();

            for (int i = 0; i < orderedResult.Count; i++)
            {
                orderedResult[i].Number.Should().Be(orderedLayout[i].Number);
                orderedResult[i].LayoutColumn.Should().Be(orderedLayout[i].LayoutColumn);
                orderedResult[i].LayoutRow.Should().Be(orderedLayout[i].LayoutRow);
                orderedResult[i].RowId.Should().Be(orderedLayout[i].RowId);
            }

            Logger.Info(seatLayout.ToPrettyString());
        }

        [Test]
        public async Task Test_Workflow_SeatLayout_Locking_Valid()
        {
            var cinemaHall = DataSeeder.CinemaHalls.Last();
            var cinemaHallId = cinemaHall.Id;
            var rows = DataSeeder.Rows.Where(r => r.CinemaHallId == cinemaHallId).ToList();
            var seats = DataSeeder.Seats.Where(s => rows.Select(r => r.Id).Contains(s.RowId));

            Logger.Info($"Load seat layout for cinema hall: {cinemaHall.ToPrettyString()} ...");
            var response = await Get(BaseUrl, $"{EndpointSeatLayout}/{cinemaHallId}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var seatLayout = JsonConvert.DeserializeObject<IEnumerable<SeatDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();
            seatLayout.Should().HaveCount(seats.Count());
            Logger.Info(seatLayout.ToPrettyString());

            var lockSeat1 = seatLayout.ElementAt(0);
            var lockSeat2 = seatLayout.ElementAt(1);
            var unlockedSeat = seatLayout.ElementAt(2);

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info($"Lock seats with id: {lockSeat1.Id}, {lockSeat2.Id} ...");
            response = await Put<object>(BaseUrl, $"{EndpointLockSeat}/{lockSeat1.Id}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);
            response = await Put<object>(BaseUrl, $"{EndpointLockSeat}/{lockSeat2.Id}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info($"Unlock unlocked seat with id: {unlockedSeat.Id} ...");
            response = await Put<object>(BaseUrl, $"{EndpointUnlockSeat}/{unlockedSeat.Id}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info($"Unlock seat with id: {lockSeat1.Id} ...");
            response = await Put<object>(BaseUrl, $"{EndpointUnlockSeat}/{lockSeat1.Id}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info($"Load seat layout for cinema hall: {cinemaHall.ToPrettyString()} ...");
            response = await Get(BaseUrl, $"{EndpointSeatLayout}/{cinemaHallId}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            seatLayout = JsonConvert.DeserializeObject<IEnumerable<SeatDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();
            seatLayout.Should().HaveCount(seats.Count());

            seatLayout.First(s => s.Id == lockSeat1.Id).State.Should().Be(SeatState.Free);
            seatLayout.First(s => s.Id == lockSeat2.Id).State.Should().Be(SeatState.Locked);
            seatLayout.First(s => s.Id == unlockedSeat.Id).State.Should().Be(SeatState.Free);

            Logger.Info(seatLayout.ToPrettyString());
        }

        [Test]
        public async Task Test_Workflow_SeatLayout_Invalid()
        {
            await Authenticate(AdminUser, AdminPassword);

            var invalidCinemaHall = DataSeeder.CinemaHalls.First(c =>
                DataSeeder.Reservations
                    .Select(r => DataSeeder.Schedules.First(s => r.ScheduleId == s.Id).CinemaHallId)
                    .Contains(c.Id)).Id;
            var validCinemaHall = DataSeeder.CinemaHalls.First(c =>
                !DataSeeder.Reservations
                    .Select(r => DataSeeder.Schedules.First(s => r.ScheduleId == s.Id).CinemaHallId)
                    .Contains(c.Id)).Id;

            var response = await Put(BaseUrl, $"{EndpointSeatLayout}/{invalidCinemaHall}",
                new List<SeatDto>());
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, $"{EndpointSeatLayout}/{validCinemaHall}",
                new List<SeatDto> {new SeatDto {LayoutColumn = -1}});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, $"{EndpointSeatLayout}/{validCinemaHall}",
                new List<SeatDto> {new SeatDto {LayoutRow = -1}});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, $"{EndpointSeatLayout}/{validCinemaHall}",
                new List<SeatDto> {new SeatDto {Number = 1}});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, $"{EndpointSeatLayout}/{validCinemaHall}",
                new List<SeatDto> {new SeatDto {Number = -1}});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, $"{EndpointSeatLayout}/{validCinemaHall}",
                new List<SeatDto> {new SeatDto {RowId = 500}});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Test_Workflow_SeatLayout_Locking_Invalid()
        {
            await Authenticate(AdminUser, AdminPassword);

            var response = await Put<object>(BaseUrl, $"{EndpointLockSeat}/0");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put<object>(BaseUrl, $"{EndpointLockSeat}/500");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            var seatWithReservation = DataSeeder.Seats.First(s =>
                DataSeeder.Reservations.Select(r => DataSeeder.SeatReservations.First(sr => sr.ReservationId == r.Id))
                    .First().SeatId == s.Id);

            response = await Put<object>(BaseUrl, $"{EndpointLockSeat}/{seatWithReservation}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Test_Workflow_SeatLayout_Range_Invalid()
        {
            var cinemaHall = DataSeeder.CinemaHalls.Last();
            var cinemaHallId = cinemaHall.Id;
            var rows = DataSeeder.Rows.Where(r => r.CinemaHallId == cinemaHallId).ToList();
            var rowId = rows.First().Id;
            var seats = DataSeeder.Seats.Where(s => rows.Select(r => r.Id).Contains(s.RowId)).ToList();

            await Authenticate(AdminUser, AdminPassword);

            var layout = new List<SeatDto>
            {
                new SeatDto
                {
                    RowId = rowId, Number = 500, LayoutColumn = -1, LayoutRow = cinemaHall.SizeRow - 1
                }
            };
            Logger.Info($"Try update seat layout: ${layout.ToPrettyString()} ...");
            var response = await Put(BaseUrl, $"{EndpointSeatLayout}/{cinemaHallId}", layout);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            layout = new List<SeatDto>
            {
                new SeatDto
                {
                    RowId = rowId, Number = 500, LayoutColumn = cinemaHall.SizeRow - 1, LayoutRow = -1
                }
            };
            Logger.Info($"Try update seat layout: ${layout.ToPrettyString()} ...");
            response = await Put(BaseUrl, $"{EndpointSeatLayout}/{cinemaHallId}", layout);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            layout = new List<SeatDto>
            {
                new SeatDto
                {
                    RowId = rowId, Number = 500, LayoutColumn = cinemaHall.SizeColumn,
                    LayoutRow = cinemaHall.SizeRow - 1
                }
            };
            Logger.Info($"Try update seat layout: ${layout.ToPrettyString()} ...");
            response = await Put(BaseUrl, $"{EndpointSeatLayout}/{cinemaHallId}", layout);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            layout = new List<SeatDto>
            {
                new SeatDto
                {
                    RowId = rowId, Number = 500, LayoutColumn = cinemaHall.SizeRow - 1, LayoutRow = cinemaHall.SizeRow
                }
            };
            Logger.Info($"Try update seat layout: ${layout.ToPrettyString()} ...");
            response = await Put(BaseUrl, $"{EndpointSeatLayout}/{cinemaHallId}", layout);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            layout = new List<SeatDto>
            {
                Mapper.Map(seats.First()),
                new SeatDto
                {
                    RowId = rowId, Number = 500, LayoutColumn = seats.First().LayoutColumn,
                    LayoutRow = seats.First().LayoutRow
                }
            };
            Logger.Info($"Try update seat layout: ${layout.ToPrettyString()} ...");
            response = await Put(BaseUrl, $"{EndpointSeatLayout}/{cinemaHallId}", layout);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Test_Workflow_SeatLayout_UpdateParallel_Users()
        {
            const int newNumber1 = 4100;
            const int newNumber2 = 4200;

            var cinemaHall = DataSeeder.CinemaHalls.Last();
            var cinemaHallId = cinemaHall.Id;
            var rows = DataSeeder.Rows.Where(r => r.CinemaHallId == cinemaHallId).ToList();
            var seats = DataSeeder.Seats.Where(s => rows.Select(r => r.Id).Contains(s.RowId));

            Logger.Info($"Load seat layout for cinema hall: {cinemaHall.ToPrettyString()} ...");
            var response = await Get(BaseUrl, $"{EndpointSeatLayout}/{cinemaHallId}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var seatLayout = JsonConvert.DeserializeObject<IEnumerable<SeatDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();
            seatLayout.Should().HaveCount(seats.Count());
            Logger.Info(seatLayout.ToPrettyString());

            IList<SeatDto> updatedLayout = new List<SeatDto>
            {
                new SeatDto
                {
                    Number = newNumber1, RowId = rows.First().Id, LayoutColumn = cinemaHall.SizeColumn - 1,
                    LayoutRow = cinemaHall.SizeRow - 1
                }
            };
            seatLayout.First().Number = newNumber2;
            updatedLayout.Add(seatLayout.First());

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info("Update seat layout ...");
            response = await Put(BaseUrl, $"{EndpointSeatLayout}/{cinemaHallId}", updatedLayout);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info("Try to update outdated seat layout ...");
            updatedLayout.Last().Number = 5000;
            response = await Put(BaseUrl, $"{EndpointSeatLayout}/{cinemaHallId}", updatedLayout);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            Logger.Info($"Load seat layout for cinema hall: {cinemaHall.ToPrettyString()} ...");
            response = await Get(BaseUrl, $"{EndpointSeatLayout}/{cinemaHallId}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            seatLayout = JsonConvert.DeserializeObject<IEnumerable<SeatDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();
            seatLayout.Should().HaveCount(updatedLayout.Count);
            Logger.Info(seatLayout.ToPrettyString());

            var orderedLayout = seatLayout.OrderBy(s => s.Number).ToList();
            orderedLayout.First().Number.Should().Be(newNumber1);
            orderedLayout.Last().Number.Should().Be(newNumber2);

            Logger.Info(seatLayout.ToPrettyString());
        }

        [Test]
        public async Task Test_Update_WithoutAuthentication()
        {
            var id = DataSeeder.CinemaHalls.Last().Id;
            var response = await Put(BaseUrl, $"{EndpointSeatLayout}/{id}", new object());
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Unauthorized);
        }
    }
}