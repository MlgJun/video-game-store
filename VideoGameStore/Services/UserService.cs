using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using VideoGameStore.Context;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;
using VideoGameStore.Mappers;

namespace VideoGameStore.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly AppDbContext _context;
        private readonly CustomerMapper _customerMapper;
        private readonly SellerMapper _sellerMapper;

        public UserService(UserManager<AspNetUser> userManager, ILogger<UserService> logger, AppDbContext context,
            CustomerMapper customerMapper, SellerMapper sellerMapper)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _customerMapper = customerMapper;
            _sellerMapper = sellerMapper;
        }

        public async Task<CustomerResponse> CreateCustomer(UserRequest request)
        {
            // 1. ПЕРВЫМ Identity!
            var aspNetUser = new AspNetUser
            {
                Email = request.Email,
                UserName = request.Username
            };
            var result = await _userManager.CreateAsync(aspNetUser, request.Password);
            if (!result.Succeeded) throw new Exception("Identity failed");

            await _userManager.AddToRoleAsync(aspNetUser, "CUSTOMER");

            // 2. Customer с ID = AspNetUser.Id
            var customer = new Customer
            {
                Id = aspNetUser.Id,  // ← ОДИН ID!
                CreatedAt = DateTime.UtcNow
            };
            var cart = new Cart { CustomerId = aspNetUser.Id };

            await _context.Customers.AddAsync(customer);
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            return _customerMapper.ToResponse(customer, aspNetUser);
        }


        public async Task<SellerResponse> CreateSeller(UserRequest request)
        {
            var aspNetUser = new AspNetUser
            {
                Email = request.Email,
                UserName = request.Username
            };

            var result = await _userManager.CreateAsync(aspNetUser, request.Password);
            if (!result.Succeeded) throw new Exception(result.Errors.First().Description);

            await _userManager.AddToRoleAsync(aspNetUser, "SELLER");

            // Seller.Id = AspNetUser.Id
            var seller = new Seller
            {
                Id = aspNetUser.Id,  // ← ОДИН ID!
                CreatedAt = DateTime.UtcNow
            };

            await _context.Sellers.AddAsync(seller);
            await _context.SaveChangesAsync();

            return _sellerMapper.ToResponse(seller, aspNetUser, new Dictionary<Game, string>());
        }

        private async Task<AspNetUser> CreateIdentity(UserRequest request, User user, string role)
        {
            AspNetUser aspNetUser = new AspNetUser
            {
                Email = request.Email,
                UserName = request.Username,  // Логин по Username!
                NormalizedUserName = request.Username.ToUpperInvariant(),
                NormalizedEmail = request.Email.ToUpperInvariant()
            };

            var result = await _userManager.CreateAsync(aspNetUser, request.Password);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to create {role}: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            // ✅ ТОЛЬКО РОЛЬ! Identity сам создаст NameIdentifier claim
            await _userManager.AddToRoleAsync(aspNetUser, role);

            _logger.LogError($"User CREATED AspNetUser.Id={aspNetUser.Id}, DomainUser.Id={user.Id}");
            return aspNetUser;
        }
    }
}
