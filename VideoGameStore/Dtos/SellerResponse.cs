namespace VideoGameStore.Dtos
{
    public record class SellerResponse(long Id, string Username, string Email, List<GameResponse>? Games);
}
