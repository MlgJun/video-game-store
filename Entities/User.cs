namespace VideoGameStore.Entities
{
    public class User
    {
        public long Id { get; set; }
        public UserRole Role { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public virtual Cart Cart { get; set; } = null!;
        public List<Order>? Orders { get; set; }

    }
}
