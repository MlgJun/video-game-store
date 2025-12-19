using VideoGameStore.Context;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;
using VideoGameStore.Mappers;
using VideoGameStore.Utils;
using VideoGameStore.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace VideoGameStore.Services
{
    public class GameService : IGameService
    {
        private readonly GameMapper _gameMapper;
        private readonly AppDbContext _context;
        private readonly IKeyService _keyService;
        private readonly IFileStorage _fileStorage;

        public GameService(AppDbContext context, GameMapper gameMapper, IKeyService keyService, IFileStorage fileStorage)
        {
            _context = context;
            _gameMapper = gameMapper;
            _keyService = keyService;
            _fileStorage = fileStorage;
        }

        public async Task<GameResponse> Create(GameWithKeysRequest request, Seller seller)
        {
            string imageUrl = PathGenerator.GenerateFilePath(request.Image);

            await _fileStorage.UploadPhotoAsync(imageUrl, request.Image);

            Game game = _gameMapper.ToEntity(request, seller, imageUrl);

            List<Key> keys = await _keyService.CreateKeysAsync(request.Keys, game);

            await _context.Games.AddAsync(game);
            await _context.Keys.AddRangeAsync(keys);
            await _context.SaveChangesAsync();

            return _gameMapper.ToResponse(game, _fileStorage.Url(game.ImageUrl));
        }

        public async Task<GameResponse> AddKeys(long gameId, IFormFile keys)
        {
            Game? game = await _context.Games.FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
                throw new EntityNotFound($"Game not found by id : {gameId}");

            await _keyService.CreateKeysAsync(keys, game);

            await _context.SaveChangesAsync();

            return _gameMapper.ToResponse(game, _fileStorage.Url(game.ImageUrl));

        }

        public async Task<bool> Delete(long gameId, long sellerId)
        {
            Game? game = await _context.Games.FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
                throw new EntityNotFound($"Game not found by id : {gameId}");

            if (game.Seller.Id != sellerId)
                throw new BadRequest("You can only delete your own games");

            await _fileStorage.DeletePhotoAsync(game.ImageUrl);

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Page<GameResponse>> FindAll(Pageable pageable)
        {
            return await _context.Games.Include(g => g.Keys).Include(g => g.Genres).ToPageAsync(pageable, 
                g => _gameMapper.ToResponse(g, _fileStorage.Url(g.ImageUrl)), 
                new List<Expression<Func<Game, bool>>>() { g => g.Keys.Any() });
        }

        public async Task<Page<GameResponse>> FindAllByFilter(Pageable pageable, FilterRequest filter)
        {
            List<Expression<Func<Game, bool>>> predicates = new List<Expression<Func<Game, bool>>>();

            if(filter.MinPrice > 0)
                predicates.Add(g => g.Price >= filter.MinPrice);

            if (filter.MaxPrice > 0)
                predicates.Add(g => g.Price >= filter.MaxPrice);

            if (!filter.GameTitle.IsNullOrEmpty())
                predicates.Add(g => g.Title.Contains(filter.GameTitle));

            if (!filter.Genres.IsNullOrEmpty())
            {
                var genres = filter.Genres!;
                predicates.Add(game =>
                    game.Genres.Any(gameGenre =>
                        genres.Any(filterGenre =>
                            gameGenre.Title.Contains(filterGenre))));
            }

            return await _context.Games.Include(g => g.Keys).Include(g => g.Genres)
                .ToPageAsync(pageable, g => _gameMapper.ToResponse(g, _fileStorage.Url(g.ImageUrl)), predicates);
        }

        public async Task<Page<SellerGameResponse>> FindAllBySellerId(long sellerId, Pageable pageable)
        {
            return await _context.Games.AsNoTracking().Include(g => g.Keys).ToPageAsync(pageable, 
                g => _gameMapper.ToResponseForSeller(g, _fileStorage.Url(g.ImageUrl)), 
                new List<Expression<Func<Game, bool>>>() { g => g.Seller.Id == sellerId });
        }

        public async Task<GameResponse> FindById(long gameId)
        {
            Game? game = await _context.Games.AsNoTracking().Include(g => g.Keys).Include(g => g.Genres)
               .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
                throw new EntityNotFound($"Game not found by id : {gameId}");

            return _gameMapper.ToResponse(game, _fileStorage.Url(game.ImageUrl));
        }

        public async Task<GameResponse> Update(long gameId, long sellerId, GameRequest request)
        {
            Game? game = await _context.Games.Include(g => g.Keys).Include(g => g.Genres).FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
                throw new EntityNotFound($"Game not found by id : {gameId}");

            if (game.Seller.Id != sellerId)
                throw new BadRequest("You can only update your own games");

            string imageUrl = PathGenerator.GenerateFilePath(request.Image);

            await _fileStorage.DeletePhotoAsync(game.ImageUrl);

            await _fileStorage.UploadPhotoAsync(imageUrl, request.Image);

            _gameMapper.Update(request, game, imageUrl);

            await _context.SaveChangesAsync();

            return _gameMapper.ToResponse(game, _fileStorage.Url(game.ImageUrl));
        }
    }
}
