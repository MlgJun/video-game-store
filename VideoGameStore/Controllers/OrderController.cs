using Microsoft.AspNetCore.Mvc;
using VideoGameStore.Dtos;
using VideoGameStore.Services;

namespace VideoGameStore.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        private long _psevdoUser = 1;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Route("api/orders/")]
        [HttpGet]
        public ActionResult<Page<OrderResponse>> GetPageOrders([FromQuery]Pageable pageable)
        {
            return Ok(_orderService.FindAllByUserId(_psevdoUser, pageable));
        }

        [Route("api/orders")]
        [HttpPost]
        public ActionResult CreateOrder([FromBody] OrderRequest order )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return StatusCode( 201, _orderService.Create(_psevdoUser, order));
        }

    }
}
