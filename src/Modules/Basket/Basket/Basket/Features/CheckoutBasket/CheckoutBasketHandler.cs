using MassTransit;
using Shared.Messaging.Events;
using System.Text.Json;

namespace Basket.Basket.Features.CheckoutBasket;

public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckout)
    : ICommand<CheckoutBasketResult>;

public record CheckoutBasketResult(bool IsSuccess);

public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(x => x.BasketCheckout).NotNull().WithMessage("BasketCheckoutDto can't be null");
        RuleFor(x => x.BasketCheckout.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

internal class CheckoutBasketHandler(BasketDbContext dbContext)
    : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    {
        // get existing basket with total price
        // Set totalprice on basketcheckout event message
        // send basket checkout event to rabbitmq using masstransit
        // delete the basket
        //dual write problem

        await using var transction =
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // get existing basket with total price
            var basket = await dbContext.ShoppingCarts
                .Include(s => s.Items)
                .SingleOrDefaultAsync(s => s.UserName == command.BasketCheckout.UserName, cancellationToken);


            if (basket is null)
            {
                throw new BasketNotFoundException(command.BasketCheckout.UserName);
            }

            // Set totalprice on basketcheckout event message
            var eventMessage = command.BasketCheckout.Adapt<BasketCheckoutIntegrationEvent>();
            eventMessage.TotalPrice = basket.TotalPrice;


            // write the message to outbox table
            var outboxMssage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = typeof(BasketCheckoutIntegrationEvent).AssemblyQualifiedName!, // review
                Content = JsonSerializer.Serialize(eventMessage),
                OccuredOn = DateTime.UtcNow
            };

            dbContext.OutboxMessages.Add(outboxMssage);

            //delete the basket 
            dbContext.ShoppingCarts.Remove(basket);

            await dbContext.SaveChangesAsync(cancellationToken);

            await transction.CommitAsync(cancellationToken);

            return new CheckoutBasketResult(true);

        }
        catch (Exception)
        {
            await transction.RollbackAsync(cancellationToken);
            return new CheckoutBasketResult(false);
        }


        #region without outbox pattern
        //var basket =await repository.GetBasket(command.BasketCheckout.UserName, true, cancellationToken);

        //var eventMessage = command.BasketCheckout.Adapt<BasketCheckoutIntegrationEvent>(); 
        //eventMessage.TotalPrice = basket.TotalPrice; // review 

        //await bus.Send(eventMessage);

        //await repository.DeleteBasket(command.BasketCheckout.UserName, cancellationToken);

        //return new CheckoutBasketResult(true); 
        #endregion

    }
}
