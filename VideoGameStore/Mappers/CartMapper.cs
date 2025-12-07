using VideoGameStore.Dtos;
using VideoGameStore.Entities;

namespace VideoGameStore.Mappers
{
    public class CartMapper
    {
        private readonly CartItemMapper _itemMapper;

        public CartMapper(CartItemMapper itemMapper)
        {   
            _itemMapper = itemMapper;
        }

        public CartResponse ToResponse(Cart cart)
        {
            return new CartResponse(_itemMapper.ToResponseList(cart.CartItems));
        }
    }
}