namespace Ordering.Orders.Dtos;

//public  record OrderDto(Guid Id, Guid CustomerId, string OrderName, Address ShippingAddress, Address BillingAddress, Payment Payment,List<OrderItem> Items);
public  record OrderDto(Guid Id,
    Guid CustomerId,
    string OrderName,
    AddressDto ShippingAddress,
    AddressDto BillingAddress,
    PaymentDto Payment,
    List<OrderItemDto> Items);


