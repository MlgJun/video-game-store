namespace VideoGameStore.Entities
{
    public class Seller : User
    {
        public virtual List<Game> Games { get; set; } = new();
    }
}
