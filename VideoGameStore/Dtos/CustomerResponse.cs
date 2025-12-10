using System.ComponentModel.DataAnnotations;

namespace VideoGameStore.Dtos
{
    public record class CustomerResponse(long Id, string Username, string Email, CartResponse CartResponse);
}
