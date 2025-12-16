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
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
                throw new InvalidOperationException("Customer with this login already exists");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Cart cart = new Cart();
                Customer customer = _customerMapper.ToEntity(request, cart);

                await _context.Customers.AddAsync(customer);
                await _context.Carts.AddAsync(cart);

                await _context.SaveChangesAsync();

                AspNetUser user = await CreateIdentity(request, customer, "Customer");

                await transaction.CommitAsync();

                _logger.LogDebug($"Customer created with email and username: {request.Email}, {request.Username}");

                return _customerMapper.ToResponse(customer, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<SellerResponse> CreateSeller(UserRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
                throw new InvalidOperationException("Seller with this login already exists");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Seller seller = _sellerMapper.ToEntity(request);

                await _context.Sellers.AddAsync(seller);
                await _context.SaveChangesAsync();

                AspNetUser user = await CreateIdentity(request, seller, "Seller");

                await transaction.CommitAsync();

                _logger.LogDebug($"Seller created with email and username: {request.Email}, {request.Username}");

                return _sellerMapper.ToResponse(seller, user, new List<Game>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<AspNetUser> CreateIdentity(UserRequest request, User user, string role)
        {
            AspNetUser aspNetUser = new AspNetUser
            {
                Email = request.Email,
                UserName = request.Username,
            };

            var result = await _userManager.CreateAsync(aspNetUser, request.Password);

            if (!result.Succeeded)
            {
                string errors = string.Join(",", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create {role} : {errors}");
            }

            await _userManager.AddClaimAsync(aspNetUser, new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            await _userManager.AddClaimAsync(aspNetUser, new Claim(ClaimTypes.Role, role));
            await _userManager.AddToRoleAsync(aspNetUser, role);

            _logger.LogDebug($"User CREATED id: {user.Id})");

            return aspNetUser;
        }
    }
}
