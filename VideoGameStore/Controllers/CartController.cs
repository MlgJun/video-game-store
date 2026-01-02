using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VideoGameStore.Context;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;
using VideoGameStore.Mappers;
using VideoGameStore.Services;

namespace VideoGameStore.Controllers
{
    [Route("api/carts")]
    [ApiController]
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;
        private readonly CartMapper _cartMapper;

        public CartController(ICartService cartService, AppDbContext dbContext, UserManager<AspNetUser> userManager, ILogger<CartController> logger, CartMapper cartMapper) 
            : base(dbContext, userManager, logger)
        {
            _cartService = cartService;
            _cartMapper = cartMapper;
        }


        [HttpDelete("items")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult> DeleteItems([FromBody] CartItemRequest cartItemRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetCurrentDomainUserAsync();

            return Ok(await _cartService.RemoveFromCart(((Customer)user).Cart, cartItemRequest));
        }


        [HttpPost("items")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult> AddItemToCart([FromBody] CartItemRequest cartItemRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetCurrentDomainUserAsync();

            return Ok(await _cartService.AddToCart(((Customer)user).Cart, cartItemRequest));
        }

        [HttpGet]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<ActionResult> GetCart()
        {
            var customer = await GetCurrentDomainUserAsync();
           
            return Ok(_cartMapper.ToResponse(((Customer)customer).Cart));
        }
    }
}
