namespace VideoGameStore.Entities
{
    public class Cart
    {
        public long Id { get; set; }
        public virtual User User { get; set; } = null!;
        public long UserId { get; set; }
        public List<CartItem>? CartItems { get; set; }
    }
}
