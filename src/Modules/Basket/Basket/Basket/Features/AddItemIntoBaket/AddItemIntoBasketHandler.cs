using Catalog.Contracts.Products.Features.GetProductById;

namespace Basket.Basket.Features.AddItemIntoBaket;


public record AddItemIntoBasketCommand(string UserName ,ShoppingCartItemDto ShoppingCartItem)
    : ICommand<AddItemIntoBasketResult>;

public record AddItemIntoBasketResult(Guid Id);


public class AddItemIntoBasketCommandValidator : AbstractValidator<AddItemIntoBasketCommand>
{
    public AddItemIntoBasketCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
        RuleFor(x => x.ShoppingCartItem.ProductId).NotEmpty().WithMessage("ProductId is required");
        RuleFor(x => x.ShoppingCartItem.Quantity).GreaterThan(0).WithMessage("UserName must be GreaterThan 0");
    }
}

internal class AddItemIntoBasketHandler
    (IBasketRepository repository, ISender sender)
    : ICommandHandler<AddItemIntoBasketCommand, AddItemIntoBasketResult>
{
    public async Task<AddItemIntoBasketResult> Handle(AddItemIntoBasketCommand command, CancellationToken cancellationToken)
    {


        //var shoppingCart = await dbContext.ShoppingCarts
        //    .Include(x => x.Items)
        //    .SingleOrDefaultAsync(x => x.UserName == command.UserName, cancellationToken);

        //if (shoppingCart is null)
        //{
        //    throw new BasketNotFoundException(command.UserName);
        //}



       

        var shoppingCart = await repository.GetBasket(command.UserName, false, cancellationToken);



        // TODO : before AddItem into SC,we should call catalog module GetProductById Method
        // get latest product info and set price and productName when adding item into basket

        // sync communication in-memory process
        var query = new GetProductByIdQuery(command.ShoppingCartItem.ProductId);

        var result = await sender.Send(query);
          

        shoppingCart.AddItem(
            command.ShoppingCartItem.ProductId,
            command.ShoppingCartItem.Quantity,
            command.ShoppingCartItem.Color,
            result.Product.Price,
            result.Product.Name
            //command.ShoppingCartItem.Price,
            //command.ShoppingCartItem.ProductName
            );

     
       await repository.SaveChangesAsync(command.UserName,cancellationToken);

        return new AddItemIntoBasketResult(shoppingCart.Id);
    }
}
