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

        protected BaseController(AppDbContext dbContext, UserManager<AspNetUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        protected async Task<User> GetCurrentDomainUserAsync()
        {
            long identityUserId = long.Parse(
                User.FindFirst("UserId")?.Value
                ?? throw new UnauthorizedAccessException("User not authenticated"));

            var role = User.FindFirst(ClaimTypes.Role)?.Value
                      ?? throw new UnauthorizedAccessException();

            return role switch
            {
                "Customer" => await _dbContext.Customers
                    .FirstOrDefaultAsync(c => c.Id == identityUserId)
                    ?? throw new KeyNotFoundException(),

                "Seller" => await _dbContext.Sellers
                    .FirstOrDefaultAsync(s => s.Id == identityUserId)
                    ?? throw new KeyNotFoundException(),

                _ => throw new UnauthorizedAccessException("Unknown role")
            };
        }
    }
}
