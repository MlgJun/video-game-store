using VideoGameStore.Dtos;
using VideoGameStore.Entities;

namespace VideoGameStore.Mappers
{
    public class OrderMapper
    {
        private readonly OrderItemMapper _orderItemMapper;

        public OrderMapper(OrderItemMapper orderItemMapper)
        {
            _orderItemMapper = orderItemMapper;    
        }

        public OrderResponse ToResponse(Order order)
        {
            return new OrderResponse(_orderItemMapper.ToResponseList(order.OrderItems), order.TotalAmount, DateTime.Now);
        }

        public Order ToEntity(OrderRequest orderRequest, Customer customer, List<Game> games, decimal totalAmount)
        {
            var entity = new Order();
            entity.Customer = customer;
            entity.OrderItems = _orderItemMapper.ToEnitytList(orderRequest.OrderItems, games);
            entity.TotalAmount = totalAmount;

            return entity;
        }

        public List<OrderResponse> ToResponseList(List<Order> listOrders)
        {
            List<OrderResponse> orederItems = [];

            foreach (var i in listOrders)
            {
                orederItems.Add(ToResponse(i));
            }

            return orederItems;
        }
    }
}