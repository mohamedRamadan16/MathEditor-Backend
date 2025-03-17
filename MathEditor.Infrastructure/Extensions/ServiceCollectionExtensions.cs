using MathEditor.Domain.Entities;
using MathEditor.Infrastructure.Persistence;
using MathEditor.Infrastructure.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MathEditor.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MathEditorDB");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString)
                   .EnableSensitiveDataLogging()
        );

        // services
        services.AddScoped<IMathEditorSeeder, MathEditorSeeder>();

        services.AddIdentityApiEndpoints<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
    }
}
