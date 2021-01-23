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
    class ManageCinemaHallsTest : BaseApiTest
    {
        private const string BaseUrl = "infrastructure";
        private const string EndpointCinemaHall = "cinemahall";
        private const string EndpointCinemaHalls = "cinemahalls";

        private static readonly IApolloLogger<ManageMoviesTest> Logger =
            LoggerFactory.CreateLogger<ManageMoviesTest>();

        [Test]
        public async Task Test_Workflow_CinemaHalls_Valid()
        {
            Logger.Info("Load cinema halls ...");
            var response = await Get(BaseUrl, EndpointCinemaHalls);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var cinemaHalls = JsonConvert.DeserializeObject<IEnumerable<CinemaHallDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();
            cinemaHalls.Should().HaveCount(DataSeeder.CinemaHalls.Count);
            Logger.Info(cinemaHalls.ToPrettyString());

            var newHall = new CinemaHallDto {Label = "New Hall", SizeColumn = 10, SizeRow = 5};
            var updateHallReservations = cinemaHalls.First(c =>
                DataSeeder.Reservations
                    .Select(r => DataSeeder.Schedules.First(s => r.ScheduleId == s.Id).CinemaHallId)
                    .Contains(c.Id));
            var updateHall = cinemaHalls.First(c =>
                !DataSeeder.Reservations
                    .Select(r => DataSeeder.Schedules.First(s => r.ScheduleId == s.Id).CinemaHallId)
                    .Contains(c.Id));
            updateHall.Label = "Renovated Hall";
            var deleteReferencedHall = DataSeeder.CinemaHalls
                .First(c => DataSeeder.Rows.Select(r => r.CinemaHallId).Contains(c.Id)).Id;
            var deletedHall = DataSeeder.CinemaHalls
                .Last(c => !DataSeeder.Rows.Select(r => r.CinemaHallId).Contains(c.Id))
                .Id;

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info($"Add new cinema hall: ${newHall.ToPrettyString()} ...");
            response = await Post(BaseUrl, EndpointCinemaHall, newHall);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Created);
            var newId = await GetCreatedIdAsync(response);

            Logger.Info($"Try update cinema hall with reservations: {updateHallReservations.ToPrettyString()} ...");
            response = await Put(BaseUrl, EndpointCinemaHall, updateHallReservations);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            Logger.Info($"Update cinema hall: ${updateHall.ToPrettyString()} ...");
            response = await Put(BaseUrl, EndpointCinemaHall, updateHall);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info($"Try delete referenced cinema hall: {deleteReferencedHall.ToPrettyString()} ...");
            response = await Delete(BaseUrl, $"{EndpointCinemaHall}/{deleteReferencedHall}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            Logger.Info($"Delete cinema hall: ${deletedHall.ToPrettyString()} ...");
            response = await Delete(BaseUrl, $"{EndpointCinemaHall}/{deletedHall}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info("Load cinema halls ...");
            response = await Get(BaseUrl, EndpointCinemaHalls);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            cinemaHalls = JsonConvert.DeserializeObject<IEnumerable<CinemaHallDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();

            var cinemaHall = cinemaHalls.Select(g => g.Id).ToList();
            cinemaHall.Should().Contain(newId);
            cinemaHall.Should().Contain(updateHallReservations.Id);
            cinemaHall.Should().Contain(updateHall.Id);
            cinemaHall.Should().Contain(deleteReferencedHall);
            cinemaHall.Should().NotContain(deletedHall);

            cinemaHalls.First(g => g.Id == newId).Label.Should().Be(newHall.Label);
            cinemaHalls.First(g => g.Id == updateHall.Id).Label.Should().Be(updateHall.Label);

            Logger.Info(cinemaHalls.ToPrettyString());
        }

        [Test]
        public async Task Test_Workflow_CinemaHalls_Invalid()
        {
            await Authenticate(AdminUser, AdminPassword);

            var response = await Delete(BaseUrl, $"{EndpointCinemaHall}/500");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointCinemaHall, new CinemaHallDto());
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointCinemaHall, new CinemaHallDto {Id = 500L});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointCinemaHall,
                new CinemaHallDto {Id = 500L, Label = "Test", SizeColumn = 10, SizeRow = 10});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Post(BaseUrl, EndpointCinemaHall, new CinemaHallDto());
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Post(BaseUrl, EndpointCinemaHall, new CinemaHallDto {Label = ""});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Post(BaseUrl, EndpointCinemaHall, new CinemaHallDto {Label = "A"});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Post(BaseUrl, EndpointCinemaHall,
                new CinemaHallDto {Label = "New Hall", SizeRow = 0, SizeColumn = 10});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Post(BaseUrl, EndpointCinemaHall,
                new CinemaHallDto {Label = "New Hall", SizeRow = 10, SizeColumn = 0});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Test_Workflow_RequestSingleCinemaHall_Valid()
        {
            var expectedHall = DataSeeder.CinemaHalls.First();
            var id = expectedHall.Id;

            Logger.Info($"Load single cinema hall with id {id} ...");
            var response = await Get(BaseUrl, $"{EndpointCinemaHall}/{id}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var cinemaHall = JsonConvert.DeserializeObject<CinemaHallDto>(await response.Content.ReadAsStringAsync());

            cinemaHall.Label.Should().Be(expectedHall.Label);
            cinemaHall.SizeRow.Should().Be(expectedHall.SizeRow);
            cinemaHall.SizeColumn.Should().Be(expectedHall.SizeColumn);

            Logger.Info(cinemaHall.ToPrettyString());
        }

        [Test]
        public async Task Test_Workflow_UpdateParallel_Users()
        {
            const string newTitle = "Renovated Hall";

            Logger.Info("Load cinema halls ...");
            var response = await Get(BaseUrl, EndpointCinemaHalls);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var cinemaHalls = JsonConvert.DeserializeObject<IEnumerable<CinemaHallDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();
            cinemaHalls.Should().HaveCount(DataSeeder.CinemaHalls.Count);
            Logger.Info(cinemaHalls.ToPrettyString());

            var cinemaHallWithoutReservations = cinemaHalls.First(c =>
                !DataSeeder.Reservations
                    .Select(r => DataSeeder.Schedules.First(s => r.ScheduleId == s.Id).CinemaHallId)
                    .Contains(c.Id));
            var cinemaHall = cinemaHalls.First(c => c.Id == cinemaHallWithoutReservations.Id);
            cinemaHall.Label = newTitle;

            await Authenticate(AdminUser, AdminPassword);

            response = await Put(BaseUrl, EndpointCinemaHall, cinemaHall);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info("Try to update outdated cinema hall ...");
            cinemaHall.Label = "Super Hall";
            response = await Put(BaseUrl, EndpointCinemaHall, cinemaHall);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            Logger.Info($"Load single cinema hall with id {cinemaHall.Id} ...");
            response = await Get(BaseUrl, $"{EndpointCinemaHall}/{cinemaHall.Id}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            cinemaHall = JsonConvert.DeserializeObject<CinemaHallDto>(await response.Content.ReadAsStringAsync());

            cinemaHall.Label.Should().Be(newTitle);

            Logger.Info(cinemaHall.ToPrettyString());
        }

        [Test]
        public async Task Test_Delete_WithoutAuthentication()
        {
            var id = DataSeeder.CinemaHalls.Last().Id;
            var response = await Delete(BaseUrl, $"{EndpointCinemaHall}/{id}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Test_Create_WithoutAuthentication()
        {
            var response = await Post(BaseUrl, EndpointCinemaHall, new object());
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Test_Update_WithoutAuthentication()
        {
            var response = await Put(BaseUrl, EndpointCinemaHall, new object());
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Unauthorized);
        }
    }
}