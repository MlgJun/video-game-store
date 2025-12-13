namespace VideoGameStore.Entities
{
    public class Key : BaseEntity
    {
        public string Value { get; set; } = null!;
        public Game Game { get; set; } = null!;
        public long GameId { get; set; }
    }
}
