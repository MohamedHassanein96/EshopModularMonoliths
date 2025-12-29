namespace Basket.Basket.Features.DeleteBasket;


public record DeleteBasketCommand(string UserName)
    : ICommand<DeleteBasketResult>;

public record DeleteBasketResult(bool IsSuccess);

public class DeleteBasketCommandValidator : AbstractValidator<DeleteBasketCommand>
{
    //Why
    //public DeleteBasketCommandValidator()
    //{
    //    RuleFor(x => x.Id).NotEmpty().WithMessage("Basket Id is required");
    //}
}



internal class DeleteBasketHandler (IBasketRepository repository)
    : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
    public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
    {
        //var basket = await dbContext.ShoppingCarts
        //    .SingleOrDefaultAsync(x=>x.UserName == command.UserName, cancellationToken);

        //if (basket is null)
        //{
        //    throw new BasketNotFoundException(command.UserName);
        //}

        //dbContext.ShoppingCarts.Remove(basket);
        //await dbContext.SaveChangesAsync(cancellationToken);

        await repository.DeleteBasket(command.UserName, cancellationToken);

        return new DeleteBasketResult(true);
    }
}
