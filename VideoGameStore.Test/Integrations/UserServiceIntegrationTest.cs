using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;
using VideoGameStore.Mappers;
using VideoGameStore.Services;
using VideoGameStore.Tests;

namespace VideoGameStore.Test.Integrations
{
    [Collection("Database collection")]
    public class UserServiceIntegrationTest : IntegrationTestBase
    {
        private IUserService _userService = null!;
        private UserManager<AspNetUser> _userManager = null!;
        private CustomerMapper _customerMapper = null!;
        private SellerMapper _sellerMapper = null!;

        public UserServiceIntegrationTest(MsSqlFixture fixture) : base(fixture)
        {
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _userService = GetService<IUserService>();
            _userManager = GetService<UserManager<AspNetUser>>();
            _customerMapper = GetService<CustomerMapper>();
            _sellerMapper = GetService<SellerMapper>();
        }

        private UserRequest CreateUserRequest(string email = "test@example.com", string username = "testuser",
            string password = "Password123!", UserRole role = UserRole.CUSTOMER)
        {
            return new UserRequest(email, username, password, role);
        }

        [Fact]
        public async Task CreateCustomer_WithValidRequest_ShouldCreateCustomerAndCart()
        {
            var request = CreateUserRequest();

            var result = await _userService.CreateCustomer(request);

            Assert.NotNull(result);
            Assert.Equal(request.Email, result.Email);
            Assert.Equal(request.Username, result.Username);

            var persistedCustomer = await DbContext.Customers.Include(c => c.Cart)
                .FirstOrDefaultAsync(c => c.Id == result.Id);

            Assert.NotNull(persistedCustomer);
            Assert.NotNull(persistedCustomer.Cart);

            var aspNetUser = await _userManager.FindByEmailAsync(request.Email);
            Assert.NotNull(aspNetUser);
            Assert.True(await _userManager.IsInRoleAsync(aspNetUser, "Customer"));
        }

        [Fact]
        public async Task CreateCustomer_WithDuplicateEmail_ShouldThrowInvalidOperationException()
        {
            var request = CreateUserRequest("duplicate@example.com", "user1");
            await _userService.CreateCustomer(request);

            var duplicateRequest = CreateUserRequest("duplicate@example.com", "differentuser");

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userService.CreateCustomer(duplicateRequest));

            Assert.Contains("already exists", exception.Message);
        }

        [Fact]
        public async Task CreateSeller_WithValidRequest_ShouldCreateSeller()
        {
            var request = CreateUserRequest("sellernew@example.com", "sellernew", "Password123!", UserRole.SELLER);

            var result = await _userService.CreateSeller(request);

            Assert.NotNull(result);
            Assert.Equal(request.Email, result.Email);
            Assert.Equal(request.Username, result.Username);

            var persistedSeller = await DbContext.Sellers
                .FirstOrDefaultAsync(s => s.Id == result.Id);

            Assert.NotNull(persistedSeller);

            var aspNetUser = await _userManager.FindByEmailAsync(request.Email);
            Assert.NotNull(aspNetUser);
            Assert.True(await _userManager.IsInRoleAsync(aspNetUser, "Seller"));
        }

        [Fact]
        public async Task CreateSeller_WithDuplicateEmail_ShouldThrowInvalidOperationException()
        {
            var request = CreateUserRequest("duplicate.seller@example.com", "seller1", "Password123!", UserRole.SELLER);
            await _userService.CreateSeller(request);

            var duplicateRequest = CreateUserRequest("duplicate.seller@example.com", "seller2");

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userService.CreateSeller(duplicateRequest));

            Assert.Contains("already exists", exception.Message);
        }

        [Fact]
        public async Task CreateSeller_ShouldPersistInDatabase()
        {
            var request = CreateUserRequest("seller2@example.com", "seller2", "Password123!", UserRole.SELLER);

            var result = await _userService.CreateSeller(request);

            Assert.NotNull(result);

            var persistedSeller = await DbContext.Sellers
                .FirstOrDefaultAsync(s => s.Id == result.Id);

            Assert.NotNull(persistedSeller);
            Assert.Equal(result.Id, persistedSeller.Id);
        }

        [Fact]
        public async Task CreateCustomer_ShouldCreateIdentityUser()
        {
            var request = CreateUserRequest("identity.customer@example.com", "identitycustomer");

            var result = await _userService.CreateCustomer(request);

            var identityUser = await _userManager.FindByEmailAsync(request.Email);
            Assert.NotNull(identityUser);
            Assert.Equal(request.Email, identityUser.Email);
            Assert.Equal(request.Username, identityUser.UserName);
        }

        [Fact]
        public async Task CreateSeller_ShouldCreateIdentityUser()
        {
            var request = CreateUserRequest("identity.seller@example.com", "identityseller", "Password123!", UserRole.SELLER);

            var result = await _userService.CreateSeller(request);

            var identityUser = await _userManager.FindByEmailAsync(request.Email);
            Assert.NotNull(identityUser);
            Assert.Equal(request.Email, identityUser.Email);
            Assert.Equal(request.Username, identityUser.UserName);
        }

        [Fact]
        public async Task CreateCustomer_WithDuplicateUsername_ShouldThrowInvalidOperationException()
        {
            var request = CreateUserRequest("first@example.com", "duplicateusername");
            await _userService.CreateCustomer(request);

            var duplicateRequest = CreateUserRequest("second@example.com", "duplicateusername");

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userService.CreateCustomer(duplicateRequest));

            Assert.Contains("Failed to create", exception.Message);
        }
    }
}
