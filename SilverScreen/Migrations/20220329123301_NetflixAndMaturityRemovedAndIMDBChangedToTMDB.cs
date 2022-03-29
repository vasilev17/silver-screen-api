using Microsoft.EntityFrameworkCore.Migrations;

namespace SilverScreen.Migrations
{
    public partial class NetflixAndMaturityRemovedAndIMDBChangedToTMDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IMDB_U",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "IMDB_ID",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "MaturityRating",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "NetflixURL",
                table: "Movies");

            migrationBuilder.AlterColumn<string>(
                name: "BGImage",
                table: "Movies",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                defaultValueSql: "''",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "TMDB_ID",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TMDB_ID",
                table: "Movies");

            migrationBuilder.AlterColumn<string>(
                name: "BGImage",
                table: "Movies",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldDefaultValueSql: "''");

            migrationBuilder.AddColumn<string>(
                name: "IMDB_ID",
                table: "Movies",
                type: "varchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValueSql: "''");

            migrationBuilder.AddColumn<string>(
                name: "MaturityRating",
                table: "Movies",
                type: "varchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NetflixURL",
                table: "Movies",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IMDB_U",
                table: "Movies",
                column: "IMDB_ID",
                unique: true);
        }
    }
}
