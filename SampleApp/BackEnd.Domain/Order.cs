namespace BackEnd.Domain;

public class Order
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string? CustomerName { get; set; }
    public string? Note { get; set; }
    public OrderStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set;}
}

public enum OrderStatus { Pending, Confirmed, Shipped, Delivered, Cancelled }