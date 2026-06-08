using Ecommerce.Catalog.Application.Products.DeactivateProduct;

namespace Ecommerce.Catalog.UnitTests.Products;

public sealed class DeactivateProductCommandValidatorTests
{
    private readonly DeactivateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_WithProductId_Succeeds()
    {
        var result = _validator.Validate(new DeactivateProductCommand(Guid.NewGuid()));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyProductId_Fails()
    {
        var result = _validator.Validate(new DeactivateProductCommand(Guid.Empty));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(DeactivateProductCommand.ProductId));
    }
}
