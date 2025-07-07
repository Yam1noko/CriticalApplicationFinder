using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations.Internal
{
    /// <inheritdoc />
    public partial class GermansMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rules_RuleFullNames_FullName",
                table: "Rules");

            migrationBuilder.DropForeignKey(
                name: "FK_Rules_RuleSubstrings_Substring",
                table: "Rules");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_RuleSubstrings_Substring",
                table: "RuleSubstrings");

            migrationBuilder.DropIndex(
                name: "IX_RuleSubstrings_Substring",
                table: "RuleSubstrings");

            migrationBuilder.DropIndex(
                name: "IX_Rules_FullName",
                table: "Rules");

            migrationBuilder.DropIndex(
                name: "IX_Rules_Substring",
                table: "Rules");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_RuleFullNames_FullName",
                table: "RuleFullNames");

            migrationBuilder.DropIndex(
                name: "IX_RuleFullNames_FullName",
                table: "RuleFullNames");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Rules");

            migrationBuilder.RenameColumn(
                name: "Substring",
                table: "RuleSubstrings",
                newName: "substring");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "RuleSubstrings",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Rules",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UseAnd",
                table: "Rules",
                newName: "use_and");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Rules",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "Substring",
                table: "Rules",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "RuleFullNames",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "RuleFullNames",
                newName: "surname");

            migrationBuilder.AddColumn<int>(
                name: "rule_id",
                table: "RuleSubstrings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "RuleFullNames",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "patronymic",
                table: "RuleFullNames",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "rule_id",
                table: "RuleFullNames",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RuleSubstrings_rule_id",
                table: "RuleSubstrings",
                column: "rule_id");

            migrationBuilder.CreateIndex(
                name: "IX_RuleFullNames_rule_id",
                table: "RuleFullNames",
                column: "rule_id");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleFullNames_Rules_rule_id",
                table: "RuleFullNames",
                column: "rule_id",
                principalTable: "Rules",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RuleSubstrings_Rules_rule_id",
                table: "RuleSubstrings",
                column: "rule_id",
                principalTable: "Rules",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RuleFullNames_Rules_rule_id",
                table: "RuleFullNames");

            migrationBuilder.DropForeignKey(
                name: "FK_RuleSubstrings_Rules_rule_id",
                table: "RuleSubstrings");

            migrationBuilder.DropIndex(
                name: "IX_RuleSubstrings_rule_id",
                table: "RuleSubstrings");

            migrationBuilder.DropIndex(
                name: "IX_RuleFullNames_rule_id",
                table: "RuleFullNames");

            migrationBuilder.DropColumn(
                name: "rule_id",
                table: "RuleSubstrings");

            migrationBuilder.DropColumn(
                name: "name",
                table: "RuleFullNames");

            migrationBuilder.DropColumn(
                name: "patronymic",
                table: "RuleFullNames");

            migrationBuilder.DropColumn(
                name: "rule_id",
                table: "RuleFullNames");

            migrationBuilder.RenameColumn(
                name: "substring",
                table: "RuleSubstrings",
                newName: "Substring");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RuleSubstrings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Rules",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "use_and",
                table: "Rules",
                newName: "UseAnd");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Rules",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Rules",
                newName: "Substring");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RuleFullNames",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "surname",
                table: "RuleFullNames",
                newName: "FullName");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Rules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_RuleSubstrings_Substring",
                table: "RuleSubstrings",
                column: "Substring");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_RuleFullNames_FullName",
                table: "RuleFullNames",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_RuleSubstrings_Substring",
                table: "RuleSubstrings",
                column: "Substring",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rules_FullName",
                table: "Rules",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_Substring",
                table: "Rules",
                column: "Substring");

            migrationBuilder.CreateIndex(
                name: "IX_RuleFullNames_FullName",
                table: "RuleFullNames",
                column: "FullName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rules_RuleFullNames_FullName",
                table: "Rules",
                column: "FullName",
                principalTable: "RuleFullNames",
                principalColumn: "FullName",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rules_RuleSubstrings_Substring",
                table: "Rules",
                column: "Substring",
                principalTable: "RuleSubstrings",
                principalColumn: "Substring",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
