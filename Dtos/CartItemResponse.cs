using VideoGameStore.Entities;

namespace VideoGameStore.Dtos
{
    public class CartItemResponse
    {
        public string GameTitle { get; set; } = null!;
        public int Quantity { get; set; } = 1;
    }
}
