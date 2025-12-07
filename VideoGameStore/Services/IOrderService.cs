using VideoGameStore.Dtos;

namespace VideoGameStore.Services
{
    public interface IOrderService
    {
        public Page<OrderResponse> FindAllByUserId(long userId);
        public OrderResponse Create(long userId, OrderRequest request);
    }
}
