namespace VideoGameStore.Dtos
{
    public record class OrderItemResponse (string gameTitle, int quantity, decimal price);
 
}
