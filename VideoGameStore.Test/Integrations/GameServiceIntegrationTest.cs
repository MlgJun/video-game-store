using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;
using VideoGameStore.Exceptions;
using VideoGameStore.Mappers;
using VideoGameStore.Services;
using VideoGameStore.Tests;

namespace VideoGameStore.Test.Integrations
{
    [Collection("Database collection")]
    public class GameServiceIntegrationTest : IntegrationTestBase
    {
        private IGameService _gameService = null!;
        private GameMapper _gameMapper = null!;
        private Seller _testSeller = null!;

        public GameServiceIntegrationTest(MsSqlFixture fixture) : base(fixture)
        {
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _gameMapper = GetService<GameMapper>();
            _gameService = GetService<IGameService>();

            _testSeller = new Seller { CreatedAt = DateTime.Now };
            await DbContext.Sellers.AddAsync(_testSeller);
            await DbContext.SaveChangesAsync();
        }

        private GameWithKeysRequest CreateGameWithKeysRequest(string title = "Test Game", decimal price = 29.99m)
        {
            return new GameWithKeysRequest
            {
                Title = title + new Random().Next(1, 100).ToString(),
                Price = price,
                Description = "Test Description",
                DeveloperTitle = "Test Developer",
                PublisherTitle = "Test Publisher",
                File = CreateMockFormFile(),
                Genres = new List<GenreRequest>() { new GenreRequest("Action") }
            };
        }

        private IFormFile CreateMockFormFile(params string[] keys)
        {
            var keysList = keys.Length > 0
                ? keys
                : new[] { "KEY-001", "KEY-002", "KEY-003" };

            var json = JsonSerializer.Serialize(keysList);

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(json);
            writer.Flush();
            stream.Position = 0;

            return new FormFile(stream, 0, stream.Length, "file", "keys.json")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/json"
            };
        }



        [Fact]
        public async Task Create_WithValidRequest_ShouldCreateGameWithKeys()
        {
            // Arrange
            var request = CreateGameWithKeysRequest();

            // Act
            var result = await _gameService.Create(request, _testSeller);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Title, result.Title);
            Assert.Equal(request.Price, result.Price);

            var persistedGame = await DbContext.Games.Include(g => g.Keys)
                .FirstOrDefaultAsync(g => g.Id == result.Id);

