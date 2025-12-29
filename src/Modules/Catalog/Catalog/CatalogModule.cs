using FluentValidation;
using Microsoft.AspNetCore.Builder; // this is due to  Fluent validation package
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.Interceptors;

namespace Catalog;
public static class CatalogModule
{
    // Dependency Injection 
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Domain - Add Services to the Container 

        // Presentation - Api Endpoints Services
        // Add Carter

        // Application   - Use Case Services
        #region Old Reg for Mediator
        //services.AddMediatR(config =>
        //{
        // any class will implements one of the 4 below will be registered in DI By the first lie code
        // IRequestHandler-INotificationHandler-IPipelineBehavior-IRequestPreProcessor-IRequestPostProcessor
        //config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

        //    config.AddOpenBehavior(typeof(ValidationBehavior<,>));   // adding Validation behavior Pipeline
        //    config.AddOpenBehavior(typeof(LoggingBehavior<,>));     // adding logging behavior pipeline
        //});
        //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); 
        #endregion


        // Data - Infrastructure Services
        var connectionString = configuration.GetConnectionString("Database");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventInterceptor>();

        services.AddDbContext<CatalogDbContext>((sp,options) =>
        {
            options.UseNpgsql(connectionString);
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>()); // because DispatchDomainEventInterceptor needs mediator in his ctor
              // options.AddInterceptors(new AuditableEntityInterceptor());
             // options.AddInterceptors(new DispatchDomainEventInterceptor());
            // the first one is build by th DI and the DI will be responsible for inject the required services in the CTOR
           // The second one you are the responsible for creating the object you cannot be able to inject services in the CTOR 
        });
                               

        services.AddScoped<IDataSeeder, CatalogDataSeeder>();

        return services;
    }
    // to use IApplicationBuilder we have to install http.abstraction package but it's deprecated 
    // so we will use the Fluent validation package

    public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
    {
        //HTTP Request Pipeline Configurations 

        // presentation - Api Endpoints services
        // Application  - Use Case services
        // Data - Infrastructure Services


        app.UseMigration<CatalogDbContext>();
        return app;
    }
 

}
