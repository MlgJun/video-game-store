namespace VideoGameStore.Entities
{
    public class Game : BaseEntity
    {
        public string DeveloperTitle { get; set; } = null!;
        public string PublisherTitle { get; set; } = null!;
        public decimal Price { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public Seller Seller { get; set; } = null!;
        public List<Genre> Genres { get; set; } = new();
        public List<Key> Keys { get; set; } = new();
    }
}
