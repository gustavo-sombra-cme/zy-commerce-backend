using Ecommerce.Catalog.Application.Products.SearchProducts;

namespace Ecommerce.Catalog.UnitTests.Products;

public sealed class SearchProductsQueryValidatorTests
{
    private readonly SearchProductsQueryValidator _validator = new();

    [Fact]
    public void Validate_WithDefaultPagination_Succeeds()
    {
        var result = _validator.Validate(new SearchProductsQuery(null, null, null, null));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithPageNumberLessThanOne_Fails(int pageNumber)
    {
        var result = _validator.Validate(new SearchProductsQuery(null, null, pageNumber, 20));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(SearchProductsQuery.PageNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithPageSizeLessThanOne_Fails(int pageSize)
    {
        var result = _validator.Validate(new SearchProductsQuery(null, null, 1, pageSize));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(SearchProductsQuery.PageSize));
    }

    [Fact]
    public void Validate_WithPageSizeGreaterThanMaximum_Fails()
    {
        var result = _validator.Validate(new SearchProductsQuery(
            null,
            null,
            1,
            SearchProductsQueryValidator.MaxPageSize + 1));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(SearchProductsQuery.PageSize));
    }

    [Fact]
    public void Validate_WithValidSearchAndFilter_Succeeds()
    {
        var result = _validator.Validate(new SearchProductsQuery("SKU", true, 1, 20));

        Assert.True(result.IsValid);
    }
}
