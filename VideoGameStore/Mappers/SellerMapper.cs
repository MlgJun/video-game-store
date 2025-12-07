using VideoGameStore.Dtos;
using VideoGameStore.Entities;

namespace VideoGameStore.Mappers
{
    public class SellerMapper
    {
        private readonly GameMapper _gameMapper;

        public SellerMapper(GameMapper gameMapper)
        {
            _gameMapper = gameMapper;
        }

        public SellerResponse ToResponse(Seller seller, List<Game> games)
        {
            return new SellerResponse(seller.Id, seller.Login, _gameMapper.ToResponseList(games));
        }

        public Customer ToEntity(UserRequest customerRequest, Cart cart, string password)
        {
            var customer = new Customer();

            customer.Login = customerRequest.Login;
            customer.Password = password;
            customer.Cart = cart;
            customer.Role = customerRequest.UserRole;

            return customer;
        }
    }
}
