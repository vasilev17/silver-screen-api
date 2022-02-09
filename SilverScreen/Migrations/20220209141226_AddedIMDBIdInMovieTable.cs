using Microsoft.EntityFrameworkCore.Migrations;

namespace SilverScreen.Migrations
{
    public partial class AddedIMDBIdInMovieTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IMDB_ID",
                table: "Movies",
                type: "varchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IMDB_U",
                table: "Movies",
                column: "IMDB_ID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IMDB_U",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "IMDB_ID",
                table: "Movies");
        }
    }
}
