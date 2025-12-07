namespace VideoGameStore.Entities
{
    public class CartItem
    {
        public long Id { get; set; }
        public virtual Game Game { get; set; } = null!;
        public virtual Cart Cart { get; set; } = null!;
        public long CartId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
