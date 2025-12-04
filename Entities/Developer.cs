namespace VideoGameStore.Entities
{
    public class Developer
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public virtual List<Game>? Games { get; set; }
        public virtual Publisher Publisher { get; set; } = null!;
    }
}
