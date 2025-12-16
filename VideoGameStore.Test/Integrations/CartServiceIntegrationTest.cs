using VideoGameStore.Dtos;
using VideoGameStore.Entities;
using VideoGameStore.Exceptions;
using VideoGameStore.Mappers;
using VideoGameStore.Services;
using VideoGameStore.Test.Integrations;
using VideoGameStore.Tests;

public class CartServiceIntegrationTests : IntegrationTestBase
{
    private ICartService _cartService = null!;
    private CartItemMapper _cartItemMapper = null!;
    private CartMapper _cartMapper = null!;
    private Game _testGame = null!;
    private Cart _testCart = null!;
    private Customer _testCustomer = null!;
    private Seller _testSeller = null!;

    public CartServiceIntegrationTests(MsSqlFixture fixture) : base(fixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _cartItemMapper = GetService<CartItemMapper>();
        _cartMapper = GetService<CartMapper>();
        _cartService = GetService<ICartService>();

        _testSeller = new Seller { CreatedAt = DateTime.Now };

        _testGame = new Game
        {
            Title = "Test Game",
            Price = 29.99m,
            Description = "Test Description",
            CreatedAt = DateTime.Now,
            Seller = _testSeller,
            DeveloperTitle = "Cda games",
            PublisherTitle = "Cda games"
        };

        _testSeller.Games.Add(_testGame);

        await DbContext.Sellers.AddAsync(_testSeller);
        await DbContext.Games.AddAsync(_testGame);

        _testCustomer = new Customer { CreatedAt = DateTime.Now };

        _testCart = new Cart
        {
            Customer = _testCustomer,
            CartItems = new List<CartItem>()
        };

        _testCustomer.Cart = _testCart;

        await DbContext.Carts.AddAsync(_testCart);
        await DbContext.Customers.AddAsync(_testCustomer);
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task AddToCart_WithValidGameId_ShouldAddCartItem()
    {
        var request = new CartItemRequest(
            GameId: _testGame.Id,
            Quantity: 2
        );

        var initialCount = _testCart.CartItems.Count;

        var result = await _cartService.AddToCart(_testCart, request);

        Assert.NotNull(result);
        Assert.Equal(initialCount + 1, _testCart.CartItems.Count);

        var addedItem = _testCart.CartItems.Last();
        Assert.Equal(_testGame.Id, addedItem.GameId);
        Assert.Equal(2, addedItem.Quantity);
    }

    [Fact]
    public async Task AddToCart_WithInvalidGameId_ShouldThrowEntityNotFound()
    {
        var invalidGameId = 999L;
        var request = new CartItemRequest(
            GameId: invalidGameId,
            Quantity: 1
        );

        var exception = await Assert.ThrowsAsync<EntityNotFound>(
            () => _cartService.AddToCart(_testCart, request));

        Assert.Contains($"Game not found by id: {invalidGameId}", exception.Message);
    }

    [Fact]
    public async Task AddToCart_MultipleItems_ShouldAddAllItems()
    {
        var game2 = new Game
        {
            Title = "Test Game 2",
            Price = 39.99m,
            Description = "Test Description 2",
            CreatedAt = DateTime.UtcNow,
            DeveloperTitle = "Cda games",
            PublisherTitle = "Cda games",
            Seller = _testSeller
        };

        DbContext.Games.Add(game2);
        await DbContext.SaveChangesAsync();

        var request1 = new CartItemRequest(GameId: _testGame.Id, Quantity: 1);
        var request2 = new CartItemRequest(GameId: game2.Id, Quantity: 2);

        // Act
        await _cartService.AddToCart(_testCart, request1);
        var result = await _cartService.AddToCart(_testCart, request2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, _testCart.CartItems.Count);
    }

    [Fact]
    public async Task RemoveFromCart_WithValidGameIdAndLowerQuantity_ShouldDecreaseQuantity()
    {
        var cartItem = new CartItem
        {
            GameId = _testGame.Id,
            Game = _testGame,
            Quantity = 5,
            CartId = _testCart.Id
        };

        _testCart.CartItems.Add(cartItem);
        await DbContext.SaveChangesAsync();

        var request = new CartItemRequest(
            GameId: _testGame.Id,
            Quantity: 2
        );

        var result = await _cartService.RemoveFromCart(_testCart, request);

        Assert.NotNull(result);
        Assert.Single(_testCart.CartItems);
        Assert.Equal(3, _testCart.CartItems.First().Quantity);
    }

    [Fact]
    public async Task RemoveFromCart_WithEqualQuantity_ShouldRemoveCartItem()
    {
        var cartItem = new CartItem
        {
            GameId = _testGame.Id,
            Game = _testGame,
            Quantity = 3,
            CartId = _testCart.Id
        };

        _testCart.CartItems.Add(cartItem);
        await DbContext.SaveChangesAsync();

        var request = new CartItemRequest(
            GameId: _testGame.Id,
            Quantity: 3
        );

        var result = await _cartService.RemoveFromCart(_testCart, request);

        Assert.NotNull(result);
        Assert.Empty(_testCart.CartItems);
    }

    [Fact]
    public async Task RemoveFromCart_WithInvalidGameId_ShouldThrowEntityNotFound()
    {
        // Arrange
        var cartItem = new CartItem
        {
            GameId = _testGame.Id,
            Game = _testGame,
            Quantity = 1,
            CartId = _testCart.Id
        };

        _testCart.CartItems.Add(cartItem);
        await DbContext.SaveChangesAsync();

        var invalidGameId = 999L;
        var request = new CartItemRequest(
            GameId: invalidGameId,
            Quantity: 1
        );

        var exception = await Assert.ThrowsAsync<EntityNotFound>(
            () => _cartService.RemoveFromCart(_testCart, request));

        Assert.Contains($"Game not found by id: {invalidGameId}", exception.Message);
    }

    [Fact]
    public async Task Clear_WithItems_ShouldRemoveAllItems()
    {
        var cartItem1 = new CartItem
        {
            GameId = _testGame.Id,
            Game = _testGame,
            Quantity = 1,
            CartId = _testCart.Id
        };

        var game2 = new Game
        {
            Title = "Test Game 2",
            Price = 39.99m,
            Description = "Test Description 2",
            CreatedAt = DateTime.UtcNow,
            DeveloperTitle = "Cda games",
            PublisherTitle = "Cda games",
            Seller = _testSeller
        };

        DbContext.Games.Add(game2);
        await DbContext.SaveChangesAsync();

        var cartItem2 = new CartItem
        {
            GameId = game2.Id,
            Game = game2,
            Quantity = 2,
            CartId = _testCart.Id
        };

        _testCart.CartItems.Add(cartItem1);
        _testCart.CartItems.Add(cartItem2);
        await DbContext.SaveChangesAsync();

        Assert.Equal(2, _testCart.CartItems.Count);

        await _cartService.Clear(_testCart);

        Assert.Empty(_testCart.CartItems);
    }

    [Fact]
    public async Task Clear_WithEmptyCart_ShouldNotThrowException()
    {
        Assert.Empty(_testCart.CartItems);

        await _cartService.Clear(_testCart);

        Assert.Empty(_testCart.CartItems);
    }

    [Fact]
    public async Task CartResponse_ShouldContainCorrectData()
    {
        var cartItem = new CartItem
        {
            GameId = _testGame.Id,
            Game = _testGame,
            Quantity = 2,
            CartId = _testCart.Id
        };

        _testCart.CartItems.Add(cartItem);
        await DbContext.SaveChangesAsync();

        var result = await _cartService.AddToCart(_testCart, new CartItemRequest(
            GameId: _testGame.Id,
            Quantity: 1
        ));

        Assert.NotNull(result);
        Assert.NotEmpty(result.CartItems);
    }
}
