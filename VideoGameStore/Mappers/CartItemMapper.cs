using VideoGameStore.Dtos;
using VideoGameStore.Entities;

namespace VideoGameStore.Mappers
{
    public class CartItemMapper
    {
        public CartItemResponse ToResponse(CartItem cartItem) 
        {
            return new CartItemResponse(cartItem.Game.Id, cartItem.Game.Title, cartItem.Quantity);
        }

        public CartItem ToEntity(CartItemRequest cartItemRequest, Game game)
        {
            var entity = new CartItem();
            entity.Quantity = cartItemRequest.Quantity;
            entity.Game = game;

            return entity;
        }

        public List<CartItemResponse> ToResponseList(List<CartItem> listCartItem)
        {
            List<CartItemResponse> cartItems = [];

            foreach (var i in listCartItem)
            {
                cartItems.Add(ToResponse(i));
            }

            return cartItems;
        }

        public List<CartItem> ToEnitytList(List<CartItemRequest> listCartItem, List<Game> games)
        {
            Dictionary<long, Game> dicGames = new Dictionary<long, Game>();

            foreach (var game in games)
            {
                dicGames.Add(game.Id, game);
            }

            List<CartItem> cartItems = [];

            foreach (var i in listCartItem)
            {
                cartItems.Add(ToEntity(i, dicGames[i.GameId]));
            }

            return cartItems;
        }

    }
}
