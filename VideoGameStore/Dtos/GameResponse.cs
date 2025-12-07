namespace VideoGameStore.Dtos
{
    public record class GameResponse (long Id, string PublisherTitle, string DeveloperTitle, decimal Price, string Title, string? Description);
   
}
