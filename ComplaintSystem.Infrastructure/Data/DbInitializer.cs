using Microsoft.AspNetCore.Identity;
using ComplaintSystem.Domain.Entities;
using ComplaintSystem.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace ComplaintSystem.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Seed Roles
        string[] roles = { "Admin", "Staff", "Supervisor", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed Admin User
        var adminEmail = "admin@complaintsystem.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin@123");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Seed Categories if empty
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { Name = "Technical Issue", Description = "Issues related to hardware or software" },
                new Category { Name = "Billing", Description = "Billing and payment concerns" },
                new Category { Name = "Customer Service", Description = "Feedback on staff or services" },
                new Category { Name = "Infrastructure", Description = "Physical building or facility issues" }
            );
        }

        // Seed Departments if empty
        if (!context.Departments.Any())
        {
            context.Departments.AddRange(
                new Department { Name = "IT Department", Email = "it@complaintsystem.com" },
                new Department { Name = "Finance", Email = "finance@complaintsystem.com" },
                new Department { Name = "General Administration", Email = "admin@complaintsystem.com" }
            );
        }

        await context.SaveChangesAsync();
    }
}
