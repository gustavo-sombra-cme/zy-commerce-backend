using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Catalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAssistantCatalogReadOnlyViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "assistant");

            migrationBuilder.Sql(
                """
                CREATE OR ALTER VIEW [assistant].[v_ProductSearch]
                AS
                SELECT
                    [p].[Id] AS [ProductId],
                    [p].[Name],
                    [p].[Sku],
                    [p].[Description],
                    [p].[Price] AS [PriceAmount],
                    [p].[IsActive],
                    [p].[CreatedAt],
                    [p].[UpdatedAt]
                FROM [catalog].[Products] AS [p];
                """);

            migrationBuilder.Sql(
                """
                CREATE OR ALTER VIEW [assistant].[v_ProductDetails]
                AS
                SELECT
                    [p].[Id] AS [ProductId],
                    [p].[Name],
                    [p].[Sku],
                    [p].[Description],
                    [p].[Price] AS [PriceAmount],
                    [p].[IsActive],
                    [p].[CreatedAt],
                    [p].[UpdatedAt]
                FROM [catalog].[Products] AS [p];
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS [assistant].[v_ProductDetails];");
            migrationBuilder.Sql("DROP VIEW IF EXISTS [assistant].[v_ProductSearch];");
            migrationBuilder.Sql("IF SCHEMA_ID(N'assistant') IS NOT NULL EXEC(N'DROP SCHEMA [assistant]');");
        }
    }
}
