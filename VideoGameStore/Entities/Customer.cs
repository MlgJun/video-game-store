namespace VideoGameStore.Entities
{
    public class Customer : User
    {
        public virtual Cart Cart { get; set; } = null!;
        public virtual List<Order>? Orders { get; set; }
    }
}
