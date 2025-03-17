
using MathEditor.API.Extensions;
using MathEditor.Infrastructure.Extensions;
using MathEditor.Infrastructure.Seeders;
using Serilog;

namespace MathEditor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            builder.AddPresentation();
            builder.Services.AddInfrastructure(builder.Configuration); // for sql server connection
            //builder.Services.AddApplication();

            var app = builder.Build();

            /// seeding when start the application
            var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<IMathEditorSeeder>();
            seeder.Seed();


            app.UseSerilogRequestLogging();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