            Assert.NotNull(persistedGame);
            Assert.NotEmpty(persistedGame.Keys);
        }

        [Fact]
        public async Task FindById_WithValidGameId_ShouldReturnGame()
        {
            // Arrange
            var request = CreateGameWithKeysRequest();
            var created = await _gameService.Create(request, _testSeller);

            // Act
            var result = await _gameService.FindById(created.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
            Assert.Equal(request.Title, result.Title);
        }

        [Fact]
        public async Task FindById_WithInvalidGameId_ShouldThrowEntityNotFound()
        {
            // Act & Assert
            const long invalidGameId = 999L;
            var exception = await Assert.ThrowsAsync<EntityNotFound>(
                () => _gameService.FindById(invalidGameId));

            Assert.Contains("Game not found by id", exception.Message);
        }

        [Fact]
        public async Task Delete_WithValidGameId_ShouldDeleteGame()
        {
            // Arrange
            var request = CreateGameWithKeysRequest();
            var created = await _gameService.Create(request, _testSeller);

            // Act
            var result = await _gameService.Delete(created.Id, _testSeller.Id);

            // Assert
            Assert.True(result);

            var deletedGame = await DbContext.Games.FirstOrDefaultAsync(g => g.Id == created.Id);
            Assert.Null(deletedGame);
        }

        [Fact]
        public async Task Delete_WithInvalidSellerId_ShouldThrowBadRequest()
        {
            // Arrange
            var request = CreateGameWithKeysRequest();
            var created = await _gameService.Create(request, _testSeller);

            var otherSeller = new Seller { CreatedAt = DateTime.Now };
            await DbContext.Sellers.AddAsync(otherSeller);
            await DbContext.SaveChangesAsync();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequest>(
                () => _gameService.Delete(created.Id, otherSeller.Id));

            Assert.Contains("You can only delete your own games", exception.Message);
        }

        [Fact]
        public async Task Delete_WithInvalidGameId_ShouldThrowEntityNotFound()
        {
            // Act & Assert
            const long invalidGameId = 999L;
            var exception = await Assert.ThrowsAsync<EntityNotFound>(
                () => _gameService.Delete(invalidGameId, _testSeller.Id));

            Assert.Contains("Game not found by id", exception.Message);
        }

        [Fact]
        public async Task Update_WithValidRequest_ShouldUpdateGame()
        {
            // Arrange
            var createRequest = CreateGameWithKeysRequest();
            var created = await _gameService.Create(createRequest, _testSeller);

            var updateRequest = new GameRequest(
                Title: "Updated Title",
                Price: 39.99m,
                Description: "Updated Description",
                DeveloperTitle: "Updated Developer",
                PublisherTitle: "Updated Publisher",
                Genres: new List<GenreRequest>() { new GenreRequest("Action")}
            );

            // Act
            var result = await _gameService.Update(created.Id, _testSeller.Id, updateRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateRequest.Title, result.Title);
            Assert.Equal(updateRequest.Price, result.Price);
        }

        [Fact]
        public async Task Update_WithInvalidSellerId_ShouldThrowBadRequest()
        {
            // Arrange
            var createRequest = CreateGameWithKeysRequest();
            var created = await _gameService.Create(createRequest, _testSeller);

            var otherSeller = new Seller { CreatedAt = DateTime.Now };
            await DbContext.Sellers.AddAsync(otherSeller);
            await DbContext.SaveChangesAsync();

            var updateRequest = new GameRequest(
                Title: "Updated Title",
                Price: 39.99m,
                Description: "Updated Description",
                DeveloperTitle: "Updated Developer",
                PublisherTitle: "Updated Publisher",
                Genres: new List<GenreRequest>()
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequest>(
                () => _gameService.Update(created.Id, otherSeller.Id, updateRequest));

            Assert.Contains("You can only update your own games", exception.Message);
        }

        [Fact]
        public async Task AddKeys_WithValidGameId_ShouldAddKeys()
        {
            var createRequest = CreateGameWithKeysRequest();
            var created = await _gameService.Create(createRequest, _testSeller);

            var initialKeysCount = await DbContext.Keys.CountAsync(k => k.GameId == created.Id);

            var newKeysFile = CreateMockFormFile("KEY-NEW-001", "KEY-NEW-002");

            var result = await _gameService.AddKeys(created.Id, newKeysFile);

            Assert.NotNull(result);

            var finalKeysCount = await DbContext.Keys.CountAsync(k => k.GameId == created.Id);
            Assert.True(finalKeysCount > initialKeysCount);
        }


        [Fact]
        public async Task AddKeys_WithInvalidGameId_ShouldThrowEntityNotFound()
        {
            // Arrange
            const long invalidGameId = 999L;
            var keysFile = CreateMockFormFile();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFound>(
                () => _gameService.AddKeys(invalidGameId, keysFile));

            Assert.Contains("Game not found by id", exception.Message);
        }

        [Fact]
        public async Task FindAll_WithGamesAndKeys_ShouldReturnGames()
        {
            // Arrange
            var request1 = CreateGameWithKeysRequest("Game 1", 29.99m);
            var request2 = CreateGameWithKeysRequest("Game 2", 49.99m);

            await _gameService.Create(request1, _testSeller);
            await _gameService.Create(request2, _testSeller);

            var pageable = new Pageable(1, 10);

            // Act
            var result = await _gameService.FindAll(pageable);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Content);
            Assert.True(result.Content.Count() >= 2);
        }

        [Fact]
        public async Task FindAllBySellerId_WithValidSellerId_ShouldReturnSellerGames()
        {
            // Arrange
            var request1 = CreateGameWithKeysRequest("Seller Game 1", 29.99m);
            var request2 = CreateGameWithKeysRequest("Seller Game 2", 39.99m);

            await _gameService.Create(request1, _testSeller);
            await _gameService.Create(request2, _testSeller);

            var pageable = new Pageable(1, 10);

            // Act
            var result = await _gameService.FindAllBySellerId(_testSeller.Id, pageable);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Content);
            Assert.True(result.Content.Count() >= 2);
        }

        [Fact]
        public async Task FindAllByFilter_WithTitleFilter_ShouldReturnFilteredGames()
        {
            // Arrange
            var request = CreateGameWithKeysRequest("Specific Game Title", 29.99m);
            await _gameService.Create(request, _testSeller);

            var filter = new FilterRequest(
                MinPrice: null,
                MaxPrice: null,
                GameTitle: "Specific",
                Genres: null
            );

            var pageable = new Pageable(1, 2);

            // Act
            var result = await _gameService.FindAllByFilter(pageable, filter);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Content);
            Assert.All(result.Content, game => Assert.Contains("Specific", game.Title));
        }

        [Fact]
        public async Task FindAllByFilter_WithPriceFilter_ShouldReturnFilteredGames()
        {
            // Arrange
            await _gameService.Create(CreateGameWithKeysRequest("Cheap Game", 19000.99m), _testSeller);
            await _gameService.Create(CreateGameWithKeysRequest("Expensive Game", 5900.99m), _testSeller);

            var filter = new FilterRequest(
                MinPrice: 3000m,
                MaxPrice: null,
                GameTitle: null,
                Genres: new List<string>()
            );

            var pageable = new Pageable(1, 2);

            // Act
            var result = await _gameService.FindAllByFilter(pageable, filter);

            // Assert
            Assert.NotNull(result);
            Assert.All(result.Content, game => Assert.True(game.Price >= 3000m));
        }
    }
}

