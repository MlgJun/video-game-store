using VideoGameStore.Dtos;
using VideoGameStore.Entities;

namespace VideoGameStore.Services
{
    public interface IOrderService
    {
        public Task<Page<OrderResponse>> FindAllByCustomerId(long customerId, Pageable pageable);
        public Task<OrderResponse> Create(Customer customer, OrderRequest request);
    }
}
