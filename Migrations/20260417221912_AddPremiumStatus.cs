using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WasteManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPremiumStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPremium",
                table: "Houses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPremium",
                table: "Houses");
        }
    }
}
