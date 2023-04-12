using API.Entities.OrderAggregate;

namespace API.Dtos;

public class CreateOrderDto
{
    public bool SaveAddress { get; set; }
    public ShippingAddress ShippingAddress { get; set; }
}