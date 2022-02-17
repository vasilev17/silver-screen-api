using Microsoft.EntityFrameworkCore.Migrations;

namespace SilverScreen.Migrations
{
    public partial class ChangedMaturityRatingType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MaturityRating",
                table: "Movies",
                type: "varchar(5)",
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "enum('G','PG','PG-13','R','NC-17','NULL')",
                oldDefaultValueSql: "'NULL'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MaturityRating",
                table: "Movies",
                type: "enum('G','PG','PG-13','R','NC-17','NULL')",
                nullable: false,
                defaultValueSql: "'NULL'",
                oldClrType: typeof(string),
                oldType: "varchar(5)",
                oldMaxLength: 5,
                oldNullable: true);
        }
    }
}
