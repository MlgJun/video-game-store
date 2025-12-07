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

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Route("api/orders/")]
        [HttpGet]
        public IActionResult GetPageOrders([FromQuery]Pageable pageable)
        {
            
        }

    }
}
