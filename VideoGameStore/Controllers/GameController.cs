using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
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

        public GameController(IGameService gameService, AppDbContext dbContext, UserManager<AspNetUser> userManager, ILogger<GameController> logger)
            : base(dbContext, userManager, logger)
        {
            _gameService = gameService;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetGame(long id)
        {
            return Ok(await _gameService.FindById(id));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetGames([FromQuery] Pageable pageable, [FromQuery] FilterRequest? filter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (filter != null)
                return Ok(await _gameService.FindAllByFilter(pageable, filter));

            else
                return Ok(await _gameService.FindAll(pageable));
        }

        [HttpGet("my")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult> GetMyGames([FromQuery] Pageable pageable)
        {
            var seller = await GetCurrentDomainUserAsync();

            return Ok(await _gameService.FindAllBySellerId(seller.Id, pageable));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult> DeleteGame(long id)
        {
            var seller = await GetCurrentDomainUserAsync();

            await _gameService.Delete(id, seller.Id);
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UpdateGame(long id, [FromForm] GameRequest gameRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var seller = await GetCurrentDomainUserAsync();

            return Ok(await _gameService.Update(id, seller.Id, gameRequest));
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> CreateGame([FromForm] GameWithKeysRequest gameRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Seller seller = await GetCurrentDomainUserAsync() as Seller;

            return Ok(await _gameService.Create(gameRequest, seller));
        }

        [HttpPost("{id}/keys")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult> AddKeys(long id, IFormFile keys)
        {
            return Ok(await _gameService.AddKeys(id, keys));
        }
    }
}
