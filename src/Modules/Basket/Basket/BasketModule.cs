using Basket.Data.Processors;
using Basket.Data.Repository;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using Shared.Data.Interceptors;

namespace Basket;

public static class BasketModule
{
    public static IServiceCollection AddBasketModule(this IServiceCollection services , IConfiguration configuration )
    {
        // Domain - Add Services to the Container 
        services.AddScoped<IBasketRepository, BasketRepository>();

        // Presentation  -Api Endpoints Services
        #region AddCarter
        //services.AddCarter(configurator: config =>
        //{
        //    var basketModules = typeof(BasketModule).Assembly.GetTypes()
        //    .Where(t => t.IsAssignableTo(typeof(ICarterModule))).ToArray();

        //    config.WithModules(basketModules);
        //}); 
        #endregion

        // Application   -Use Case Services
        #region Old Registeration for Mediator
        //services.AddMediatR(config =>
        //{
        //    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

        //    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        //    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        //});
        //    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); 
        #endregion


        #region Dcorator Registeration Without Scrutor

        // this approach is les maintanable  so we will user scrutor lib
        // scrutor simplifies the process of registerating decorators in the dependency injection container
        //services.AddScoped<IBasketRepository>(provider =>
        //{
        //    var basketRepository = provider.GetRequiredService<IBasketRepository>();
        //    return new CachedBasketRepository(basketRepository, provider.GetRequiredService<IDistributedCache>());
        //});

        #endregion

        services.Decorate<IBasketRepository, CachedBasketRepository>();


        // Data / Infrastructure - DbContext
        var connectionString = configuration.GetConnectionString("Database");


        // Data / Infrastructure - Interceptor Services
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventInterceptor>();


        // Data / Infrastructure - DbContext
        services.AddDbContext<BasketDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });

        // Data / Infrastructure - Background Service
        services.AddHostedService<OutboxProcessor>();

        return services;
    }





    public static IApplicationBuilder UseBasketModule(this IApplicationBuilder app)
    {

        app.UseMigration<BasketDbContext>();
        return app;
    }
}
