
using MathEditor.Domain.Constants;
using MathEditor.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MathEditor.Infrastructure.Seeders;

public class MathEditorSeeder(ApplicationDbContext _dbContext) : IMathEditorSeeder
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
