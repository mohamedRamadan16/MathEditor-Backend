using MathEditor.Domain.Entities;
using MathEditor.Domain.IRepositories;
using MathEditor.Infrastructure.Persistence;
using MathEditor.Infrastructure.Repositories;
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

        // Register Repositories
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IRevisionRepository, RevisionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddMemoryCache();

        services.AddIdentityApiEndpoints<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
    }
}
