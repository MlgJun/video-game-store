using VideoGameStore.Entities;

namespace VideoGameStore.Dtos
{
    public record class SellerResponse(long Id, string Login, List<GameResponse>? Games);
}
