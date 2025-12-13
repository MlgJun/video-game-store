using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VideoGameStore.Context;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;
using VideoGameStore.Services;

namespace VideoGameStore.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService, AppDbContext dbContext, UserManager<AspNetUser> userManager) : base(dbContext, userManager)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<Page<OrderResponse>>> GetPageOrders([FromQuery] Pageable pageable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetCurrentDomainUserAsync();

            return Ok(await _orderService.FindAllByCustomerId(user.Id, pageable));
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> CreateOrder([FromBody] OrderRequest order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Customer customer = (Customer)await GetCurrentDomainUserAsync();

            return StatusCode(201, await _orderService.Create(customer, order));
        }
    }
}
