using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Application.Common.Behaviors;

namespace TodoList.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Add FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        //Automapper add
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        // Add MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        return services;
    }
}