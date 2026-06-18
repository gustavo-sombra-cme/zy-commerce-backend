using Ecommerce.Catalog.Application.Products.ReactivateProduct;

namespace Ecommerce.Catalog.UnitTests.Products;

public sealed class ReactivateProductCommandValidatorTests
{
    private readonly ReactivateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_WithProductId_Succeeds()
    {
        var result = _validator.Validate(new ReactivateProductCommand(Guid.NewGuid()));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyProductId_Fails()
    {
        var result = _validator.Validate(new ReactivateProductCommand(Guid.Empty));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ReactivateProductCommand.ProductId));
    }
}
