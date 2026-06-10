using Ecommerce.Catalog.Application.Products.UpdateProductDetails;
using Ecommerce.Catalog.Domain.Products;

namespace Ecommerce.Catalog.UnitTests.Products;

public sealed class UpdateProductDetailsCommandValidatorTests
{
    private readonly UpdateProductDetailsCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_Succeeds()
    {
        var result = _validator.Validate(new UpdateProductDetailsCommand(
            Guid.NewGuid(),
            "Updated Product",
            "Updated description"));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyProductId_Fails()
    {
        var result = _validator.Validate(new UpdateProductDetailsCommand(
            Guid.Empty,
            "Updated Product",
            null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateProductDetailsCommand.ProductId));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithInvalidName_Fails(string name)
    {
        var result = _validator.Validate(new UpdateProductDetailsCommand(
            Guid.NewGuid(),
            name,
            null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateProductDetailsCommand.Name));
    }

    [Fact]
    public void Validate_WithOverlongName_Fails()
    {
        var result = _validator.Validate(new UpdateProductDetailsCommand(
            Guid.NewGuid(),
            new string('A', ProductName.MaxLength + 1),
            null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateProductDetailsCommand.Name));
    }

    [Fact]
    public void Validate_WithOverlongDescription_Fails()
    {
        var result = _validator.Validate(new UpdateProductDetailsCommand(
            Guid.NewGuid(),
            "Updated Product",
            new string('A', Product.DescriptionMaxLength + 1)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateProductDetailsCommand.Description));
    }
}
