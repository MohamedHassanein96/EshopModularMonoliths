using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Orders.Features.CreateOrder;
using Shared.Messaging.Events;

namespace Ordering.Orders.EventHandlers;

public class BasketCheckoutIntegrationEventHanlder
    (ISender sender, ILogger<BasketCheckoutIntegrationEventHanlder> logger)
    : IConsumer<BasketCheckoutIntegrationEvent>
{
    public async Task Consume(ConsumeContext<BasketCheckoutIntegrationEvent> context)
    {
        // create new order and start order fullfillment process
        var createOrderCommand = MapToCreateOrderCommand(context.Message);
        await sender.Send(createOrderCommand);
    }

    private CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutIntegrationEvent message)
    {
        // create full order with incoming evnt data
        var addressDto = new AddressDto(message.FirstName, message.LastName, message.EmailAddress, message.AddressLine, message.Country, message.State, message.ZipCode);
        var paymentDto = new PaymentDto(message.CardName, message.CardNumber, message.Expiration, message.Cvv, message.PaymentMethod);
        var orderId = Guid.NewGuid();

        var orderDto = new OrderDto(
            orderId,
            message.CustomerId,
            message.UserName,
            addressDto,
            addressDto,
            paymentDto,
            [
                 new OrderItemDto(orderId, new Guid("6139715d-9f89-491c-8c80-19f2ae39281d"),2,500),
                 new OrderItemDto(orderId, new Guid("7139715d-9f89-491c-8c80-19f2ae39281d"),1,400),
            ]);



        return new CreateOrderCommand(orderDto);
    }
}
