using Microsoft.EntityFrameworkCore;
using Api.Infrastructure;

namespace EventHorizon.DataAccess.Persistence
{
    public class DbSeeder : ISeeder
    {
        private readonly ApplicationDbContext _dbContext;
        public DbSeeder(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Seed()
        {
          // Apply pending migrations
          if (_dbContext.Database.GetPendingMigrations().Any())
              await _dbContext.Database.MigrateAsync();
        }
    }
}
