using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Orders.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAssistantReadOnlyViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "assistant");

            migrationBuilder.Sql(
                """
                CREATE OR ALTER VIEW [assistant].[v_MyOrders]
                AS
                SELECT
                    [o].[Id] AS [OrderId],
                    [o].[BuyerId] AS [BuyerUserId],
                    [o].[Status],
                    [o].[TotalAmount],
                    [o].[CreatedAt],
                    COUNT([ol].[Id]) AS [LineCount]
                FROM [orders].[Orders] AS [o]
                LEFT JOIN [orders].[OrderLines] AS [ol] ON [ol].[OrderId] = [o].[Id]
                GROUP BY
                    [o].[Id],
                    [o].[BuyerId],
                    [o].[Status],
                    [o].[TotalAmount],
                    [o].[CreatedAt];
                """);

            migrationBuilder.Sql(
                """
                CREATE OR ALTER VIEW [assistant].[v_MyOrderLines]
                AS
                SELECT
                    [o].[Id] AS [OrderId],
                    [o].[BuyerId] AS [BuyerUserId],
                    [ol].[ProductId],
                    [ol].[ProductName],
                    [ol].[ProductSku],
                    [ol].[Quantity],
                    [ol].[UnitPrice] AS [UnitPriceAmount],
                    CAST([ol].[UnitPrice] * [ol].[Quantity] AS decimal(18, 2)) AS [LineTotal]
                FROM [orders].[Orders] AS [o]
                INNER JOIN [orders].[OrderLines] AS [ol] ON [ol].[OrderId] = [o].[Id];
                """);

            migrationBuilder.Sql(
                """
                CREATE OR ALTER VIEW [assistant].[v_MyOrderSummary]
                AS
                SELECT
                    [o].[BuyerId] AS [BuyerUserId],
                    COUNT_BIG(*) AS [TotalOrders],
                    CAST(SUM([o].[TotalAmount]) AS decimal(18, 2)) AS [TotalSpend],
                    MAX([o].[CreatedAt]) AS [LastOrderDate]
                FROM [orders].[Orders] AS [o]
                GROUP BY [o].[BuyerId];
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS [assistant].[v_MyOrderSummary];");
            migrationBuilder.Sql("DROP VIEW IF EXISTS [assistant].[v_MyOrderLines];");
            migrationBuilder.Sql("DROP VIEW IF EXISTS [assistant].[v_MyOrders];");
            migrationBuilder.Sql("IF SCHEMA_ID(N'assistant') IS NOT NULL EXEC(N'DROP SCHEMA [assistant]');");
        }
    }
}
