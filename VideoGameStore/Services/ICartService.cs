using VideoGameStore.Dtos;

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
        public CartResponse AddToCart(long cartId, CartItemRequest request);
        /// <summary>
        /// Удаляет указанное количество товара из корзины. Полностью удаляет позицию при нулевом остатке.
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public CartResponse RemoveFromCart(long cartId, CartItemRequest request);
        public void Clear(long cartId);
    }
}
