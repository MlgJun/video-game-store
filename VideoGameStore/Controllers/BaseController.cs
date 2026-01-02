using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VideoGameStore.Context;
using VideoGameStore.Entities;

namespace VideoGameStore.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly AppDbContext _dbContext;
        protected readonly UserManager<AspNetUser> _userManager;
        protected readonly ILogger<BaseController> _logger;

        protected BaseController(AppDbContext dbContext, UserManager<AspNetUser> userManager, ILogger<BaseController> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
        }

        protected async Task<User> GetCurrentDomainUserAsync()
        {
            // ✅ Identity САМ ставит AspNetUser.Id в claim
            var identityUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User not authenticated");

            if (!long.TryParse(identityUserIdString, out long identityUserId))
                throw new UnauthorizedAccessException("Invalid user ID");

            _logger.LogInformation("Identity id: {Id}", identityUserId);

            var roles = await _userManager.GetRolesAsync(
                await _userManager.GetUserAsync(User)) ?? new List<string>();

            var role = roles.FirstOrDefault();
            if (string.IsNullOrEmpty(role))
                throw new UnauthorizedAccessException("No role assigned");

            return role switch
            {
                "CUSTOMER" => await _dbContext.Customers
                    .Include(c => c.Cart)
                    .ThenInclude(c => c.CartItems)
                    .ThenInclude(ci => ci.Game)
                    .FirstOrDefaultAsync(c => c.Id == identityUserId)
                    ?? throw new UnauthorizedAccessException("Customer not found"),

                "SELLER" => await _dbContext.Sellers
                    .FirstOrDefaultAsync(s => s.Id == identityUserId)
                    ?? throw new UnauthorizedAccessException("Seller not found"),

                _ => throw new UnauthorizedAccessException($"Unknown role: {role}")
            };
        }
    }
}
