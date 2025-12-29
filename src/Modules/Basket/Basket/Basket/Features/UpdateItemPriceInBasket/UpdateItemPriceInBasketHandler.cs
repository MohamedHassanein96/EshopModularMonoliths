namespace Basket.Basket.Features.UpdateItemPriceInBasket;

public record UpdateItemPriceInBasketCommand(Guid ProductId, decimal Price)
    :ICommand<UpdateItemPriceInBasketResult>;

public record UpdateItemPriceInBasketResult(bool IsSuccess);

public class UpdateItemPriceInBasketCommandValidator :AbstractValidator<UpdateItemPriceInBasketCommand>
{
    public UpdateItemPriceInBasketCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product is Rquired");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}
// This class is triggered from ProductPriceChangedIntegrationEventHandler so we can use BasketDbContext Directly without repository
// there is no need to cache repo
internal class UpdateItemPriceInBasketHandler (BasketDbContext dbContext)
    : ICommandHandler<UpdateItemPriceInBasketCommand, UpdateItemPriceInBasketResult>
{
    public async Task<UpdateItemPriceInBasketResult> Handle(UpdateItemPriceInBasketCommand command, CancellationToken cancellationToken)
    {
        // FIND SHOPPING CART WTH A GIVEN PRODUCT ID
        // ITERATE ITEMS AND UPDATE PRICE OF EVERY ITEM WITH INCOMING COMMAND-PRICE
        //SAVE TO DB
        // RETURN RESULT

        var itemsToUpdate = await dbContext.ShoppingCartItems
            .Where(x=>x.ProductId == command.ProductId)
            .ToListAsync(cancellationToken);

        if (!itemsToUpdate.Any())
        {
            return new UpdateItemPriceInBasketResult(false);
        }

        foreach (var item in itemsToUpdate)
        {
           item.UpdatePrice(command.Price);
        };

       await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateItemPriceInBasketResult(true);
    }
}
