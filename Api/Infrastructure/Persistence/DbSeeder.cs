using Microsoft.EntityFrameworkCore;
using Api.Infrastructure;
using Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EventHorizon.DataAccess.Persistence
{
    public class DbSeeder : ISeeder
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public DbSeeder(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task Seed()
        {
            // Apply pending migrations
            if (_dbContext.Database.GetPendingMigrations().Any())
                await _dbContext.Database.MigrateAsync();

            // Create test user if it doesn't exist
            var testEmail = "test@example.com";
            var existingUser = await _userManager.FindByEmailAsync(testEmail);
            
            if (existingUser == null)
            {
                var testUser = new User
                {
                    UserName = testEmail,
                    Email = testEmail,
                    Name = "Test User",
                    Handle = "testuser",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Create user with password that meets Identity requirements
                var result = await _userManager.CreateAsync(testUser, "Test123!");
                
                if (result.Succeeded)
                {
                    Console.WriteLine($"Test user created: {testEmail} with password: Test123!");
                }
                else
                {
                    Console.WriteLine($"Failed to create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
