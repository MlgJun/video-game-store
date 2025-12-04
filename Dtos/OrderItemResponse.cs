namespace VideoGameStore.Dtos
{
    public class OrderItemResponse
    {
        public  string GameTitle { get; set; } = null!;
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }
    }
}
