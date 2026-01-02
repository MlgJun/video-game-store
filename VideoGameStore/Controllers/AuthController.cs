using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VideoGameStore.Context;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;
using VideoGameStore.Services;

namespace VideoGameStore.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IUserService _userService;
        private readonly SignInManager<AspNetUser> _signInManager;

        public AuthController(AppDbContext dbContext, UserManager<AspNetUser> userManager,
            SignInManager<AspNetUser> signInManager, IUserService userService, ILogger<AuthController> logger) : base(dbContext, userManager, logger)
        {
            _userService = userService;
            _signInManager = signInManager;
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register(UserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.UserRole == UserRole.SELLER)
                return StatusCode(201, await _userService.CreateSeller(request));

            else
                return StatusCode(201, await _userService.CreateCustomer(request));
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(UserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ✅ ОДИН вызов — всё делает сам!
            var result = await _signInManager.PasswordSignInAsync(
                request.Username,  // Ищет по Username/Email автоматически!
                request.Password,
                isPersistent: true,
                lockoutOnFailure: false);

            return result.Succeeded
                ? Ok(new { message = "Login successful" })
                : Unauthorized("Invalid login or password");
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully" });
        }
    }
}
