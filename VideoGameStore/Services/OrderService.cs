using Microsoft.EntityFrameworkCore;
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
            List<long> gameIds = request.OrderItems.Select(item => item.GameId).ToList();
            List<Game> games = _context.Games.Where(g => gameIds.Contains(g.Id)).ToList();  
            
            if (games.Count != gameIds.Count)
                throw new EntityNotFound("Some game not found");

            Queue<string> queue = DeleteKeys(gameIds);

            decimal totalAmount = 0;

            foreach (var game in games)
            {
                totalAmount += game.Price;
            }

            Order order = _mapper.ToEntity(request, customer, games, totalAmount, queue);

            await _context.Orders.AddAsync(order);

            await _context.SaveChangesAsync();

            return _mapper.ToResponse(order);
        }

        public async Task<Page<OrderResponse>> FindAllByCustomerId(long customerId, Pageable pageable)
        {
            return await _context.Orders.ToPageAsync(pageable, order => _mapper.ToResponse(order),
                new List<Predicate<Order>> { o => o.Customer.Id == customerId });
        } 

        private Queue<string> DeleteKeys(List<long> gameIds)
        {
            List<Key> keys = _context.Keys.Where(k => gameIds.Contains(k.GameId)).ToList();

            Queue<string> queue = new Queue<string>(keys.Select(k => k.Value).ToList());

            _context.Keys.RemoveRange(keys);
            return queue;
        }
    }
}
