using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VideoGameStore.Context;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;
using VideoGameStore.Services;

namespace VideoGameStore.Controllers
{
    [Route("api/games")]
    [ApiController]
    public class GameController : BaseController
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService, AppDbContext dbContext, UserManager<AspNetUser> userManager) : base(dbContext, userManager)
        {
            _gameService = gameService;
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetGame(long id)
        {
            return Ok(await _gameService.FindById(id));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult> DeleteGame(long id)
        {
            await _gameService.Delete(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult> UpdateGame(long id, [FromBody] GameRequest gameRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await _gameService.Update(id, gameRequest));
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult> CreateGame([FromBody] GameRequest gameRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await _gameService.Create(gameRequest));
        }

    }
}
