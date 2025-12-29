namespace Catalog.Products.Features.CreateProduct;

//PEPR Pattern :Presentation, Endpoints, Processing(commands + handlers), Repository.
//Data Structure for Creating a Product and the Response 

// Presentation Layer : DTOs=> are Part of Presentation layer, Define structure of data which coming from clients and returning to them (data itself).
public record CreateProductRequest(ProductDto Product);
public record CreateProductResponse(Guid Id);

// ICarterModule this interface allow us to define routes in a structured way

// Endpoint : part of Presentation Layer that actually handles HTTP Request/Response (execution part).
public class CreateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/products", async (CreateProductRequest request, ISender sender) =>
        {
            // mediator accept only command or query
            var command = request.Adapt<CreateProductCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateProductResponse>();

            // Results is a helper factory that creates response objects (IResult) for Minimal APIs.
            // It does NOT generate the HTTP response itself — ASP.NET Core handles serialization
            // and writes the final HTTP response to the client.
            return Results.Created($"/products/{response.Id}", response);

        })
            .WithName("CreateProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Product")
            .WithDescription("Create Product");
    }
}
