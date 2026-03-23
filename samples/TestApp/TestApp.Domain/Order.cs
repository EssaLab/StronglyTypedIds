namespace TestApp.Domain;

public class Order
{
    public OrderId Id { get; init; }
    public CardId CardId { get; init; }
    public DateTime OrderDate { get; init; }
    public CustomerId CustomerId { get; init; }
}

