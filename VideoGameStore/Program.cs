using Microsoft.EntityFrameworkCore;
using VideoGameStore.Context;
using Microsoft.AspNetCore.Identity;
using VideoGameStore.Entities;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("LocalConnection")));

        builder.Services.AddIdentity<AspNetUser, IdentityRole<long>>(options =>
            {
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<long>>()
            .AddEntityFrameworkStores<AppDbContext>();

        builder.Services.AddAuthentication();

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("SellerOnly", policy =>
                policy.RequireRole("Seller"));

            options.AddPolicy("CustomerOnly", policy =>
                policy.RequireRole("Customer"));
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();

            var roles = new[] { "Seller", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<long> { Name = role });
                }
            }
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}