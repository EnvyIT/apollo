using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Core.Interfaces;
using Apollo.UnitOfWork.Interfaces;
using static Apollo.Core.Dto.Mapper;

namespace Apollo.Core.Implementation
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MovieService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<MovieDto>> GetMoviesAsync()
        {
            return (await _unitOfWork.RepositoryMovie.GetMoviesAsync())
                .Select(Map);
        }

        public async Task<IEnumerable<MovieDto>> GetMoviesAsync(int page, int pageSize)
        {
            return (await _unitOfWork.RepositoryMovie.GetMoviesAsync(page, pageSize)).Select(Map);
        }

        public async Task<IEnumerable<MovieDto>> GetActiveMoviesByGenreIdAsync(GenreDto genreDto)
        {
            return (await _unitOfWork.RepositoryMovie.GetActiveMoviesByGenreIdAsync(genreDto.Id))
                .Select(Map);
        }

        public async Task<IEnumerable<MovieDto>> GetActiveMoviesByGenreIdAsync(long genreId)
        {
            return (await _unitOfWork.RepositoryMovie.GetActiveMoviesByGenreIdAsync(genreId)).Select(Map);
        }

        public async Task<IEnumerable<MovieDto>> GetActiveMoviesByTitleAsync(string title)
        {
            return (await _unitOfWork.RepositoryMovie.GetActiveMoviesByTitleAsync(title))
                .Select(Map);
        }

        public async Task<IEnumerable<MovieDto>> GetActiveMoviesByTitleAsync(string title, int page, int pageSize)
        {
            return (await _unitOfWork.RepositoryMovie.GetActiveMoviesByTitleAsync(title, page, pageSize)).Select(Map);
        }

        public async Task<IEnumerable<MovieDto>> GetActiveMoviesByGenreIdAsync(IEnumerable<long> genreIds, int page, int pageSize)
        {
            return (await _unitOfWork.RepositoryMovie.GetActiveMoviesByGenreIdAsync(genreIds, page, pageSize)).Select(Map);
        }

        public async Task<IEnumerable<MovieDto>> GetMoviesByActorIdAsync(long actorId)
        {
            return (await _unitOfWork.RepositoryMovie.GetMoviesByActorIdAsync(actorId)).Select(Map);
        }

        public async Task<IEnumerable<MovieDto>> GetActiveMoviesByGenreIdAsync(IEnumerable<long> genreIds)
        {
            return (await _unitOfWork.RepositoryMovie.GetActiveMoviesByGenreIdAsync(genreIds))
                .Select(Map);
        }

        public async Task<MovieDto> GetActiveMovieByIdAsync(long id)
        {
            return Map(await _unitOfWork.RepositoryMovie.GetActiveMovieByIdAsync(id));
        }

        public async Task<MovieDto> UpdateMovieAsync(MovieDto movieDto)
        {
            var movie = Map(movieDto);
            return await _unitOfWork.RepositoryMovie.UpdateMovieAsync(movie) > 0 ? movieDto : null;
        }

        public async Task<MovieDto> AddMovieAsync(MovieDto movieDto)
        {
            var movieId = await _unitOfWork.RepositoryMovie.AddMovieAsync(Map(movieDto));
            return await GetActiveMovieByIdAsync(movieId);
        }

        public async Task<bool> DeleteMovieAsync(MovieDto movieDto)
        {
            return await _unitOfWork.RepositoryMovie.DeleteMovieAsync(movieDto.Id) > 0;
        }

        public async Task<bool> DeleteMovieAsync(long movieId)
        {
            return await _unitOfWork.RepositoryMovie.DeleteMovieAsync(movieId) > 0;
        }

        public async Task<MovieDto> AddMovieAsync(MovieDto movie, IEnumerable<long> actorIds)
        {
            var movieId = await _unitOfWork.RepositoryMovie.AddMovieAsync(Map(movie), actorIds);
            return await GetActiveMovieByIdAsync(movieId);
        }

        public async Task AddActorToMovieAsync(long movieId, long actorId)
        {
            await _unitOfWork.RepositoryMovie.AddActorToMovieAsync(movieId, actorId);
        }

        public async Task AddActorToMovieAsync(long movieId, IEnumerable<long> actorIds)
        {
            await _unitOfWork.RepositoryMovie.AddActorToMovieAsync(movieId, actorIds);
        }

        public async Task<bool> RemoveActorFromMovieAsync(long movieId, long actorId)
        {
            return await _unitOfWork.RepositoryMovie.RemoveActorFromMovieAsync(movieId, actorId) > 0;
        }

        public async Task<bool> RemoveActorFromMovieAsync(long movieId, IEnumerable<long> actorIds)
        {
            return await _unitOfWork.RepositoryMovie.RemoveActorFromMovieAsync(movieId, actorIds) > 0;
        }

        public async Task<IEnumerable<ActorDto>> GetActiveActorsAsync(int page, int pageSize)
        {
            return (await _unitOfWork.RepositoryMovie.GetActiveActorsAsync())
                .Select(Map)
                .Skip((page -1) * pageSize)
                .Take(pageSize);
        }

        public async Task<IEnumerable<ActorDto>> GetActorsByMovieIdAsync(long movieId)
        {
            return (await _unitOfWork.RepositoryMovie.GetActorsByMovieIdAsync(movieId))
                .Select(Map);
        }

        public async Task<bool> DeleteActorAsync(long actorId)
        {
            return await _unitOfWork.RepositoryMovie.DeleteActorAsync(actorId) > 0;
        }

        public async Task<ActorDto> AddActorAsync(ActorDto actor)
        {
            var actorId = await _unitOfWork.RepositoryMovie.AddActorAsync(Map(actor));
            return await GetActorByIdAsync(actorId);
        }

        private async Task<ActorDto> GetActorByIdAsync(long actorId)
        {
            return Map(await _unitOfWork.RepositoryMovie.GetActorByIdAsync(actorId));
        }


        public async Task<ActorDto> UpdateActorAsync(ActorDto actorDto)
        {
            var actor = Map(actorDto);
            return await _unitOfWork.RepositoryMovie.UpdateActorAsync(actor) > 0 ? actorDto : null;
        }

        public async Task<IEnumerable<GenreDto>> GetActiveGenresAsync()
        {
            return (await _unitOfWork.RepositoryMovie.GetActiveGenresAsync())
                .Select(Map);
        }

        public async Task<bool> DeleteGenreAsync(long genreId)
        {
            return await _unitOfWork.RepositoryMovie.DeleteGenreAsync(genreId) > 0;
        }

        public async Task<GenreDto> AddGenreAsync(GenreDto genreDto)
        {
            var genreId = await _unitOfWork.RepositoryMovie.AddGenreAsync(Map(genreDto));
            return await GetActiveGenreByIdAsync(genreId);
        }

        private async Task<GenreDto> GetActiveGenreByIdAsync(long genreId)
        {
           return Map(await _unitOfWork.RepositoryMovie.GetActiveGenreByIdAsync(genreId));
        }


        public async Task<GenreDto> UpdateGenreAsync(GenreDto genreDto)
        {
            var genre = Map(genreDto);
            return await _unitOfWork.RepositoryMovie.UpdateGenreAsync(genre) > 0 ? genreDto : null;
        }

        public async Task<byte[]> FetchImageByMovieId(long movieId)
        {
            return await _unitOfWork.RepositoryMovie.FetchImageByMovieId(movieId);
        }
    }
}