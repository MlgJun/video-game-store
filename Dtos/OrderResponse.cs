using VideoGameStore.Entities;

namespace VideoGameStore.Dtos
{
    public class OrderResponse
    {
        public  List<OrderItemResponse> OrderItems { get; set; } = null!;
        public decimal TotalAmount { get; set; }
    }
}
