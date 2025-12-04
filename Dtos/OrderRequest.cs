namespace VideoGameStore.Dtos
{
    public record class OrderRequest (List<OrderItemRequest> OrderItems);
}
