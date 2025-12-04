namespace VideoGameStore.Dtos
{
    public record class OrderItemRequest (long GameId, int Quantity);
}
