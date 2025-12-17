using Microsoft.EntityFrameworkCore;
using VideoGameStore.Context;
using Microsoft.AspNetCore.Identity;
using VideoGameStore.Entities;
using VideoGameStore.Services;
using VideoGameStore.Mappers;
using Microsoft.Data.SqlClient;
using VideoGameStore.Exceptions;
using Microsoft.OpenApi.Models;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "VideoGameStore API",
                Version = "v1"
            });
        });

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("LocalConnection")
            ));

        builder.Services.AddIdentity<AspNetUser, IdentityRole<long>>(options =>
        {
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
            .AddRoles<IdentityRole<long>>()
            .AddEntityFrameworkStores<AppDbContext>();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("SellerOnly", policy =>
                policy.RequireRole("Seller"));

            options.AddPolicy("CustomerOnly", policy =>
                policy.RequireRole("Customer"));
        });

        builder.Services.AddScoped<SellerMapper>();
        builder.Services.AddScoped<GlobalExceptionFilter>();
        builder.Services.AddScoped<CustomerMapper>();
        builder.Services.AddScoped<CartMapper>();
        builder.Services.AddScoped<GameMapper>();
        builder.Services.AddScoped<GenreMapper>();
        builder.Services.AddScoped<KeyMapper>();
        builder.Services.AddScoped<OrderMapper>();
        builder.Services.AddScoped<CartItemMapper>();
        builder.Services.AddScoped<OrderItemMapper>();

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IKeyService, KeyService>();
        builder.Services.AddScoped<IGenreService, GenreService>();
        builder.Services.AddScoped<IGameService, GameService>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var isTestEnvironment = app.Environment.IsEnvironment("Testing");

            try
            {
                if (!isTestEnvironment)
                {
                    await CreateDatabaseAsync(configuration, logger);
                }

                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.MigrateAsync();
                logger.LogInformation("Migrations applied");

                await CreateRolesAsync(scope.ServiceProvider, logger);

                if (!isTestEnvironment)
                {
                    await LoadInitDataAsync(db, logger);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during application initialization");
                throw;
            }
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VideoGameStore v1");
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        await app.RunAsync();
    }

    private static async Task CreateDatabaseAsync(IConfiguration configuration, ILogger<Program> logger)
    {
        try
        {
            var connString = configuration.GetConnectionString("LocalConnection");
            var builder = new SqlConnectionStringBuilder(connString)
            {
                InitialCatalog = "master"
            };

            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();

                cmd.CommandText = @"IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'VideoGameStoreDb')
                    BEGIN
                        CREATE DATABASE [VideoGameStoreDb];
                    END";
                await cmd.ExecuteNonQueryAsync();
                logger.LogInformation("Database created or already exists");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating database");
            throw;
        }
    }

    private static async Task CreateRolesAsync(IServiceProvider serviceProvider, ILogger<Program> logger)
    {
        try
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();
            var roles = new[] { "Seller", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<long> { Name = role });
                    logger.LogInformation($"Role '{role}' created");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating roles");
            throw;
        }
    }

    private static async Task LoadInitDataAsync(AppDbContext db, ILogger<Program> logger)
    {
        try
        {
            var initSqlPath = Path.Combine(AppContext.BaseDirectory, "Sql", "init-db.sql");
            if (File.Exists(initSqlPath))
            {
                var initSql = await File.ReadAllTextAsync(initSqlPath);
                if (!string.IsNullOrWhiteSpace(initSql))
                {
                    await db.Database.ExecuteSqlRawAsync(initSql);
                    logger.LogInformation("Init data loaded");
                }
            }
            else
            {
                logger.LogWarning($"Init SQL file not found: {initSqlPath}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading init data");
            throw;
        }
    }
}
