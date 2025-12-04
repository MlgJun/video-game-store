namespace VideoGameStore.Entities
{
    public class Cart
    {
        public long Id { get; set; }
        public virtual Customer Customer { get; set; } = null!;
        public long CustomerId { get; set; }
        public List<CartItem>? CartItems { get; set; }
    }
}
