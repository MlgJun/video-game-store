using VideoGameStore.Entities;

namespace VideoGameStore.Services
{
    public interface IKeyService
    {
        public Task<List<Key>> CreateKeysAsync(IFormFile file, Game game);
    }
}
