using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations.External
{
    /// <inheritdoc />
    public partial class AddCreationDateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Requests_CreationDate",
                table: "Requests",
                column: "CreationDate",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Requests_CreationDate",
                table: "Requests");
        }
    }
}
