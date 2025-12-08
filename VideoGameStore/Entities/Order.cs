namespace VideoGameStore.Entities
{
    public class Order
    {
        public long Id { get; set; }
        public virtual Customer Customer { get; set; } = null!;
        public virtual List<OrderItem> OrderItems { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
