using Carter;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Shared.Extensions;

public static class CarterExtensions
{
    public static IServiceCollection AddCarterWithAssemblies
        (this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddCarter(configurator: config =>
        {
            foreach (var assembley in assemblies)
            {
                var modules = assembley.GetTypes() // GetTypes will return all classes
               .Where(t => t.IsAssignableTo(typeof(ICarterModule))).ToArray();

                config.WithModules(modules);
            }

        });
        return services;
    }
}


