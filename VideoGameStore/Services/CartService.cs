using VideoGameStore.Context;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;
using VideoGameStore.Exceptions;
using VideoGameStore.Mappers;

namespace VideoGameStore.Services
{
    public class CartService : ICartService
    {
        private readonly CartItemMapper _cartItemMapper;
        private readonly CartMapper _cartMapper;
        private readonly AppDbContext _context;

        public CartService(CartItemMapper cartItemMapper, CartMapper cartMapper,AppDbContext context)
        {
            _cartItemMapper = cartItemMapper;
            _context = context;
            _cartMapper = cartMapper;
        }

        public async Task<CartResponse> AddToCart(Cart cart, CartItemRequest request)
        {
            Game? game = _context.Games.FirstOrDefault(g => g.Id == request.GameId);

            if (game == null)
                throw new EntityNotFound($"Game not found by id: {request.GameId}");

            CartItem cartItem = _cartItemMapper.ToEntity(request, game);

            cart.CartItems.Add(cartItem);

            await _context.SaveChangesAsync();

            return _cartMapper.ToResponse(cart);
        }

        public async Task Clear(Cart cart)
        {
            cart.CartItems.Clear();

            await _context.SaveChangesAsync();
        }

        public async Task<CartResponse> RemoveFromCart(Cart cart, CartItemRequest request)
        {
            var result = cart.CartItems.Select((item, index) => (item, index)).Where(x => x.item.Game.Id == request.GameId).FirstOrDefault();

            if (result.item == null)
                throw new EntityNotFound($"Game not found by id: {request.GameId}");

            if(result.item.Quantity > request.Quantity)
                result.item.Quantity -= request.Quantity;
            else
                cart.CartItems.RemoveAt(result.index);

            await _context.SaveChangesAsync();

            return _cartMapper.ToResponse(cart);
        }
    }
}
