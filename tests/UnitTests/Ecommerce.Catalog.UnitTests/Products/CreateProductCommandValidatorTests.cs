using Ecommerce.Catalog.Application.Products.CreateProduct;
using Ecommerce.Catalog.Domain.Products;

namespace Ecommerce.Catalog.UnitTests.Products;

public sealed class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_Succeeds()
    {
        var result = _validator.Validate(new CreateProductCommand("SKU-001", "Test Product", "Description"));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("sku-001")]
    [InlineData("SKU 001")]
    [InlineData("SKU.001")]
    public void Validate_WithInvalidSku_Fails(string sku)
    {
        var result = _validator.Validate(new CreateProductCommand(sku, "Test Product", null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductCommand.Sku));
    }

    [Fact]
    public void Validate_WithOverlongSku_Fails()
    {
        var result = _validator.Validate(new CreateProductCommand(
            new string('A', Sku.MaxLength + 1),
            "Test Product",
            null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductCommand.Sku));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithInvalidName_Fails(string name)
    {
        var result = _validator.Validate(new CreateProductCommand("SKU-001", name, null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductCommand.Name));
    }

    [Fact]
    public void Validate_WithOverlongName_Fails()
    {
        var result = _validator.Validate(new CreateProductCommand(
            "SKU-001",
            new string('A', ProductName.MaxLength + 1),
            null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductCommand.Name));
    }

    [Fact]
    public void Validate_WithOverlongDescription_Fails()
    {
        var result = _validator.Validate(new CreateProductCommand(
            "SKU-001",
            "Test Product",
            new string('A', Product.DescriptionMaxLength + 1)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateProductCommand.Description));
    }
}
