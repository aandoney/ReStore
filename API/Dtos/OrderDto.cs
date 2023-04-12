using API.Entities.OrderAggregate;

namespace API.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public ShippingAddress ShippingAddress { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public List<OrderItemDto> OrderItems { get; set; }
    public long Subtotal { get; set; }
    public long DeliveryFee { get; set; }
    public string orderStatus { get; set; }
    public long Total { get; set; }
}