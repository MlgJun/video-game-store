using VideoGameStore.Dtos;
using VideoGameStore.Entities;

namespace VideoGameStore.Services
{
    public interface ICartService
    {
        /// <summary>
        /// Добавляет позицию в корзину. Если позиция уже есть, то увеличивает ее количество в корзине.
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<CartResponse> AddToCart(Cart cart, CartItemRequest request);
        /// <summary>
        /// Удаляет указанное количество товара из корзины. Полностью удаляет позицию при нулевом остатке.
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<CartResponse> RemoveFromCart(Cart cart, CartItemRequest request);
        public Task Clear(Cart cart);
    }
}
