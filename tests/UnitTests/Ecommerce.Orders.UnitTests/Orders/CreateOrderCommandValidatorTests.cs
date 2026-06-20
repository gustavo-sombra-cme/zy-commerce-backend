using Ecommerce.Orders.Application.Orders.CreateOrder;

namespace Ecommerce.Orders.UnitTests.Orders;

public sealed class CreateOrderCommandValidatorTests
{
    [Fact]
    public void Validate_ShouldPass_ForValidCommand()
    {
        var validator = new CreateOrderCommandValidator();
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new[]
            {
                new CreateOrderLineCommand(Guid.NewGuid(), "SKU-1", "Product", 10m, 1)
            });

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenNoLinesAreProvided()
    {
        var validator = new CreateOrderCommandValidator();
        var command = new CreateOrderCommand(Guid.NewGuid(), Array.Empty<CreateOrderLineCommand>());

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_ForInvalidLineSnapshot()
    {
        var validator = new CreateOrderCommandValidator();
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new[]
            {
                new CreateOrderLineCommand(Guid.Empty, "", "", -1m, 0)
            });

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName.Contains(nameof(CreateOrderLineCommand.ProductId), StringComparison.Ordinal));
        Assert.Contains(result.Errors, error => error.PropertyName.Contains(nameof(CreateOrderLineCommand.ProductSku), StringComparison.Ordinal));
        Assert.Contains(result.Errors, error => error.PropertyName.Contains(nameof(CreateOrderLineCommand.ProductName), StringComparison.Ordinal));
        Assert.Contains(result.Errors, error => error.PropertyName.Contains(nameof(CreateOrderLineCommand.UnitPrice), StringComparison.Ordinal));
        Assert.Contains(result.Errors, error => error.PropertyName.Contains(nameof(CreateOrderLineCommand.Quantity), StringComparison.Ordinal));
    }
}
