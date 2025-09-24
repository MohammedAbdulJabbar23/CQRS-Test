using Microsoft.OpenApi.Models;
using TodoList.Application;
using TodoList.Infrastructure;
using TodoList.Infrastructure.Persistence;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "TodoList API",
                Version = "v1",
                Description = "A simple TodoList API with JWT authentication"
            });

            // Add JWT auth option in Swagger UI
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer {your token}'"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        // Add your custom infrastructure (DbContext, JWT config, etc.)
        builder.Services
        .AddInfrastructure(builder.Configuration)
        .AddApplication();

        var app = builder.Build();

        // Enable Swagger
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoList API v1");
            });
        }

        // Middlewares
        // app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        using (var scope = app.Services.CreateScope())
        {
            await SeedData.SeedDefaultDataAsync(scope.ServiceProvider);
        }
        app.MapControllers();

        app.Run();
    }
}
