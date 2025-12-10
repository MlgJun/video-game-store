using VideoGameStore.Dtos;

namespace VideoGameStore.Services
{
    public interface IUserService
    {
        public Task<SellerResponse> CreateSeller(UserRequest request);
        public Task<CustomerResponse> CreateCustomer(UserRequest request);
    }
}
