namespace VideoGameStore.Tests
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using VideoGameStore.Context;
    using VideoGameStore.Mappers;
    using VideoGameStore.Services;

    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly string _connectionString;

        public IntegrationTestWebAppFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.FirstOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(_connectionString));

                services.AddScoped<SellerMapper>();
                services.AddScoped<CustomerMapper>();
                services.AddScoped<CartMapper>();
                services.AddScoped<GameMapper>();
                services.AddScoped<GenreMapper>();
                services.AddScoped<KeyMapper>();
                services.AddScoped<OrderMapper>();
                services.AddScoped<CartItemMapper>();
                services.AddScoped<OrderItemMapper>();

                services.AddScoped<ICartService, CartService>();
                services.AddScoped<IUserService, UserService>();
                
                services.AddScoped<IOrderService, OrderService>();
                services.AddScoped<IKeyService, KeyService>();
                services.AddScoped<IGenreService, GenreService>();
                services.AddScoped<IGameService, GameService>();

                using var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            });
        }
    }
}
