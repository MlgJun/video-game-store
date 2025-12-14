namespace VideoGameStore.Entities
{
    public class Order : BaseEntity
    {
        public virtual Customer Customer { get; set; } = null!;
        public virtual List<OrderItem> OrderItems { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
