using VideoGameStore.Entities;

namespace VideoGameStore.Dtos
{
    public class UserResponse
    {
        public string Login { get; set; } = null!;
        public CartResponse CartResponse { get; set; } = null!;
        public UserRole UserRole { get; set; }
    }
}
