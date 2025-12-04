namespace VideoGameStore.Entities
{
    public class Game
    {
        public long Id { get; set; }
        public string DeveloperTitle { get; set; } = null!;
        public string PublisherTitle { get; set; } = null!;
        public decimal Price { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }
}
