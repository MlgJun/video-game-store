using VideoGameStore.Entities;

namespace VideoGameStore.Dtos
{
    public record class UserResponse(string login, CartResponse cartResponse, UserRole userRole);
}
