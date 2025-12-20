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
            long identityUserId = long.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User not authenticated"));

            _logger.LogError($"Identity id :{User.FindFirstValue(ClaimTypes.NameIdentifier)})");

            var role = User.FindFirstValue(ClaimTypes.Role)
                      ?? throw new UnauthorizedAccessException("User not authenticated");

            return role switch
            {
                "Customer" => await _dbContext.Customers
                    .Include(c => c.Cart)
                    .ThenInclude(c => c.CartItems)
                    .ThenInclude(ci => ci.Game)
                    .FirstOrDefaultAsync(c => c.Id == identityUserId)
                    ?? throw new UnauthorizedAccessException("Customer not authenticated"),

                "Seller" => await _dbContext.Sellers
                    .FirstOrDefaultAsync(s => s.Id == identityUserId)
                    ?? throw new UnauthorizedAccessException("Seller not authenticated"),

                _ => throw new UnauthorizedAccessException("Unknown role")
            };
        }
    }
}
