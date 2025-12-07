namespace VideoGameStore.Dtos
{
    public record class OrderResponse (List<OrderItemResponse> OrderItems,  decimal TotalAmount);
}
