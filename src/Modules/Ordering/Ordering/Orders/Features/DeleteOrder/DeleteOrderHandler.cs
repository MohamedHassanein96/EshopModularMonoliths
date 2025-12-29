
using Ordering.Orders.Exceptions;

namespace Ordering.Orders.Features.DeleteOrder;


public record DeleteOrderCommand(Guid OrderId)
    :ICommand<DeleteOrderResult>;

public record DeleteOrderResult(bool IsSuccess);


public class DeleteOrderCommandValidator :AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("OrderName is Required");
    }
}

internal class DeleteOrderHandler(OrderingDbContext dbContext)
    : ICommandHandler<DeleteOrderCommand, DeleteOrderResult>
{
    public async Task<DeleteOrderResult> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
    {
       var order = await dbContext.Orders.FindAsync
            (command.OrderId, cancellationToken);

        if (order is null)
        {
            throw new OrderNotFoundException(command.OrderId);
        }

        dbContext.Orders.Remove(order); // Review 
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteOrderResult(true);
    }
}
