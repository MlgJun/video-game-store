using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VideoGameStore.Entities;

namespace VideoGameStore.Context
{
    public class AppDbContext : IdentityDbContext<AspNetUser, IdentityRole<long>, long>
    {
        public DbSet<Seller> Sellers { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //USER
            modelBuilder.Entity<User>().UseTpcMappingStrategy();

            modelBuilder.Entity<Customer>().ToTable("Customers");

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Cart)
                .WithOne(c => c.Customer)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Seller>().ToTable("Sellers");

            //GAME
            modelBuilder.Entity<Game>()
               .Property(g => g.Price)
               .HasColumnType("decimal(10,2)");

            //ORDER
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(12,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(10,2)");

            //CART
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(c => c.Cart)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.CustomerId)
                .IsUnique();

            modelBuilder.Entity<CartItem>().HasIndex(ci => ci.CartId);
        }
    }
}
