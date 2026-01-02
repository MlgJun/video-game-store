namespace VideoGameStore.Dtos
{
    public record class CartItemResponse(long GameId, string GameTitle, int Quantity, decimal Price);
}
