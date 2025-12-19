using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VideoGameStore.Context;
using VideoGameStore.Dtos;
using VideoGameStore.Entities;
using VideoGameStore.Exceptions;
using VideoGameStore.Mappers;
using VideoGameStore.Utils;

namespace VideoGameStore.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly OrderMapper _mapper;

        public OrderService(AppDbContext context, OrderMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderResponse> Create(Customer customer, OrderRequest request)
        {
            Dictionary<long, int> gameIdsWithQuantity = request.OrderItems.ToDictionary(item => item.GameId, item => item.Quantity);
            List<Game> games = await _context.Games
                     .Where(g => gameIdsWithQuantity.Keys.Contains(g.Id))
                     .ToListAsync();

            if (games.Count != gameIdsWithQuantity.Count)
                throw new EntityNotFound($"Some game not found");

            Dictionary<long, List<string>> keysByGameId = DeleteKeys(gameIdsWithQuantity);

            decimal totalAmount = games.Sum(game => game.Price * gameIdsWithQuantity[game.Id]);

            Order order = _mapper.ToEntity(request, customer, games, totalAmount, keysByGameId);

            await _context.Orders.AddAsync(order);

            await _context.SaveChangesAsync();

            return _mapper.ToResponse(order);
        }

        public async Task<Page<OrderResponse>> FindAllByCustomerId(long customerId, Pageable pageable)
        {
            return await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Game).ToPageAsync(pageable, order => _mapper.ToResponse(order),
                new List<Expression<Func<Order, bool>>> { o => o.Customer.Id == customerId });
        }

        private Dictionary<long, List<string>> DeleteKeys(Dictionary<long, int> gameIdsWithQuantity)
        {
            List<long> gameIdList = gameIdsWithQuantity.Keys.ToList();

            Dictionary<long, List<string>> keysByGameId = _context.Keys
                .Where(k => gameIdList.Contains(k.GameId))
                .GroupBy(k => k.GameId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(k => k.Value).ToList()
                );

            foreach (KeyValuePair<long, int> kvp in gameIdsWithQuantity)
            {
                long gameId = kvp.Key;
                int quantity = kvp.Value;

                if (!keysByGameId.ContainsKey(gameId) || keysByGameId[gameId].Count < quantity)
                    throw new InvalidOperationException(
                        $"Not enough keys for game {gameId}. Need {quantity}, found {keysByGameId.GetValueOrDefault(gameId)?.Count ?? 0}");

                keysByGameId[gameId] = keysByGameId[gameId].Take(quantity).ToList();
            }

            HashSet<string> usedKeyValues = keysByGameId.Values.SelectMany(v => v).ToHashSet();

            List<Key> keysToDeleteFromDb = _context.Keys
                .Where(k => gameIdList.Contains(k.GameId) && usedKeyValues.Contains(k.Value))
                .ToList();

            _context.Keys.RemoveRange(keysToDeleteFromDb);

            return keysByGameId;
        }


    }
}
