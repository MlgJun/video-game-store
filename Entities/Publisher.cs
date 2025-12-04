namespace VideoGameStore.Entities
{
    public class Publisher
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public virtual List<Developer>? Developers { get; set; }
        public virtual List<Game>? Games { get; set; }
    }
}
