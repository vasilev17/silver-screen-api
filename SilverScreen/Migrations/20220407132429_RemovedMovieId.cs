using Microsoft.EntityFrameworkCore.Migrations;

namespace SilverScreen.Migrations
{
    public partial class RemovedMovieId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MovieId",
                table: "CommentReports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_MovieId",
                table: "CommentReports",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReports_Movies_MovieId",
                table: "CommentReports",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_Movies_MovieId",
                table: "CommentReports");

            migrationBuilder.DropIndex(
                name: "IX_CommentReports_MovieId",
                table: "CommentReports");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "CommentReports");
        }
    }
}
