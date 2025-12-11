using Microsoft.AspNetCore.Mvc;
using VideoGameStore.Dtos;
using VideoGameStore.Services;

namespace VideoGameStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        private long _psevdoUser = 1;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Route("api/cart/items")]
        [HttpDelete]
        public ActionResult DeleteItems([FromQuery] CartItemRequest cartItemRequest)
        {
            return Ok(_cartService.RemoveFromCart(_psevdoUser, cartItemRequest));
        }
    }
}
