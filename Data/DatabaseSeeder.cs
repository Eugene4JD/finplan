using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using FinancialPlanningAPI.Data;
using FinancialPlanningAPI.Models;

public static class DatabaseSeeder
{
    public static void Seed(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Ensure the database is created
            context.Database.EnsureCreated();

            // Check if the admin role already exists
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var adminRole = new Role { Name = "Admin" };
                context.Roles.Add(adminRole);
                context.SaveChanges();

                // Create an admin user
                var adminUser = new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("adminpassword")
                };
                context.Users.Add(adminUser);
                context.SaveChanges();

                // Assign admin role to admin user
                var userRole = new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                };
                context.UserRoles.Add(userRole);
                context.SaveChanges();
            }
        }
    }
}