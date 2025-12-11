using VideoGameStore.Dtos;

namespace VideoGameStore.Services
{
    public interface IOrderService
    {
        public Task<Page<OrderResponse>> FindAllByUserId(long userId, Pageable pageable);
        public Task<OrderResponse> Create(long userId, OrderRequest request);
    }
}
