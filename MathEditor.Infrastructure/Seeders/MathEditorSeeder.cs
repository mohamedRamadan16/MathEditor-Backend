
using MathEditor.Domain.Constants;
using MathEditor.Domain.Entities;
using MathEditor.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MathEditor.Infrastructure.Seeders;

public class MathEditorSeeder(ApplicationDbContext _dbContext, UserManager<ApplicationUser> _userManager) : IMathEditorSeeder
{
    public async Task Seed()
    {
        if (_dbContext.Database.GetPendingMigrations().Any())
            await _dbContext.Database.MigrateAsync();

        if (await _dbContext.Database.CanConnectAsync())
        {
            // Seeding the roles
            if (!_dbContext.Roles.Any())
            {
                var roles = GetRoles();
                await _dbContext.Roles.AddRangeAsync(roles);
                await _dbContext.SaveChangesAsync();
            }

            if (!await _dbContext.Users.AnyAsync())
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@matheditor.com",
                    Handle = "admin",
                    Name = "Administrator",
                    Role = "admin",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin@123");
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to seed admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }

    private IEnumerable<IdentityRole> GetRoles()
    {
        List<IdentityRole> roles = [
                new(MathEditorRoles.User){
                        NormalizedName = MathEditorRoles.User.ToUpper()
                    },
                    new(MathEditorRoles.Admin){
                        NormalizedName = MathEditorRoles.Admin.ToUpper()
                    },
                    new(MathEditorRoles.Owner)
                    {
                        NormalizedName = MathEditorRoles.Owner.ToUpper()
                    }
            ];
        return roles;
    }
}
