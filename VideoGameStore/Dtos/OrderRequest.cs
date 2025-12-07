using System.ComponentModel.DataAnnotations;

namespace VideoGameStore.Dtos
{
    public record class OrderRequest ([MinLength(1)] List<OrderItemRequest> OrderItems);
}
