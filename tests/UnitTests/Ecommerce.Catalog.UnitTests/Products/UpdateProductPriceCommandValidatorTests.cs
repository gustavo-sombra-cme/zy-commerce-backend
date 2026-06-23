using Ecommerce.Catalog.Application.Products.UpdateProductPrice;

namespace Ecommerce.Catalog.UnitTests.Products;

public sealed class UpdateProductPriceCommandValidatorTests
{
    [Fact]
    public void Validate_WithValidCommand_IsValid()
    {
        var validator = new UpdateProductPriceCommandValidator();

        var result = validator.Validate(new UpdateProductPriceCommand(Guid.NewGuid(), 19.99m));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyProductId_IsInvalid()
    {
        var validator = new UpdateProductPriceCommandValidator();

        var result = validator.Validate(new UpdateProductPriceCommand(Guid.Empty, 19.99m));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithNegativePrice_IsInvalid()
    {
        var validator = new UpdateProductPriceCommandValidator();

        var result = validator.Validate(new UpdateProductPriceCommand(Guid.NewGuid(), -0.01m));

        Assert.False(result.IsValid);
    }
}
