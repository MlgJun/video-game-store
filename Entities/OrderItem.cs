namespace VideoGameStore.Entities
{
    public class OrderItem
    {
        public long Id { get; set; }
        public virtual Game Game { get; set; } = null!;
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }
    }
}
