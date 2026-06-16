using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Catalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductStatusCreatedAtIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Products_Status_CreatedAtUtc",
                schema: "catalog",
                table: "Products",
                columns: new[] { "Status", "CreatedAtUtc" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Status_CreatedAtUtc",
                schema: "catalog",
                table: "Products");
        }
    }
}
