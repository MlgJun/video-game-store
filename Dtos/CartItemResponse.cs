using System.ComponentModel.DataAnnotations;

namespace VideoGameStore.Dtos
{
    public record class CartItemResponse(string GameTitle, int Quantity);
}
