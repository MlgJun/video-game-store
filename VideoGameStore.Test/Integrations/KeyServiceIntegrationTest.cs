using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using VideoGameStore.Entities;
using VideoGameStore.Services;
using VideoGameStore.Exceptions;
using VideoGameStore.Tests;

namespace VideoGameStore.Test.Integrations
{
    [Collection("Database collection")]
    public class KeyServiceIntegrationTest : IntegrationTestBase
    {
        private IKeyService _keyService = null!;
        private Game _testGame = null!;
        private Seller _seller = null!;

        public KeyServiceIntegrationTest(MsSqlFixture fixture) : base(fixture)
        {
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _keyService = GetService<IKeyService>();

            _seller = new Seller { CreatedAt = DateTime.UtcNow };

            _testGame = new Game
            {
                Title = "Test Game",
                Description = "Description",
                DeveloperTitle = "Ziliboba",
                PublisherTitle = "ZilibobaPublisher",
                Price = 29.99m,
                CreatedAt = DateTime.Now,
                Seller = _seller,
                ImageUrl = "fakeurl/1546"
            };

            _seller.Games.Add(_testGame);

            await DbContext.Sellers.AddAsync(_seller);
            await DbContext.Games.AddAsync(_testGame);
            await DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task CreateKeysAsync_WithValidFile_ShouldCreateKeys()
        {
            var keys = new[] { "KEY-001-ABC", "KEY-002-DEF", "KEY-003-GHI" };
            var file = CreateFormFile(keys);

            var result = await _keyService.CreateKeysAsync(file, _testGame);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.All(result, key => Assert.Equal(_testGame.Id, key.GameId));
        }

        [Fact]
        public async Task CreateKeysAsync_WithEmptyArray_ShouldThrowBadRequest()
        {
            var keys = Array.Empty<string>();
            var file = CreateFormFile(keys);

            var exception = await Assert.ThrowsAsync<BadRequest>(
                () => _keyService.CreateKeysAsync(file, _testGame));

            Assert.Contains("doesn't contain keys", exception.Message);
        }

        [Fact]
        public async Task CreateKeysAsync_WithInvalidJson_ShouldThrowJsonException()
        {
            var bytes = Encoding.UTF8.GetBytes("{ invalid json }");
            var stream = new MemoryStream(bytes);
            var file = new FormFileFromStream(stream, 0, stream.Length, "file", "keys.json");

            await Assert.ThrowsAsync<JsonException>(
                () => _keyService.CreateKeysAsync(file, _testGame));
        }

        [Fact]
        public async Task CreateKeysAsync_ShouldMapKeysToCorrectGame()
        {
            var game2 = new Game
            {
                Title = "Another Game",
                Description = "Desc",
                DeveloperTitle = "Ziliboba",
                PublisherTitle = "ZilibobaPublisher",
                Price = 49.99m,
                CreatedAt = DateTime.Now,
                Seller = _seller,
                ImageUrl = "fakeurl/999"
            };
            await DbContext.Games.AddAsync(game2);
            await DbContext.SaveChangesAsync();

            var keys = new[] { "KEY-GAME2-001", "KEY-GAME2-002" };
            var file = CreateFormFile(keys);

            var result = await _keyService.CreateKeysAsync(file, game2);

            Assert.All(result, key => Assert.Equal(game2.Id, key.GameId));
        }

        private IFormFile CreateFormFile(string[] keys)
        {
            var json = JsonSerializer.Serialize(keys);
            var bytes = Encoding.UTF8.GetBytes(json);
            var stream = new MemoryStream(bytes);
            return new FormFileFromStream(stream, 0, stream.Length, "file", "keys.json");
        }

        private class FormFileFromStream : IFormFile
        {
            private readonly Stream _stream;
            public string ContentType => "application/json";
            public string ContentDisposition => $"form-data; name=\"file\"; filename=\"{FileName}\"";
            public IHeaderDictionary Headers => new HeaderDictionary();
            public long Length => _stream.Length;
            public string Name => "file";
            public string FileName { get; }

            public FormFileFromStream(Stream stream, long baseStreamOffset, long length, string name, string fileName)
            {
                _stream = stream;
                FileName = fileName;
            }

            public Stream OpenReadStream() => _stream;
            public void CopyTo(Stream target) => _stream.CopyTo(target);
            public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
                => await _stream.CopyToAsync(target, cancellationToken);
        }
    }
}
