using Microsoft.EntityFrameworkCore;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;
using VideoGameStore.Exceptions;
using VideoGameStore.Mappers;
using VideoGameStore.Services;
using VideoGameStore.Tests;

namespace VideoGameStore.Test.Integrations
{
    public class OrderServiceIntegrationTest : IntegrationTestBase
    {
        private IOrderService _orderService = null!;
        private OrderMapper _orderMapper = null!;
        private Game _testGame1 = null!;
        private Game _testGame2 = null!;
        private Customer _testCustomer = null!;
        private Seller _testSeller = null!;

        public OrderServiceIntegrationTest(MsSqlFixture fixture) : base(fixture)
        {
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _orderMapper = GetService<OrderMapper>();
            _orderService = GetService<IOrderService>();

            _testSeller = new Seller { CreatedAt = DateTime.Now };

            _testGame1 = new Game
            {
                Title = "Test Game 1",
                Price = 29.99m,
                Description = "Test Description 1",
                CreatedAt = DateTime.Now,
                Seller = _testSeller,
                DeveloperTitle = "Cda games",
                PublisherTitle = "Cda games"
            };

            _testGame2 = new Game
            {
                Title = "Test Game 2",
                Price = 49.99m,
                Description = "Test Description 2",
                CreatedAt = DateTime.Now,
                Seller = _testSeller,
                DeveloperTitle = "Cda games",
                PublisherTitle = "Cda games"
            };

            _testSeller.Games.Add(_testGame1);
            _testSeller.Games.Add(_testGame2);

            await DbContext.Sellers.AddAsync(_testSeller);
            await DbContext.Games.AddAsync(_testGame1);
            await DbContext.Games.AddAsync(_testGame2);
            await DbContext.SaveChangesAsync();

            _testCustomer = new Customer { CreatedAt = DateTime.Now };
            await DbContext.Customers.AddAsync(_testCustomer);
            await DbContext.SaveChangesAsync();
        }

        private async Task CreateKeysForGames(int gameId1Count, int gameId2Count)
        {
            for (int i = 0; i < gameId1Count; i++)
            {
                var key = new Key { Value = $"KEY-{_testGame1.Id}-{i}", GameId = _testGame1.Id };
                await DbContext.Keys.AddAsync(key);
            }

            for (int i = 0; i < gameId2Count; i++)
            {
                var key = new Key { Value = $"KEY-{_testGame2.Id}-{i}", GameId = _testGame2.Id };
                await DbContext.Keys.AddAsync(key);
            }

            await DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task Create_WithValidOrderItems_ShouldDeleteKeysFromDatabase()
        {
            await CreateKeysForGames(1, 1);
            var initialKeysCount = DbContext.Keys.Count();

            var orderItems = new List<OrderItemRequest>
            {
                new OrderItemRequest(_testGame1.Id, 1),
                new OrderItemRequest(_testGame2.Id, 1)
            };

            var request = new OrderRequest(OrderItems: orderItems);

            var result = await _orderService.Create(_testCustomer, request);

            var finalKeysCount = DbContext.Keys.Count();

            Assert.NotNull(result);
            Assert.Equal(initialKeysCount - 2, finalKeysCount);
        }

        [Fact]
        public async Task Create_WithValidOrderItems_ShouldAssignKeysToOrderItems()
        {
            const string expectedKey1 = "KEY-TEST-GAME1";
            const string expectedKey2 = "KEY-TEST-GAME2";

            var key1 = new Key { Value = expectedKey1, GameId = _testGame1.Id };
            var key2 = new Key { Value = expectedKey2, GameId = _testGame2.Id };

            await DbContext.Keys.AddAsync(key1);
            await DbContext.Keys.AddAsync(key2);
            await DbContext.SaveChangesAsync();

            var orderItems = new List<OrderItemRequest>
            {
                new OrderItemRequest(_testGame1.Id, 1),
                new OrderItemRequest(_testGame2.Id, 1)
            };

            var request = new OrderRequest(orderItems);

            var result = await _orderService.Create(_testCustomer, request);

            Assert.NotNull(result);
            Assert.NotEmpty(result.OrderItems);
            Assert.Equal(2, DbContext.OrderItems.Where(o => o.Keys
            .Contains(expectedKey1) || o.Keys.Contains(expectedKey2)).ToList().Count);
        }

        [Fact]
        public async Task Create_WithInvalidGameId_ShouldThrowEntityNotFound()
        {
            const long invalidGameId = 999L;
            var orderItems = new List<OrderItemRequest>
            {
                new OrderItemRequest(GameId: invalidGameId, Quantity: 1)
            };

            var request = new OrderRequest(OrderItems: orderItems);

            var exception = await Assert.ThrowsAsync<EntityNotFound>(
                () => _orderService.Create(_testCustomer, request));

            Assert.Contains("Some game not found", exception.Message);
        }

        [Fact]
        public async Task Create_ShouldPersistOrderInDatabase()
        {
            await CreateKeysForGames(1, 0);

            var orderItems = new List<OrderItemRequest>
            {
                new OrderItemRequest(_testGame1.Id, 1)
            };

            var request = new OrderRequest(orderItems);

            var result = await _orderService.Create(_testCustomer, request);

            Assert.NotNull(result);

            var persistedOrder = await DbContext.Orders.Include(o => o.OrderItems).Include(o => o.Customer)
                .Where(o => o.Customer.Id == _testCustomer.Id).FirstAsync();

            Assert.NotNull(persistedOrder);
            Assert.NotNull(persistedOrder.OrderItems);
            Assert.NotNull(persistedOrder.Customer);
            Assert.Equal(persistedOrder.Customer.Id, _testCustomer.Id);
        }

        [Fact]
        public async Task Create_WithNotEnoughKeys_ShouldThrowInvalidOperationException()
        {
            await CreateKeysForGames(1, 0);

            var orderItems = new List<OrderItemRequest>
            {
                new OrderItemRequest(GameId: _testGame1.Id, Quantity: 1),
                new OrderItemRequest(GameId: _testGame2.Id, Quantity: 1)
            };

            var request = new OrderRequest(OrderItems: orderItems);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _orderService.Create(_testCustomer, request));
        }

        [Fact]
        public async Task FindAllByCustomerId_ShouldReturnHisOrders()
        {
            await CreateKeysForGames(5, 0);

            for (int i = 0; i < 5; i++)
            {
                var orderItems = new List<OrderItemRequest>
                {
                    new OrderItemRequest(_testGame1.Id, 1)
                };
                var request = new OrderRequest(orderItems);
                await _orderService.Create(_testCustomer, request);
            }

            var pageable = new Pageable(1, 2);

            var result = await _orderService.FindAllByCustomerId(_testCustomer.Id, pageable);

            Assert.NotNull(result);
            Assert.Equal(2, result.Content.Count());
        }
    }
}
