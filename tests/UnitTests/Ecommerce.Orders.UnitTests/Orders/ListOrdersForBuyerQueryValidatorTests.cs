using Ecommerce.Orders.Application.Orders.ListOrdersForBuyer;

namespace Ecommerce.Orders.UnitTests.Orders;

public sealed class ListOrdersForBuyerQueryValidatorTests
{
    private readonly ListOrdersForBuyerQueryValidator _validator = new();

    [Fact]
    public void Validate_WithDefaultPagination_Succeeds()
    {
        var result = _validator.Validate(new ListOrdersForBuyerQuery(Guid.NewGuid(), null, null));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEmptyBuyerId_Fails()
    {
        var result = _validator.Validate(new ListOrdersForBuyerQuery(Guid.Empty, 1, 20));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ListOrdersForBuyerQuery.BuyerId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithPageNumberLessThanOne_Fails(int pageNumber)
    {
        var result = _validator.Validate(new ListOrdersForBuyerQuery(Guid.NewGuid(), pageNumber, 20));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ListOrdersForBuyerQuery.PageNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithPageSizeLessThanOne_Fails(int pageSize)
    {
        var result = _validator.Validate(new ListOrdersForBuyerQuery(Guid.NewGuid(), 1, pageSize));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ListOrdersForBuyerQuery.PageSize));
    }

    [Fact]
    public void Validate_WithPageSizeGreaterThanMaximum_Fails()
    {
        var result = _validator.Validate(new ListOrdersForBuyerQuery(
            Guid.NewGuid(),
            1,
            ListOrdersForBuyerQueryValidator.MaxPageSize + 1));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ListOrdersForBuyerQuery.PageSize));
    }

    [Fact]
    public void Validate_WithMaxPageSize_Succeeds()
    {
        var result = _validator.Validate(new ListOrdersForBuyerQuery(
            Guid.NewGuid(),
            1,
            ListOrdersForBuyerQueryValidator.MaxPageSize));

        Assert.True(result.IsValid);
    }
}
