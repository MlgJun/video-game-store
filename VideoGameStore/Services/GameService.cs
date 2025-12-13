using VideoGameStore.Context;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;
using VideoGameStore.Mappers;
using VideoGameStore.Utils;
using VideoGameStore.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace VideoGameStore.Services
{
    public class GameService : IGameService
    {
        private readonly GameMapper _gameMapper;
        private readonly AppDbContext _context;
        private readonly IKeyService _keyService;

        public GameService(AppDbContext context, GameMapper gameMapper, IKeyService keyService)
        {
            _context = context;
            _gameMapper = gameMapper;
            _keyService = keyService;
        }

        public async Task<GameResponse> Create(GameRequest request)
        {
            Game game = _gameMapper.ToEntity(request);

            List<Key> keys = await _keyService.CreateKeysAsync(request.Keys, game);

            await _context.Games.AddAsync(game);
            await _context.Keys.AddRangeAsync(keys);
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
            return await _context.Games.Include(g => g.Keys).Include(g => g.Genres).ToPageAsync(pageable, g => _gameMapper.ToResponse(g), 
                new List<Predicate<Game>>() { g => g.Keys.Any() });
        }

        public async Task<Page<GameResponse>> FindAllByFilter(Pageable pageable, FilterRequest filter)
        {
            List<Predicate<Game>> predicates = new List<Predicate<Game>>();

            if(filter.MinPrice > 0)
                predicates.Add(g => g.Price >= filter.MinPrice);

            if (filter.MaxPrice > 0)
                predicates.Add(g => g.Price >= filter.MaxPrice);

            if (!filter.GameTitle.IsNullOrEmpty())
                predicates.Add(g => g.Title.Contains(filter.GameTitle));

            if (!filter.Genres.IsNullOrEmpty())
                foreach (var genre in filter.Genres)
                    predicates.Add(g => g.Title.Contains(genre));

            return await _context.Games.Include(g => g.Keys).Include(g => g.Genres)
                .ToPageAsync(pageable, g => _gameMapper.ToResponse(g), predicates);
        }

        public async Task<Page<SellerGameResponse>> FindAllBySellerId(long sellerId, Pageable pageable)
        {
            return await _context.Games.AsNoTracking().Include(g => g.Keys).ToPageAsync(pageable, g => _gameMapper.ToResponseForSeller(g), 
                new List<Predicate<Game>>() { g => g.Seller.Id == sellerId });
        }

        public async Task<GameResponse> FindById(long gameId)
        {
            Game? game = await _context.Games.AsNoTracking().Include(g => g.Keys).Include(g => g.Genres)
               .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
                throw new EntityNotFound($"Game not found by id : {gameId}");

            return _gameMapper.ToResponse(game);
        }

        public async Task<GameResponse> Update(long gameId, long sellerId, GameRequest request)
        {
            Game? game = await _context.Games.Include(g => g.Keys).Include(g => g.Genres).FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
                throw new EntityNotFound($"Game not found by id : {gameId}");

            if (game.Seller.Id != sellerId)
                throw new BadRequest("You can only update your own games");
            
            _gameMapper.Update(request, game);

            await _context.SaveChangesAsync();

            return _gameMapper.ToResponse(game);
        }
    }
}
