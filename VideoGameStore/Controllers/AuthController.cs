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

            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                return Unauthorized("Invalid login or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Unauthorized("Invalid login or password");

            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            await HttpContext.SignInAsync(
                IdentityConstants.ApplicationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });

            return Ok(new { message = "Login successful" });
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
