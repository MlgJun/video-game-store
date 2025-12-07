using VideoGameStore.Dtos;

namespace VideoGameStore.Services
{
    public interface IUserService
    {
        public SellerResponse CreateSeller(UserRequest request);
        public CustomerResponse CreateCustomer(UserRequest request);
    }
}
