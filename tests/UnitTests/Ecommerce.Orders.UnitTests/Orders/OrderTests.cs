using Ecommerce.Orders.Domain.Orders;

namespace Ecommerce.Orders.UnitTests.Orders;

public sealed class OrderTests
{
    [Fact]
    public void Create_ShouldCaptureProductSnapshots_AndCalculateTotal()
    {
        var buyerId = BuyerId.From(Guid.NewGuid());
        var createdAt = DateTimeOffset.UtcNow;
        var firstProductId = Guid.NewGuid();
        var secondProductId = Guid.NewGuid();
        var lines = new[]
        {
            OrderLine.Create(firstProductId, " SKU-1 ", " First Product ", 10.005m, 2),
            OrderLine.Create(secondProductId, "SKU-2", "Second Product", 5m, 3)
        };

        var order = Order.Create(buyerId, lines, createdAt);

        Assert.NotEqual(Guid.Empty, order.Id.Value);
        Assert.Equal(buyerId, order.BuyerId);
        Assert.Equal(OrderStatus.Created, order.Status);
        Assert.Equal(createdAt, order.CreatedAt);
        Assert.Equal(35.02m, order.TotalAmount);
        Assert.Equal(2, order.Lines.Count);

        var firstLine = order.Lines.First();
        Assert.Equal(firstProductId, firstLine.ProductId);
        Assert.Equal("SKU-1", firstLine.ProductSku);
        Assert.Equal("First Product", firstLine.ProductName);
        Assert.Equal(10.01m, firstLine.UnitPrice);
        Assert.Equal(2, firstLine.Quantity);
        Assert.Equal(20.02m, firstLine.LineTotal);
    }

    [Fact]
    public void Create_ShouldRejectOrdersWithoutLines()
    {
        var buyerId = BuyerId.From(Guid.NewGuid());

        var exception = Assert.Throws<ArgumentException>(() =>
            Order.Create(buyerId, Array.Empty<OrderLine>(), DateTimeOffset.UtcNow));

        Assert.Contains("at least one line", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void OrderLineCreate_ShouldRejectNonPositiveQuantity(int quantity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            OrderLine.Create(Guid.NewGuid(), "SKU-1", "Product", 10m, quantity));
    }

    [Fact]
    public void OrderLineCreate_ShouldRejectEmptyProductId()
    {
        Assert.Throws<ArgumentException>(() =>
            OrderLine.Create(Guid.Empty, "SKU-1", "Product", 10m, 1));
    }
}
