namespace VideoGameStore.Entities
{
    public class Genre : BaseEntity
    {
        public string Title { get; set; } = null!;
        public List<Game> Games { get; set; } = new();
    }
}
