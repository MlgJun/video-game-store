namespace VideoGameStore.Entities
{
    public abstract class User
    {
        public long Id { get; set; }
        public UserRole Role { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
      
    }
}
