using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Select;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class MovieDaoAdo : BaseDao<Movie>, IMovieDao
    {
        public MovieDaoAdo(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public Task<IEnumerable<Movie>> SelectWithReferencesAsync()
        {
            return FluentSelectAll()
                .InnerJoin<Movie, Genre, long, long>(_ => _.Genre, _ => _.GenreId, _ => _.Id)
                .QueryAsync();
        }

        public Task<IEnumerable<Movie>> SelectActiveWithGenreAsync()
        {
            return FluentSelectAll()
                .InnerJoin<Movie, Genre, long, long>(_ => _.Genre, _ => _.GenreId, _ => _.Id)
                .WhereActive()
                .QueryAsync();
        }

        public Task<Movie> SelectWithReferencesByIdAsync(long id)
        {
            return FluentSelectAll()
                .InnerJoin<Movie, Genre, long, long>(_ => _.Genre, _ => _.GenreId, _ => _.Id)
                .WhereActive()
                .And(_ => _.Id)
                .Equal(id)
                .QuerySingleAsync();
        }

        public Task<IEnumerable<Movie>> SelectByTitleAsync(string title)
        {
            return FluentSelectAll()
                .InnerJoin<Movie, Genre, long, long>(_ => _.Genre, _ => _.GenreId, _ => _.Id)
                .WhereActive()
                .And(_ => _.Title)
                .Equal(title)
                .QueryAsync();
        }

        public Task<IEnumerable<Movie>> SelectByGenreIdAsync(long genreId)
        {
            return SelectByGenreIdAsync(new[] {genreId});
        }

        public Task<IEnumerable<Movie>> SelectByGenreIdAsync(IEnumerable<long> genreIds)
        {
            return FluentSelectAll()
                .InnerJoin<Movie, Genre, long, long>(_ => _.Genre, _ => _.GenreId, _ => _.Id)
                .WhereActive()
                .And(_ => _.GenreId)
                .In(genreIds)
                .QueryAsync();
        }

        public Task<IEnumerable<Movie>> SelectPagedAsync(int page, int pageSize)
        {
            return SelectMovieWithoutImage()
                .InnerJoin<Movie, Genre, long, long>(_ => _.Genre, _ => _.GenreId, _ => _.Id)
                .WhereActive()
                .Limit(pageSize, (page - 1) * pageSize)
                .QueryAsync();
        }

       

        public Task<IEnumerable<Movie>> SelectByTitlePagedAsync(string title, int page, int pageSize)
        {
            return SelectMovieWithoutImage()
                .InnerJoin<Movie, Genre, long, long>(_ => _.Genre, _ => _.GenreId, _ => _.Id)
                .WhereActive()
                .And(_ => _.Title)
                .StartsWith(title)
                .Limit(pageSize, (page - 1) * pageSize)
                .QueryAsync();
        }

        public Task<IEnumerable<Movie>> SelectByGenreIdPagedAsync(IEnumerable<long> genreIds, int page, int pageSize)
        {
            return SelectMovieWithoutImage()
                .InnerJoin<Movie, Genre, long, long>(_ => _.Genre, _ => _.GenreId, _ => _.Id)
                .WhereActive()
                .And(_ => _.GenreId)
                .In(genreIds)
                .Limit(pageSize, (page - 1) * pageSize)
                .QueryAsync();
        }

        public async Task<byte[]> SelectImageByMovieId(long id)
        {
            var movie = await FluentSelect()
                .Column(_ => _.Image)
                .Where(_ => _.Id)
                .Equal(id)
                .QuerySingleAsync();
            return movie.Image;
        }

        private IFluentEntitySelect<Movie> SelectMovieWithoutImage()
        {
            return FluentSelect()
                .Column(_ => _.Id)
                .Column(_ => _.RowVersion)
                .Column(_ => _.Title)
                .Column(_ => _.Description)
                .Column(_ => _.GenreId)
                .Column(_ => _.Duration)
                .Column(_ => _.Trailer)
                .Column(_ => _.Rating)
                .Column(_ => _.Deleted);
        }
    }
}