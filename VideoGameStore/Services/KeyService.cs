using System.Text.Json;
using System.Text;
using VideoGameStore.Entities;
using VideoGameStore.Mappers;
using VideoGameStore.Exceptions;

namespace VideoGameStore.Services
{
    public class KeyService : IKeyService
    {
        private readonly KeyMapper _mapper;

        public KeyService(KeyMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<List<Key>> CreateKeysAsync(IFormFile file, Game game)
        {
            using MemoryStream memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream);

            string? jsonString = Encoding.UTF8.GetString(memoryStream.ToArray());

            string[]? keys = JsonSerializer.Deserialize<string[]>(jsonString);

            if (keys == null || keys.Length > 0)
                throw new BadRequest("File doesn't contain keys");

            return _mapper.ToEntityList(keys, game);
        }
    }
}
