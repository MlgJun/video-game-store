namespace VideoGameStore.Entities
{
    public class Order
    {
        public long Id { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual List<OrderItem> OrderItems { get; set; } = null!;
        public decimal TotalAmount { get; set; }
    }
}
