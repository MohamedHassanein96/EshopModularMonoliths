using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shared.Behaviors;
using System.Reflection;

namespace Shared.Extensions;

public static class MediatoRExtensions
{
    public static IServiceCollection AddMediatoRWithAssemblies(this IServiceCollection services, params Assembly[] assemblies )
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assemblies);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        services.AddValidatorsFromAssemblies(assemblies);
        return services;
    }
}
