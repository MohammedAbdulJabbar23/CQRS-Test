using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedDefaultDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (context.Database.GetPendingMigrations().Any())
        {
            await context.Database.MigrateAsync();
        }

        // Seed Roles
        var roles = new[] { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = "admin@rh.iq";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await userManager.CreateAsync(adminUser, "12345678");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Seed Regular Users
        var regularUsers = new[]
        {
            new { Email = "mohamemd@rh.iq", FirstName = "MOhammed", LastName = "Jasim", Password = "12345678" },
            new { Email = "mohammed2@rh.iq", FirstName = "Mohammed", LastName = "AbdulJabbar", Password = "12345678" }
        };

        foreach (var userData in regularUsers)
        {
            if (await userManager.FindByEmailAsync(userData.Email) == null)
            {
                var user = new User
                {
                    UserName = userData.Email,
                    Email = userData.Email,
                    EmailConfirmed = true,
                    FirstName = userData.FirstName,
                    LastName = userData.LastName,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await userManager.CreateAsync(user, userData.Password);
                await userManager.AddToRoleAsync(user, "User");
            }
        }

        await context.SaveChangesAsync();
    }
}