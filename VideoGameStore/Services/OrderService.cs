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
            List<long> ids = request.OrderItems.Select(item => item.GameId).ToList();
            List<Game> games = _context.Games.Where(g => ids.Contains(g.Id)).ToList();

            if (games.Count != ids.Count)
                throw new EntityNotFound("Some game not found");

            decimal totalAmount = 0;

            foreach (var game in games) 
            {
                totalAmount += game.Price;
            }

            Order order = _mapper.ToEntity(request, customer, games, totalAmount);

            await _context.Orders.AddAsync(order);

            return _mapper.ToResponse(order);
        }

        public async Task<Page<OrderResponse>> FindAllByCustomerId(long customerId, Pageable pageable)
        {
            return await _context.Orders.ToPageAsync(pageable, order => _mapper.ToResponse(order),
                new List<Predicate<Order>> { o => o.Customer.Id == customerId });
        }
    }
}
