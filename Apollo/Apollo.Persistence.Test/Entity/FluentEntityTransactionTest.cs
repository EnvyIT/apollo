using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Persistence.FluentEntity;
using Apollo.Persistence.Test.Entity.Mock;
using Apollo.Persistence.Util;
using Apollo.Util;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace Apollo.Persistence.Test.Entity
{
    public class FluentEntityTransactionTest : FluentEntityBaseTest
    {
        [TearDown]
        public async Task TearDown()
        {
            await _entityManager.FluentEntity().Delete<MovieMock>().ExecuteAsync();
            await _entityManager.FluentEntity().Delete<GenreMock>().ExecuteAsync();
        }

        [Test]
        public async Task Test_Transaction_Insert_Success_Commit()
        {
            var movieTitle = "Movie1";
            var genreName = "Drama";
            Func<Task> transaction = async () => await _entityManager.FluentTransaction()
                .Perform(_entityManager.FluentEntity().InsertInto(new GenreMock
                {
                    Id = 1,
                    RowVersion = DateTime.Now,
                    Name = genreName
                }))
                .Perform(_entityManager.FluentEntity().InsertInto(new MovieMock
                {
                    Id = 1,
                    RowVersion = DateTime.Now,
                    Description = "Demo",
                    Duration = 1,
                    GenreId = 1,
                    Image = new byte[] { },
                    Title = movieTitle,
                    Trailer = "empty"
                }))
                .Commit();

            await transaction.Should().NotThrowAsync();

            var selectResultMovie = await _entityManager.FluentEntity().SelectAll<MovieMock>().QuerySingleAsync();
            var selectResultGenre = await _entityManager.FluentEntity().SelectAll<GenreMock>().QuerySingleAsync();

            selectResultMovie.Title.Should().Be(movieTitle);
            selectResultGenre.Name.Should().Be(genreName);
        }

        [Test]
        public async Task Test_Transaction_Insert_Fail_Rollback()
        {
            Func<Task> transaction = async () => await _entityManager.FluentTransaction()
                .Perform(_entityManager.FluentEntity().InsertInto(new GenreMock
                {
                    Id = 1,
                    RowVersion = DateTime.Now,
                    Name = "Horror"
                }))
                .Perform(_entityManager.FluentEntity().InsertInto(new MovieMock
                {
                    Id = 1
                }))
                .Commit();

            await transaction.Should().ThrowAsync<MySqlException>();

            var selectResult = await _entityManager.FluentEntity().SelectAll<MovieMock>().QueryAsync();
            selectResult.Should().HaveCount(0);
        }

        [Test]
        public async Task TransactionPerform_WithIFluentEntityCallExecute_ShouldSucceed()
        {
            const string anime = "Anime";
            const string comic = "Comic";
            const string actionAnime = "Action-Anime";
            var genres = new List<GenreMock>
            {
                new GenreMock {Id = 1337L, Name = anime},
                new GenreMock {Id = 42L, Name = comic},
            };
            await _entityManager.FluentEntity()
                .InsertInto(genres)
                .ExecuteAsync();

            var insertedGenres = await _entityManager.FluentEntity()
                .SelectAll<GenreMock>()
                .QueryAsync();

            var insertedGenreList = insertedGenres.OrderBy(g => g.Name).ToList();
            insertedGenreList[0].Name = actionAnime;
            Func<Task> transaction = async () => await _entityManager.FluentTransaction()
                .Perform(_entityManager.FluentEntity().Update(insertedGenreList))
                .Commit();

            await transaction.Should().NotThrowAsync();
            var result = await _entityManager.FluentEntity().SelectAll<GenreMock>().QueryAsync();
            result.Should().HaveCount(2);
            var resultList = result.OrderBy(genre => genre.Name).ToList();
            resultList[0].Name.Should().Be(actionAnime);
            resultList[1].Name.Should().Be(comic);
        }

        [Test]
        public async Task Test_Transaction_Block_Success()
        {
            var movieTitle = "Movie1";
            var genreName = "Drama";
            Func<Task> transaction = async () => await _entityManager.FluentTransaction()
                .PerformBlock(async () =>
                {
                    await _entityManager.FluentEntity().InsertInto(new GenreMock
                    {
                        Id = 1,
                        RowVersion = DateTime.Now,
                        Name = genreName
                    }).ExecuteAsync();
                    await _entityManager.FluentEntity().InsertInto(new MovieMock
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
                })
                .Commit();

            await transaction.Should().NotThrowAsync();

            var selectResultMovie = await _entityManager.FluentEntity().SelectAll<MovieMock>().QuerySingleAsync();
            var selectResultGenre = await _entityManager.FluentEntity().SelectAll<GenreMock>().QuerySingleAsync();

            selectResultMovie.Title.Should().Be(movieTitle);
            selectResultGenre.Name.Should().Be(genreName);
        }

        [Test]
        public async Task Test_Transaction_Block_Fail()
        {
            Func<Task> transaction = async () => await _entityManager.FluentTransaction()
                .PerformBlock(async () =>
                {
                    await _entityManager.FluentEntity().InsertInto(new GenreMock
                    {
                        Id = 1,
                        RowVersion = DateTime.Now,
                        Name = "Horror"
                    }).ExecuteAsync();
                    throw new SqlTypeException("Primary key invalid!");
                })
                .Commit();

            await transaction.Should().ThrowAsync<SqlTypeException>();

            var selectResult = await _entityManager.FluentEntity().SelectAll<MovieMock>().QueryAsync();
            selectResult.Should().HaveCount(0);
        }

        [Test]
        public async Task Test_Transaction_Nested()
        {
            var genre1Name = "Drama";
            var genre2Name = "Horror";
            try
            {
                await _entityManager.FluentTransaction()
                    .PerformBlock(async () =>
                    {
                        await _entityManager.FluentTransaction()
                            .PerformBlock(async () =>
                            {

                                await _entityManager.FluentEntity().InsertInto(new GenreMock
                                {
                                    RowVersion = DateTime.Now,
                                    Name = genre1Name
                                }).ExecuteAsync();
                            }).Commit();


                        await _entityManager.FluentEntity().InsertInto(new GenreMock
                        {
                            RowVersion = DateTime.Now,
                            Name = genre2Name
                        }).ExecuteAsync();
                        throw new InvalidOperationException();
                    })
                    .Commit();
            }
            catch (InvalidOperationException)
            {
                // ignore
            }

            var genres = (await _entityManager.FluentEntity().SelectAll<GenreMock>().QueryAsync())
                .Select(g => g.Name)
                .ToList();

            genres.Should().NotContain(genre1Name);
            genres.Should().NotContain(genre2Name);
        }

        [Test]
        public async Task Test_Transaction_Nested_DifferentConnections()
        {
            ConfigurationHelper.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json")
                .Build();
            var appSettings = ConfigurationHelper.GetValues("Apollo_Test");
            var connectionFactory = new ConnectionFactory(appSettings[0]);
            var _entityManager2 = EntityManagerFactory.CreateEntityManager(connectionFactory);

            var genre1Name = "Drama";
            var genre2Name = "Horror";
            try
            {
                await _entityManager.FluentTransaction()
                    .PerformBlock(async () =>
                    {
                        await _entityManager2.FluentTransaction()
                            .PerformBlock(async () =>
                            {
                                await _entityManager.FluentEntity().InsertInto(new GenreMock
                                {
                                    RowVersion = DateTime.Now,
                                    Name = genre1Name
                                }).ExecuteAsync();
                            }).Commit();


                        await _entityManager.FluentEntity().InsertInto(new GenreMock
                        {
                            RowVersion = DateTime.Now,
                            Name = genre2Name
                        }).ExecuteAsync();
                        throw new InvalidOperationException();
                    })
                    .Commit();
            }
            catch (InvalidOperationException)
            {
                // ignore
            }

            var genres = (await _entityManager.FluentEntity().SelectAll<GenreMock>().QueryAsync())
                .Select(g => g.Name)
                .ToList();

            genres.Should().NotContain(genre1Name);
            genres.Should().NotContain(genre2Name);
        }
    }
}