using Keycloak.AuthServices.Authentication;

var builder = WebApplication.CreateBuilder(args);




builder.Host.UseSerilog((context, config) =>
config.ReadFrom.Configuration(context.Configuration));



// register services to the container


//centralize registeration
//common services: carter, mediator, fluentvalidation

#region Old Single Registeration for ICarter 
//Review
//builder.Services.AddCarter(configurator: config =>
//{
//    var catalogModules = typeof(CatalogModule).Assembly.GetTypes()
//    .Where(t => t.IsAssignableTo(typeof(ICarterModule))).ToArray();

//    config.WithModules(catalogModules);

//}); 
#endregion

var catalogAssembly = typeof(CatalogModule).Assembly;
var basketAssembly = typeof(BasketModule).Assembly;
var orderingAssembly = typeof(OrderingModule).Assembly;

builder.Services
    .AddCarterWithAssemblies(catalogAssembly, basketAssembly,orderingAssembly);
 

builder.Services
    .AddMediatoRWithAssemblies(basketAssembly, catalogAssembly,orderingAssembly);


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services
    .AddMassTransitWithAssemblies(builder.Configuration,catalogAssembly, basketAssembly,orderingAssembly);

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization();


#region Transformed into one single line with Extension Method
//builder.Services.AddMediatR(config =>
//{
//    config.RegisterServicesFromAssemblies(catalogAssembly, basketAssembly);
//    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
//    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
//});
//builder.Services.AddValidatorsFromAssemblies([catalogAssembly, basketAssembly]); 
#endregion



//module services: catalog, basket, ordering
builder.Services
    .AddCatalogModule(builder.Configuration)
    .AddBasketModule(builder.Configuration)
    .AddOrderingModule(builder.Configuration);


builder.Services.AddExceptionHandler<CustomExceptionHandler>();


var app = builder.Build();


// Configure the HTTP request pipeline

app.MapCarter();
app.UseSerilogRequestLogging(); // improving observability and traceability
app.UseExceptionHandler(options => { });
app.UseAuthentication();
app.UseAuthorization();



app.UseCatalogModule()
    .UseBasketModule()
    .UseOrderingModule();


app.Run();
