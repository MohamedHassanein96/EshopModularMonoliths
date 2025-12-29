namespace Basket.Basket.Features.CreateBasket;
public record CreateBasketCommand(ShoppingCartDto ShoppingCart)
    :ICommand<CreateBasketResult>;

public record CreateBasketResult(Guid Id);

public class CreateBasketCommandValidator : AbstractValidator<CreateBasketCommand>
{
    public CreateBasketCommandValidator()
    {
        RuleFor(x => x.ShoppingCart.UserName).NotEmpty().WithMessage("UserName is Required");
    }
}


internal class CreateBasketHandler (IBasketRepository repository,ISender sender)
    : ICommandHandler<CreateBasketCommand, CreateBasketResult>
{
    public async Task<CreateBasketResult> Handle(CreateBasketCommand command, CancellationToken cancellationToken)
    {
        // Create Basket entity from command obj
        // Save to DB
        // retutn result

        /* var shoppingCart = CreateNewBaskt(command.ShoppingCart); *///Review
                                                                      //dbContext.ShoppingCarts.Add(shoppingCart);
                                                                      //await dbContext.SaveChangesAsync(cancellationToken);

        #region My Implementation to get productName and Price 
        //var shopCart = ShoppingCart.Create(
        //    Guid.NewGuid(),
        //    command.ShoppingCart.UserName);

        //var listOfProductsIDS = command.ShoppingCart.Items.Select(x => x.ProductId).ToList();

        //while (listOfProductsIDS.Any())
        //{
        //    var listOfQantity = command.ShoppingCart.Items.Select(x => x.Quantity).ToList();
        //    var listOfColors = command.ShoppingCart.Items.Select(x => x.Color).ToList();

        //    var query = new GetProductByIdQuery(listOfProductsIDS.FirstOrDefault());

        //    var result = await sender.Send(query);

        //    shopCart.AddItem(listOfProductsIDS.FirstOrDefault(), listOfQantity.FirstOrDefault(), listOfColors.FirstOrDefault(), result.Product.Price, result.Product.Name);
        //    listOfProductsIDS.Remove(listOfProductsIDS.FirstOrDefault());
        //    listOfQantity.Remove(listOfQantity.FirstOrDefault());
        //    listOfColors.Remove(listOfColors.FirstOrDefault());
        //}


        //await repository.CreateBasket(shopCart, cancellationToken);
        //await repository.SaveChangesAsync(cancellationToken: cancellationToken);

        //return new CreateBasketResult(shopCart.Id);


        #endregion





        var shoppingCart = CreateNewBaskt(command.ShoppingCart);
        await repository.CreateBasket(shoppingCart, cancellationToken);

        return new CreateBasketResult(shoppingCart.Id);
    }

    private ShoppingCart CreateNewBaskt(ShoppingCartDto shoppingCartDto)
    {
        // Create new Basket
        var newBasket = ShoppingCart.Create(
            Guid.NewGuid(),
            shoppingCartDto.UserName            
            );


        shoppingCartDto.Items.ForEach(item =>
        {
            newBasket.AddItem(
                item.ProductId,
                item.Quantity,
                item.Color,
                item.Price,
                item.ProductName);
        });
        return newBasket;

    }
}
