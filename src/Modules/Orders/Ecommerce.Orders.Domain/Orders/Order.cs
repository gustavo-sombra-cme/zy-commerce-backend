namespace Ecommerce.Orders.Domain.Orders;

public sealed class Order
{
    private readonly List<OrderLine> _lines = new();

    private Order()
    {
    }

    private Order(OrderId id, BuyerId buyerId, IEnumerable<OrderLine> lines, DateTimeOffset createdAt)
    {
        if (createdAt == default)
        {
            throw new ArgumentException("Created timestamp is required.", nameof(createdAt));
        }

        var orderLines = lines.ToArray();

        if (orderLines.Length == 0)
        {
            throw new ArgumentException("Order must contain at least one line.", nameof(lines));
        }

        Id = id;
        BuyerId = buyerId;
        Status = OrderStatus.Created;
        CreatedAt = createdAt;
        _lines.AddRange(orderLines);
        TotalAmount = _lines.Sum(line => line.LineTotal);
    }

    public OrderId Id { get; private set; }

    public BuyerId BuyerId { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public decimal TotalAmount { get; private set; }

    public IReadOnlyCollection<OrderLine> Lines => _lines;

    public static Order Create(BuyerId buyerId, IEnumerable<OrderLine> lines, DateTimeOffset createdAt) =>
        new(OrderId.New(), buyerId, lines, createdAt);
}
