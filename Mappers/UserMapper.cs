using VideoGameStore.Dtos;
using VideoGameStore.Entities;

namespace VideoGameStore.Mappers
{
    public class UserMapper
    {
        private CartMapper _cartMapper;
        public UserResponse ToResponse(User user)
        {
            return new UserResponse(user.Login, _cartMapper.ToResponse(user.Cart), user.Role);
        }
    }
}
