using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations.Internal
{
    /// <inheritdoc />
    public partial class MergeFullNameColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "full_name",
                table: "RuleFullNames",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(@"
        UPDATE ""RuleFullNames""
        SET ""full_name"" = TRIM(
            COALESCE(""surname"", '') || ' ' ||
            COALESCE(""name"", '') || ' ' ||
            COALESCE(""patronymic"", '')
        );
    ");

            migrationBuilder.DropColumn(
                name: "surname",
                table: "RuleFullNames");

            migrationBuilder.DropColumn(
                name: "name",
                table: "RuleFullNames");

            migrationBuilder.DropColumn(
                name: "patronymic",
                table: "RuleFullNames");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "surname",
                table: "RuleFullNames",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "RuleFullNames",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "patronymic",
                table: "RuleFullNames",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(@"
        UPDATE ""RuleFullNames""
        SET
            ""surname"" = split_part(""full_name"", ' ', 1),
            ""name"" = split_part(""full_name"", ' ', 2),
            ""patronymic"" = split_part(""full_name"", ' ', 3)
        WHERE ""full_name"" IS NOT NULL;
    ");

            migrationBuilder.DropColumn(
                name: "full_name",
                table: "RuleFullNames");
        }

    }
}
