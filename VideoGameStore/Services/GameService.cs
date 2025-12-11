using VideoGameStore.Context;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;
using VideoGameStore.Mappers;
using VideoGameStore.Utils;
using VideoGameStore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace VideoGameStore.Services
{
    public class GameService : IGameService
    {
        private readonly GameMapper _gameMapper;
        private readonly AppDbContext _context;

        public GameService(AppDbContext context, GameMapper gameMapper)
        {
            _context = context;
            _gameMapper = gameMapper;
        }

        public async Task<GameResponse> Create(GameRequest request)
        {
            Game game = _gameMapper.ToEntity(request);

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return _gameMapper.ToResponse(game);
        }

        public async Task<bool> Delete(long gameId, long sellerId)
        {
            Game? game = await _context.Games.FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
                throw new EntityNotFound($"Game not found by id : {gameId}");

            if (game.Seller.Id != sellerId)
                throw new BadRequest("You can only delete your own games");

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Page<GameResponse>> FindAll(Pageable pageable)
        {
            return await _context.Games.ToPageAsync(pageable, g => _gameMapper.ToResponse(g), null);
        }

        public async Task<Page<GameResponse>> FindAllBySellerId(long sellerId, Pageable pageable)
        {
            return await _context.Games.ToPageAsync(pageable, g => _gameMapper.ToResponse(g), g => g.Seller.Id == sellerId);
        }

        public async Task<GameResponse> FindById(long gameId)
        {
            Game? game = await _context.Games.AsNoTracking().FirstOrDefaultAsync(g => g.Id == gameId);

            if(game == null)
                throw new EntityNotFound($"Game not found by id : {gameId}");

            return _gameMapper.ToResponse(game);
        }

        public async Task<GameResponse> Update(long gameId, long sellerId, GameRequest request)
        {
            Game? game = await _context.Games.FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
                throw new EntityNotFound($"Game not found by id : {gameId}");

            if (game.Seller.Id != sellerId)
                throw new BadRequest("You can only update your own games");

            game.Title = request.Title;
            game.Description = request.Description;
            game.Price = request.Price;
            game.DeveloperTitle = request.DeveloperTitle;
            game.PublisherTitle = request.PublisherTitle;

            await _context.SaveChangesAsync();

            return _gameMapper.ToResponse(game);
        }
    }
}
