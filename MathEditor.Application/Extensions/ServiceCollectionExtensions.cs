using Microsoft.Extensions.DependencyInjection;

namespace MathEditor.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplication(this IServiceCollection services)
        {
            // Register MediatR with the application assembly containing all handlers
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

            // Add HttpContextAccessor for handlers needing HTTP context (e.g., authentication)
            services.AddHttpContextAccessor();

        }
    }
}
