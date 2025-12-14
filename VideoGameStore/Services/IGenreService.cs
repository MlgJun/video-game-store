using VideoGameStore.Dtos;

namespace VideoGameStore.Services
{
    public interface IGenreService
    {
        public Task<List<GenreResponse>> FindAll();
    }
}
