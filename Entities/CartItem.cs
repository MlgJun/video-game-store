namespace VideoGameStore.Entities
{
    public class CartItem
    {
        public long Id { get; set; }
        public Game Game { get; set; } = null!;
        public int Quantity { get; set; } = 1;
    }
}
