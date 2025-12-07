namespace VideoGameStore.Tests
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using VideoGameStore.Context;

    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly string _connectionString;

        public IntegrationTestWebAppFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.Single(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(_connectionString));

                using var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            });
        }
    }
}
