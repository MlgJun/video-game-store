using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
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
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Key> Keys { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //USER
            modelBuilder.Entity<User>().UseTpcMappingStrategy();

            modelBuilder.Entity<Customer>()
                .ToTable("Customers")
                .HasOne(c => c.Cart)
                .WithOne(c => c.Customer)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Seller>()
                .ToTable("Sellers")
                .HasMany(s => s.Games)
                .WithOne(g => g.Seller)
                .OnDelete(DeleteBehavior.Cascade);

            //GAME
            modelBuilder.Entity<Game>()
               .ToTable("Games")
               .Property(g => g.Price)
               .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Game>()
                .HasMany(g => g.Keys)
                .WithOne(k => k.Game)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Seller)
                .WithMany(s => s.Games);

            //Keys
            modelBuilder.Entity<Key>()
                .ToTable("Keys")
                .HasOne(k => k.Game)
                .WithMany(g => g.Keys);


            //GENRE
            modelBuilder.Entity<Genre>()
                .ToTable("Genres")
                .HasMany(g => g.Games)
                .WithMany(g => g.Genres)
                .UsingEntity(e => e.ToTable("GameGenres"));

            //ORDER
            modelBuilder.Entity<Order>()
                .ToTable("Orders")
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(12,2)");

            modelBuilder.Entity<OrderItem>()
                .ToTable("OrderItems")
                .Property(oi => oi.Price)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<OrderItem>()
                 .Property(o => o.Keys)
                 .HasColumnType("nvarchar(max)");

            //CART
            modelBuilder.Entity<Cart>()
                .ToTable("Carts")
                .HasMany(c => c.CartItems)
                .WithOne(c => c.Cart)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.CustomerId)
                .IsUnique();

            modelBuilder.Entity<CartItem>()
                .ToTable("CartItems");

            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => ci.CartId);

            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => ci.GameId);
        }
    }
}
