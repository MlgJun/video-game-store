using VideoGameStore.Entities;

namespace VideoGameStore.Dtos
{
    public class UserRequest
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public UserRole UserRole { get; set; }
    }
}
