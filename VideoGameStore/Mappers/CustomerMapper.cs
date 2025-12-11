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

        public CustomerResponse ToResponse(Customer customer, AspNetUser user)
        {
            return new CustomerResponse(customer.Id, user.UserName, user.Email, _cartMapper.ToResponse(customer.Cart));
        }

        public Customer ToEntity(UserRequest customerRequest, Cart cart)
        {
            var customer = new Customer
            {
                Cart = cart,
                CreatedAt = DateTime.UtcNow
            };

            cart.Customer = customer;

            return customer;
        }
    }
}
