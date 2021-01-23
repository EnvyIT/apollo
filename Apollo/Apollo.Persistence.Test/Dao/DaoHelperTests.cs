using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.FluentEntity;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.Test.Entity.Mock;
using Apollo.Persistence.Util;
using Apollo.Util;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Apollo.Persistence.Test.Dao
{
    public class DaoHelperTests
    {
        private IDaoHelper _daoHelper;
        private IFluentEntityFrom _fluentEntity;


        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ConfigurationHelper.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json")
                .Build();
            var appSettings = ConfigurationHelper.GetValues("Apollo_Test");
            var connectionFactory = new ConnectionFactory(appSettings[0]);
            _daoHelper = new DaoHelper(connectionFactory);
            _fluentEntity = EntityManagerFactory.CreateEntityManager(connectionFactory).FluentEntity();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _fluentEntity.Delete<MovieMock>().ExecuteAsync();
            await _fluentEntity.Delete<GenreMock>().ExecuteAsync();
        }

        [Test]
        public async Task Test_Insert_Genre()
        {
            var genreName = "Drama";
            var insertResult = await _fluentEntity
                .InsertInto(new GenreMock { Id = 1, RowVersion = DateTime.Now, Name = genreName })
                .ExecuteAsync();
            var selectResult = await _fluentEntity.SelectAll<GenreMock>().QuerySingleAsync();

            insertResult.Should().Contain(1);
            selectResult.Id.Should().Be(1);
            selectResult.Name.Should().Be(genreName);
        }

        [Test]
        public async Task Test_Execute_WithoutParameter()
        {
            await _fluentEntity
                .InsertInto(new GenreMock { Id = 1, RowVersion = DateTime.Now, Name = "Drama" })
                .ExecuteAsync();
            var result = await _daoHelper.ExecuteAsync("DELETE FROM genre");

            result.Should().Be(1);
        }

        [Test]
        public async Task Test_Select_Single()
        {
            var name = "Drama";
            await _fluentEntity
                .InsertInto(new List<GenreMock>{
                    new GenreMock { Id = 1, RowVersion = DateTime.Now, Name = name },
                    new GenreMock { Id = 2, RowVersion = DateTime.Now, Name = "Horror" }
                }).ExecuteAsync();
            var selectResult = await _daoHelper.SelectSingleAsync("SELECT genre.id, genre.name FROM genre WHERE genre.id = @id", (record =>
                 new GenreMock
                 {
                     Id = record.GetInt64(0),
                     Name = record.GetString(1)
                 }), new QueryParameter("@id", 1));

            selectResult.Id.Should().Be(1);
            selectResult.Name.Should().Be(name);
        }

        [Test]
        public async Task Test_Select_SingleMultiple()
        {
            await _fluentEntity
                .InsertInto(new List<GenreMock>{
                    new GenreMock { Id = 1, RowVersion = DateTime.Now, Name = "Drama" },
                    new GenreMock { Id = 2, RowVersion = DateTime.Now, Name = "Horror" }
                }).ExecuteAsync();
            var selectResult = await _daoHelper.SelectAsync("SELECT genre.id, genre.name FROM genre WHERE genre.id >= @id", (record =>
                new GenreMock
                {
                    Id = record.GetInt64(0),
                    Name = record.GetString(1)
                }), new QueryParameter("@id", 1));

            selectResult.Should().HaveCount(2);
        }

        [Test]
        public async Task Test_Execute_WithParameter()
        {
            var newValue = "Action";
            await _fluentEntity
                .InsertInto(new GenreMock { Id = 1, RowVersion = DateTime.Now, Name = "Drama" })
                .ExecuteAsync();
            var updateResult = await _daoHelper.ExecuteAsync("UPDATE genre SET genre.name = @name", new QueryParameter("@name", newValue));
            var selectResult = await _fluentEntity.SelectAll<GenreMock>().QuerySingleAsync();

            updateResult.Should().Be(1);
            selectResult.Name.Should().Be(newValue);
        }

        [Test]
        public async Task Test_Transaction_Insert_Success_Commit()
        {
            var movieTitle = "Movie1";
            var genreName = "Drama";
            Func<Task> transaction = async () => await _daoHelper.PerformTransaction(async () =>
            {
                await _fluentEntity.InsertInto(new GenreMock { Id = 1, RowVersion = DateTime.Now, Name = genreName }).ExecuteAsync();
                await _fluentEntity.InsertInto(new MovieMock
                {
                    Id = 1,
                    RowVersion = DateTime.Now,
                    Description = "Demo",
                    Duration = 1,
                    GenreId = 1,
                    Image = new byte[] { },
                    Title = movieTitle,
                    Trailer = "empty"
                }).ExecuteAsync();
            });
            await transaction.Should().NotThrowAsync();

            var selectResultMovie = await _fluentEntity.SelectAll<Movie>().QuerySingleAsync();
            var selectResultGenre = await _fluentEntity.SelectAll<GenreMock>().QuerySingleAsync();

            selectResultMovie.Title.Should().Be(movieTitle);
            selectResultGenre.Name.Should().Be(genreName);
        }

        [Test]
        public async Task Test_Transaction_Insert_Fail_Rollback()
        {
            Func<Task> transaction = async () => await _daoHelper.PerformTransaction(async () =>
            {
                await _fluentEntity.InsertInto(new GenreMock { Id = 1, RowVersion = DateTime.Now, Name = "Drama" }).ExecuteAsync();
                throw new SqlTypeException("Invalid type given!");
            });
            await transaction.Should().ThrowAsync<SqlTypeException>();

            var selectResult = await _fluentEntity.SelectAll<Movie>().QueryAsync();
            selectResult.Should().HaveCount(0);
        }
    }
}
