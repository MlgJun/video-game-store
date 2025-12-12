namespace VideoGameStore.Entities
{
    public class Cart : BaseEntity
    {
        public virtual Customer Customer { get; set; } = null!;
        public long CustomerId { get; set; }
        public List<CartItem> CartItems { get; set; } = new();
    }
}
