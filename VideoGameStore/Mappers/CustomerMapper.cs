using VideoGameStore.Dtos;
using VideoGameStore.Entities;

namespace VideoGameStore.Mappers
{
    public class CustomerMapper
    {
        private readonly CartMapper _cartMapper;

        public CustomerMapper(CartMapper cartMapper)
        {
            _cartMapper = cartMapper;
        }

        public CustomerResponse ToResponse(Customer customer)
        {
            return new CustomerResponse(customer.Id,customer.Login, _cartMapper.ToResponse(customer.Cart));
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
