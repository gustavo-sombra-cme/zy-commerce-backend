using Ecommerce.Catalog.Domain.Products;

namespace Ecommerce.Catalog.UnitTests.Products;

public sealed class ProductTests
{
    [Fact]
    public void Create_WithValidValues_CreatesActiveProduct()
    {
        var createdAt = DateTimeOffset.UtcNow;

        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            "Test description",
            12.34m,
            createdAt);

        Assert.NotEqual(Guid.Empty, product.Id.Value);
        Assert.Equal("SKU-001", product.Sku.Value);
        Assert.Equal("Test Product", product.Name.Value);
        Assert.Equal("Test description", product.Description);
        Assert.Equal(12.34m, product.Price);
        Assert.True(product.IsActive);
        Assert.Equal(createdAt, product.CreatedAt);
        Assert.Null(product.UpdatedAt);
    }

    [Fact]
    public void Create_WithWhitespaceDescription_StoresNullDescription()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            "   ",
            DateTimeOffset.UtcNow);

        Assert.Null(product.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("SKU 001")]
    [InlineData("SKU.001")]
    public void Sku_Create_WithInvalidValue_Throws(string sku)
    {
        Assert.Throws<ArgumentException>(() => Sku.Create(sku));
    }

    [Fact]
    public void Sku_Create_WithLowercaseValue_NormalizesToUppercase()
    {
        var sku = Sku.Create("sku-001");

        Assert.Equal("SKU-001", sku.Value);
    }

    [Fact]
    public void Sku_Create_WithOverlongValue_Throws()
    {
        var sku = new string('A', Sku.MaxLength + 1);

        Assert.Throws<ArgumentException>(() => Sku.Create(sku));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ProductName_Create_WithInvalidValue_Throws(string name)
    {
        Assert.Throws<ArgumentException>(() => ProductName.Create(name));
    }

    [Fact]
    public void ProductName_Create_WithOverlongValue_Throws()
    {
        var name = new string('A', ProductName.MaxLength + 1);

        Assert.Throws<ArgumentException>(() => ProductName.Create(name));
    }

    [Fact]
    public void Create_WithOverlongDescription_Throws()
    {
        var description = new string('A', Product.DescriptionMaxLength + 1);

        Assert.Throws<ArgumentException>(() => Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            description,
            DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Deactivate_WithActiveProduct_SetsInactiveAndUpdatedAt()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            DateTimeOffset.UtcNow);
        var updatedAt = DateTimeOffset.UtcNow.AddMinutes(1);

        product.Deactivate(updatedAt);

        Assert.False(product.IsActive);
        Assert.Equal(updatedAt, product.UpdatedAt);
    }

    [Fact]
    public void Deactivate_WithInactiveProduct_IsIdempotent()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            DateTimeOffset.UtcNow);
        var firstUpdatedAt = DateTimeOffset.UtcNow.AddMinutes(1);
        var secondUpdatedAt = DateTimeOffset.UtcNow.AddMinutes(2);

        product.Deactivate(firstUpdatedAt);
        product.Deactivate(secondUpdatedAt);

        Assert.False(product.IsActive);
        Assert.Equal(firstUpdatedAt, product.UpdatedAt);
    }

    [Fact]
    public void Deactivate_WithDefaultUpdatedAt_Throws()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            DateTimeOffset.UtcNow);

        Assert.Throws<ArgumentException>(() => product.Deactivate(default));
    }

    [Fact]
    public void Create_WithNegativePrice_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            -0.01m,
            DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Create_WithMoreThanTwoDecimalPlaces_RoundsPrice()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            12.345m,
            DateTimeOffset.UtcNow);

        Assert.Equal(12.35m, product.Price);
    }

    [Fact]
    public void Reactivate_WithInactiveProduct_SetsActiveAndUpdatedAt()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            DateTimeOffset.UtcNow);
        var productId = product.Id;
        var sku = product.Sku;
        var deactivatedAt = DateTimeOffset.UtcNow.AddMinutes(1);
        var reactivatedAt = DateTimeOffset.UtcNow.AddMinutes(2);

        product.Deactivate(deactivatedAt);
        product.Reactivate(reactivatedAt);

        Assert.True(product.IsActive);
        Assert.Equal(reactivatedAt, product.UpdatedAt);
        Assert.Equal(productId, product.Id);
        Assert.Equal(sku, product.Sku);
    }

    [Fact]
    public void Reactivate_WithActiveProduct_IsIdempotent()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            DateTimeOffset.UtcNow);
        var originalUpdatedAt = DateTimeOffset.UtcNow.AddMinutes(1);

        product.UpdateDetails(
            ProductName.Create("Updated Product"),
            "Updated description",
            originalUpdatedAt);
        product.Reactivate(DateTimeOffset.UtcNow.AddMinutes(2));

        Assert.True(product.IsActive);
        Assert.Equal(originalUpdatedAt, product.UpdatedAt);
    }

    [Fact]
    public void Reactivate_WithActiveProductAndDefaultUpdatedAt_IsIdempotent()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            DateTimeOffset.UtcNow);

        product.Reactivate(default);

        Assert.True(product.IsActive);
        Assert.Null(product.UpdatedAt);
    }

    [Fact]
    public void Reactivate_WithInactiveProductAndDefaultUpdatedAt_Throws()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            DateTimeOffset.UtcNow);
        product.Deactivate(DateTimeOffset.UtcNow.AddMinutes(1));

        Assert.Throws<ArgumentException>(() => product.Reactivate(default));
    }

    [Fact]
    public void UpdateDetails_WithValidValues_UpdatesNameDescriptionAndUpdatedAt()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            "Original description",
            DateTimeOffset.UtcNow);
        var updatedAt = DateTimeOffset.UtcNow.AddMinutes(1);

        product.UpdateDetails(ProductName.Create("Updated Product"), "Updated description", updatedAt);

        Assert.Equal("Updated Product", product.Name.Value);
        Assert.Equal("Updated description", product.Description);
        Assert.Equal(updatedAt, product.UpdatedAt);
    }

    [Fact]
    public void UpdateDetails_PreservesSkuAndActiveState()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            DateTimeOffset.UtcNow);

        product.UpdateDetails(
            ProductName.Create("Updated Product"),
            "Updated description",
            DateTimeOffset.UtcNow.AddMinutes(1));

        Assert.Equal("SKU-001", product.Sku.Value);
        Assert.True(product.IsActive);
    }

    [Fact]
    public void UpdateDetails_WithWhitespaceDescription_StoresNullDescription()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            "Original description",
            DateTimeOffset.UtcNow);

        product.UpdateDetails(
            ProductName.Create("Updated Product"),
            "   ",
            DateTimeOffset.UtcNow.AddMinutes(1));

        Assert.Null(product.Description);
    }

    [Fact]
    public void UpdateDetails_WithOverlongDescription_Throws()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            DateTimeOffset.UtcNow);
        var description = new string('A', Product.DescriptionMaxLength + 1);

        Assert.Throws<ArgumentException>(() => product.UpdateDetails(
            ProductName.Create("Updated Product"),
            description,
            DateTimeOffset.UtcNow.AddMinutes(1)));
    }

    [Fact]
    public void UpdateDetails_WithDefaultUpdatedAt_Throws()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            DateTimeOffset.UtcNow);

        Assert.Throws<ArgumentException>(() => product.UpdateDetails(
            ProductName.Create("Updated Product"),
            null,
            default));
    }
}
