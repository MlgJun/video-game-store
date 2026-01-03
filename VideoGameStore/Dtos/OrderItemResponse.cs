namespace VideoGameStore.Dtos
{
    public record class OrderItemResponse (string GameTitle, int Quantity, decimal Price, List<string> Keys);
}
