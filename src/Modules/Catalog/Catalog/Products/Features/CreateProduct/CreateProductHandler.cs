namespace Catalog.Products.Features.CreateProduct;



// Processing Layer of PEPR

// Command : Part of Processing Layer of PEPR => represents the request after being transformed from Dto to Command
public record CreateProductCommand(ProductDto Product)
    : ICommand<CreateProductResult>;
//:ICommand<CreateProductResult>;

// Result : Part of Processing Layer of PEPR => represents the response that will return to endpoint after the execution of logic
public record CreateProductResult(Guid Id);

//Validation : Part of Processing Layer of PEPR
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Product.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Product.Category).NotEmpty().WithMessage("Category is required");
        RuleFor(x => x.Product.ImageFile).NotEmpty().WithMessage("ImageFile is required");
        RuleFor(x => x.Product.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}

// Business Logic : Part of Processing Layer of PEPR
internal class CreateProductHandler
    (CatalogDbContext dbContext)
    //IValidator<CreateProductCommand> validator,
    //ILogger<CreateProductHandler> logger)
    : ICommandHandler<CreateProductCommand, CreateProductResult>
//: IRequestHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        // Create Product entity from command object
        // Save to DB
        // Return Result


        #region replaced with MediatR Pipeline Behaviours  
        ////validation part 
        //var result = await validator.ValidateAsync(command, cancellationToken);
        //var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
        //if (errors.Any())
        //{
        //    throw new ValidationException(errors.FirstOrDefault());
        //}

        #endregion

        //logging part
        //logger.LogInformation("CreateProductCommandHandler.Handle called with {@Command}", command);


        // actual logic
        var product = CreateNewProduct(command.Product);
        dbContext.Products.Add(product);  // review 
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateProductResult(product.Id);  // REVIEW
    }

    private Product CreateNewProduct(ProductDto productDto)
    {
        var product = Product.Create(
            Guid.NewGuid(),
            productDto.Name,
            productDto.Category,
            productDto.Description,
            productDto.ImageFile,
            productDto.Price
         );
        return product;
    }

}
