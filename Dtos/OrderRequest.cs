namespace VideoGameStore.Dtos
{
    public class OrderRequest
    {
        public List<OrderItemRequest> OrderItems { get; set; } = null!;
    }
}
