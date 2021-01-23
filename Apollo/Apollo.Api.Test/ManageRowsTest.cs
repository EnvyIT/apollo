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
    public class ManageRowsTest : BaseApiTest
    {
        private const string BaseUrl = "infrastructure";
        private const string EndpointCategory = "category";
        private const string EndpointCategories = "categories";
        private const string EndpointRow = "row";
        private const string EndpointRows = "rows";

        private static readonly IApolloLogger<ManageMoviesTest> Logger = LoggerFactory.CreateLogger<ManageMoviesTest>();

        [Test]
        public async Task Test_Workflow_RowCategory_Valid()
        {
            Logger.Info("Load row categories ...");
            var response = await Get(BaseUrl, EndpointCategories);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var categories = JsonConvert.DeserializeObject<IEnumerable<RowCategoryDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();
            categories.Should().HaveCount(DataSeeder.RowCategories.Count);
            Logger.Info(categories.ToPrettyString());

            var newCategory = new RowCategoryDto {Name = "New Category", PriceFactor = 1.85};
            var updateCategoryReferences =
                categories.First(c => DataSeeder.Rows.Select(r => r.CategoryId).Contains(c.Id));
            updateCategoryReferences.PriceFactor = 2.0;
            var updateCategory = categories.First(c => !DataSeeder.Rows.Select(r => r.CategoryId).Contains(c.Id));
            updateCategory.Name = "Updated category";
            var deleteCategoryReferences = categories
                .Last(c => DataSeeder.Rows.Select(r => r.CategoryId).Contains(c.Id))
                .Id;
            var deletedCategory = categories
                .Last(c => !DataSeeder.Rows.Select(r => r.CategoryId).Contains(c.Id))
                .Id;

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info($"Add new row category: {newCategory.ToPrettyString()} ...");
            response = await Post(BaseUrl, EndpointCategory, newCategory);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Created);
            var newId = await GetCreatedIdAsync(response);

            Logger.Info(
                $"Update row category with referenced seats: {updateCategoryReferences.ToPrettyString()} ...");
            response = await Put(BaseUrl, EndpointCategory, updateCategoryReferences);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info($"Update row category: ${updateCategory.ToPrettyString()} ...");
            response = await Put(BaseUrl, EndpointCategory, updateCategory);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info(
                $"Try delete row category with referenced seats: {deleteCategoryReferences} ...");
            response = await Delete(BaseUrl, $"{EndpointCategory}/{deleteCategoryReferences}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            Logger.Info($"Delete cinema hall: {deletedCategory} ...");
            response = await Delete(BaseUrl, $"{EndpointCategory}/{deletedCategory}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info("Load row categories ...");
            response = await Get(BaseUrl, EndpointCategories);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            categories = JsonConvert.DeserializeObject<IEnumerable<RowCategoryDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();

            var rowCategoryIds = categories.Select(g => g.Id).ToList();
            rowCategoryIds.Should().Contain(newId);
            rowCategoryIds.Should().Contain(updateCategoryReferences.Id);
            rowCategoryIds.Should().Contain(updateCategory.Id);
            rowCategoryIds.Should().Contain(deleteCategoryReferences);
            rowCategoryIds.Should().NotContain(deletedCategory);

            categories.First(g => g.Id == newId).Name.Should().Be(newCategory.Name);
            categories.First(g => g.Id == updateCategory.Id).Name.Should().Be(updateCategory.Name);
            categories.First(g => g.Id == updateCategoryReferences.Id).PriceFactor.Should()
                .Be(updateCategoryReferences.PriceFactor);

            Logger.Info(categories.ToPrettyString());
        }

        [Test]
        public async Task Test_Workflow_RowCategory_Invalid()
        {
            await Authenticate(AdminUser, AdminPassword);

            var response = await Delete(BaseUrl, $"{EndpointCategory}/500");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointCategory, new RowCategoryDto());
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointCategory, new RowCategoryDto {Id = 500L});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointCategory,
                new RowCategoryDto {Id = 500L, Name = "Category", PriceFactor = 1.5});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            var id = DataSeeder.RowCategories.Last().Id;

            response = await Put(BaseUrl, EndpointCategory,
                new RowCategoryDto {Id = id, Name = string.Empty, PriceFactor = 1.5});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointCategory,
                new RowCategoryDto {Id = id, Name = "Category", PriceFactor = 0.5});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointCategory,
                new RowCategoryDto {Id = id, Name = "Category", PriceFactor = 2.5});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Post(BaseUrl, EndpointCategory,
                new RowCategoryDto {Name = string.Empty, PriceFactor = 1.5});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Post(BaseUrl, EndpointCategory,
                new RowCategoryDto {Name = "Category", PriceFactor = 2.5});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Test_Workflow_RowCategory_UpdateParallel_Users()
        {
            const string newName = "Brand new category";

            Logger.Info("Load categories ...");
            var response = await Get(BaseUrl, EndpointCategories);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var categories = JsonConvert.DeserializeObject<IEnumerable<RowCategoryDto>>(
                await response.Content.ReadAsStringAsync()
            ).ToList();
            categories.Should().HaveCount(DataSeeder.RowCategories.Count);
            Logger.Info(categories.ToPrettyString());

            var category = categories.First();
            category.Name = newName;

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info("Update row category...");
            response = await Put(BaseUrl, EndpointCategory, category);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info("Try to update outdated row category...");
            category.Name = "Super premium";
            response = await Put(BaseUrl, EndpointCategory, category);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            Logger.Info($"Load single row category with id {category.Id} ...");
            response = await Get(BaseUrl, $"{EndpointCategory}/{category.Id}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            category = JsonConvert.DeserializeObject<RowCategoryDto>(await response.Content.ReadAsStringAsync());

            category.Name.Should().Be(newName);

            Logger.Info(category.ToPrettyString());
        }

        [Test]
        public async Task Test_Workflow_Row_WithReservations_Valid()
        {
            var cinemaHall = DataSeeder.CinemaHalls.First().Id;
            Logger.Info("Load rows ...");
            var response = await Get(BaseUrl, $"{EndpointRows}/{cinemaHall}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var rows = JsonConvert
                .DeserializeObject<IEnumerable<RowDto>>(await response.Content.ReadAsStringAsync()).ToList();
            rows.Should().HaveCount(DataSeeder.Rows.Count(r => r.CinemaHallId == cinemaHall));
            Logger.Info(rows.ToPrettyString());

            var defaultHall = DataSeeder.CinemaHalls.First().Id;
            var defaultCategory = DataSeeder.RowCategories.First().Id;
            var newRow = new RowDto {CategoryId = defaultCategory, CinemaHallId = defaultHall, Number = 515};
            var updatedRow = rows.First();
            updatedRow.Number = 510;
            var deleteRowReferences = rows
                .Where(r => r.CinemaHallId == cinemaHall)
                .First(r => DataSeeder.Seats.Select(s => s.RowId).Contains(r.Id))
                .Id;

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info($"Add new row: {newRow.ToPrettyString()} ...");
            response = await Post(BaseUrl, EndpointRow, newRow);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Created);
            var newId = await GetCreatedIdAsync(response);

            Logger.Info($"Update row: ${updatedRow.ToPrettyString()} ...");
            response = await Put(BaseUrl, EndpointRow, updatedRow);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info($"Try delete row with referenced seats: {deleteRowReferences} ...");
            response = await Delete(BaseUrl, $"{EndpointRow}/{deleteRowReferences}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            Logger.Info("Load rows ...");
            response = await Get(BaseUrl, $"{EndpointRows}/{cinemaHall}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            rows = JsonConvert.DeserializeObject<IEnumerable<RowDto>>(await response.Content.ReadAsStringAsync())
                .ToList();

            var rowCategoryIds = rows.Select(g => g.Id).ToList();
            rowCategoryIds.Should().Contain(newId);
            rowCategoryIds.Should().Contain(updatedRow.Id);
            rowCategoryIds.Should().Contain(deleteRowReferences);

            rows.First(g => g.Id == newId).Number.Should().Be(newRow.Number);
            rows.First(g => g.Id == updatedRow.Id).Number.Should().Be(updatedRow.Number);

            Logger.Info(rows.ToPrettyString());
        }

        [Test]
        public async Task Test_Workflow_RowWithoutReservations_Valid()
        {
            var cinemaHall = DataSeeder.CinemaHalls.Last().Id;
            Logger.Info("Load rows ...");
            var response = await Get(BaseUrl, $"{EndpointRows}/{cinemaHall}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var rows = JsonConvert
                .DeserializeObject<IEnumerable<RowDto>>(await response.Content.ReadAsStringAsync()).ToList();
            rows.Should().HaveCount(DataSeeder.Rows.Count(r => r.CinemaHallId == cinemaHall));
            Logger.Info(rows.ToPrettyString());

            var defaultCategory = DataSeeder.RowCategories.First().Id;
            var newRow = new RowDto {CategoryId = defaultCategory, CinemaHallId = cinemaHall, Number = 515};
            var updatedRow = rows.First();
            updatedRow.Number = 510;
            var deleteRowReferences = rows
                .Where(r => r.CinemaHallId == cinemaHall)
                .First(r => DataSeeder.Seats.Select(s => s.RowId).Contains(r.Id))
                .Id;
            var deletedRow = rows
                .Where(r => r.CinemaHallId == cinemaHall)
                .First(r => !DataSeeder.Seats.Select(s => s.RowId).Contains(r.Id))
                .Id;

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info($"Add new row: {newRow.ToPrettyString()} ...");
            response = await Post(BaseUrl, EndpointRow, newRow);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Created);
            var newId = await GetCreatedIdAsync(response);

            Logger.Info($"Update row: ${updatedRow.ToPrettyString()} ...");
            response = await Put(BaseUrl, EndpointRow, updatedRow);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info($"Try delete row with referenced seats: {deleteRowReferences} ...");
            response = await Delete(BaseUrl, $"{EndpointRow}/{deleteRowReferences}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            Logger.Info($"Delete row: {deletedRow} ...");
            response = await Delete(BaseUrl, $"{EndpointRow}/{deletedRow}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info("Load rows ...");
            response = await Get(BaseUrl, $"{EndpointRows}/{cinemaHall}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            rows = JsonConvert.DeserializeObject<IEnumerable<RowDto>>(await response.Content.ReadAsStringAsync())
                .ToList();

            var rowIds = rows.Select(g => g.Id).ToList();
            rowIds.Should().Contain(newId);
            rowIds.Should().Contain(updatedRow.Id);
            rowIds.Should().Contain(deleteRowReferences);
            rowIds.Should().NotContain(deletedRow);

            rows.First(g => g.Id == newId).Number.Should().Be(newRow.Number);
            rows.First(g => g.Id == updatedRow.Id).Number.Should().Be(updatedRow.Number);

            Logger.Info(rows.ToPrettyString());
        }

        [Test]
        public async Task Test_Workflow_Row_Invalid()
        {
            var defaultHall = DataSeeder.CinemaHalls.First().Id;
            var defaultCategory = DataSeeder.RowCategories.First().Id;

            await Authenticate(AdminUser, AdminPassword);

            var response = await Delete(BaseUrl, $"{EndpointRow}/500");
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointRow, new RowDto());
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointRow,
                new RowDto {Id = 500, CinemaHallId = defaultHall, CategoryId = defaultCategory});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointRow,
                new RowDto {Id = 500, CinemaHallId = 0, CategoryId = defaultCategory});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Put(BaseUrl, EndpointRow,
                new RowDto {Id = 500, CinemaHallId = defaultHall, CategoryId = 0});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            var id = DataSeeder.Rows.Last().Id;

            response = await Post(BaseUrl, EndpointRow,
                new RowDto {CinemaHallId = 0, CategoryId = defaultCategory});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            response = await Post(BaseUrl, EndpointRow, new RowDto {CinemaHallId = defaultHall, CategoryId = 0});
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Test_Workflow_Row_UpdateParallel_Users()
        {
            var cinemaHall = DataSeeder.CinemaHalls.Last().Id;
            const int newNumber = 255;

            Logger.Info("Load rows ...");
            var response = await Get(BaseUrl, $"{EndpointRows}/{cinemaHall}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var rows = JsonConvert.DeserializeObject<IEnumerable<RowDto>>(await response.Content.ReadAsStringAsync())
                .ToList();
            rows.Should().HaveCount(DataSeeder.Rows.Count(r => r.CinemaHallId == cinemaHall));
            Logger.Info(rows.ToPrettyString());

            var row = rows.First();
            row.Number = newNumber;

            await Authenticate(AdminUser, AdminPassword);

            Logger.Info("Update row ...");
            response = await Put(BaseUrl, EndpointRow, row);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.NoContent);

            Logger.Info("Try to update outdated row ...");
            row.Number = 10;
            response = await Put(BaseUrl, EndpointRow, row);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.BadRequest);

            Logger.Info($"Load single row with id {row.Id} ...");
            response = await Get(BaseUrl, $"{EndpointRow}/{row.Id}");
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            row = JsonConvert.DeserializeObject<RowDto>(await response.Content.ReadAsStringAsync());

            row.Number.Should().Be(newNumber);

            Logger.Info(row.ToPrettyString());
        }

        [Test]
        public async Task Test_Delete_WithoutAuthentication()
        {
            var urls = new[]
            {
                $"{EndpointCategory}/1",
                $"{EndpointRow}/1"
            };
            foreach (var endpoint in urls)
            {
                var response = await Delete(BaseUrl, endpoint);
                await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Unauthorized);
            }
        }

        [Test]
        public async Task Test_Create_WithoutAuthentication()
        {
            var endpoints = new[]
            {
                EndpointCategory,
                EndpointRow
            };
            foreach (var endpoint in endpoints)
            {
                var response = await Post(BaseUrl, endpoint, new object());
                await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Unauthorized);
            }
        }

        [Test]
        public async Task Test_Update_WithoutAuthentication()
        {
            var endpoints = new[]
            {
                EndpointCategory,
                EndpointRow
            };
            foreach (var endpoint in endpoints)
            {
                var response = await Put(BaseUrl, endpoint, new object());
                await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Unauthorized);
            }
        }
    }
}