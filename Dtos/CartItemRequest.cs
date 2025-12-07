using System.ComponentModel.DataAnnotations;

namespace VideoGameStore.Dtos
{
    public record class CartItemRequest ([Range(1, long.MaxValue)] long GameId, [Range(1, int.MaxValue)] int Quantity);
}
