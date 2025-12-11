using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VideoGameStore.Context;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;
using VideoGameStore.Services;

namespace VideoGameStore.Controllers
{
    [Route("api/carts")]
    [ApiController]
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService, AppDbContext dbContext, UserManager<AspNetUser> userManager) : base(dbContext, userManager)
        {
            _cartService = cartService;
        }


        [HttpDelete("items")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> DeleteItems([FromQuery] CartItemRequest cartItemRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetCurrentDomainUserAsync();

            return Ok(_cartService.RemoveFromCart(((Customer)user).Cart.Id, cartItemRequest));
        }


        [HttpPost("items")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> AddItemToCart([FromBody] CartItemRequest cartItemRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetCurrentDomainUserAsync();

            return Ok(_cartService.AddToCart(((Customer)user).Cart.Id, cartItemRequest));
        }
    }
}
