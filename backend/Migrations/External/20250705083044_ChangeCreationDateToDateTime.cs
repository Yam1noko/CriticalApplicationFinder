using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations.External
{
    /// <inheritdoc />
    public partial class ChangeCreationDateToDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Requests""
                ALTER COLUMN ""CreationDate"" TYPE timestamp with time zone
                USING ""CreationDate""::timestamp with time zone;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Requests""
                ALTER COLUMN ""CreationDate"" TYPE text
                USING ""CreationDate""::text;
            ");
        }
    }
}
