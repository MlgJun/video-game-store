using VideoGameStore.Dtos;
using VideoGameStore.Entities;

namespace VideoGameStore.Mappers
{
    public class OrderItemMapper
    {
        public OrderItemResponse ToResponse(OrderItem orderItem)
        {
            return new OrderItemResponse(orderItem.Game.Title, orderItem.Quantity, orderItem.Price);
        }

        public OrderItem ToEntity(OrderItemRequest itemRequest, Game game)
        {
            var entity = new OrderItem();
            entity.Quantity = itemRequest.Quantity;
            entity.Price = game.Price;
            entity.Game = game;

            return entity;
        }

        public List<OrderItemResponse> ToResponseList(List<OrderItem> listOrderItems)
        {
            List<OrderItemResponse> orederItems = [];

            foreach (var i in listOrderItems)
            {
                orederItems.Add(ToResponse(i));
            }

            return orederItems;
        }

        public List<OrderItem> ToEnitytList(List<OrderItemRequest> listOrderItem, List<Game> games)
        {
            Dictionary<long, Game> dicGames = new Dictionary<long, Game>();

            foreach (var game in games)
            {
                dicGames.Add(game.Id, game);
            }

            List<OrderItem> orderItems = [];

            foreach (var i in listOrderItem)
            {
                orderItems.Add(ToEntity(i, dicGames[i.GameId]));
            }

            return orderItems;
        }
    }
}
