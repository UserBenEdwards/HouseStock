using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class SetNullOnCategoryDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appliances_ApplianceCategories_CategoryId",
                table: "Appliances");

            migrationBuilder.AddForeignKey(
                name: "FK_Appliances_ApplianceCategories_CategoryId",
                table: "Appliances",
                column: "CategoryId",
                principalTable: "ApplianceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appliances_ApplianceCategories_CategoryId",
                table: "Appliances");

            migrationBuilder.AddForeignKey(
                name: "FK_Appliances_ApplianceCategories_CategoryId",
                table: "Appliances",
                column: "CategoryId",
                principalTable: "ApplianceCategories",
                principalColumn: "Id");
        }
    }
}
