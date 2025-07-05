using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations.Internal
{
    /// <inheritdoc />
    public partial class AddIsCriticalToRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isCritical",
                table: "Requests",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isCritical",
                table: "Requests");
        }
    }
}
