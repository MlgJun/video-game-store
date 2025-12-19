namespace VideoGameStore.Dtos
{
    public record class SellerGameResponse(long Id, string Title, decimal Price, List<string> Keys, string ImageUrl);
}
