using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace SilverScreen.Migrations
{
    public partial class FixedStaffAndGenreTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "GMovieFK",
                table: "Genre");

            migrationBuilder.DropForeignKey(
                name: "StaffMovieFK",
                table: "Staff");

            migrationBuilder.DropTable(
                name: "WatchedMovie");

            migrationBuilder.DropIndex(
                name: "StaffMovieFK",
                table: "Staff");

            migrationBuilder.DropIndex(
                name: "GMovieFK",
                table: "Genre");

            migrationBuilder.DropColumn(
                name: "MovieID",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "MovieID",
                table: "Genre");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "User",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAdmin",
                table: "User",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "User",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAdmin",
                table: "User",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AddColumn<int>(
                name: "MovieID",
                table: "Staff",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MovieID",
                table: "Genre",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WatchedMovie",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    MovieID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Watched = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchedMovie", x => x.ID);
                    table.ForeignKey(
                        name: "WMovieFK",
                        column: x => x.MovieID,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "WUserFK",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "StaffMovieFK",
                table: "Staff",
                column: "MovieID");

            migrationBuilder.CreateIndex(
                name: "GMovieFK",
                table: "Genre",
                column: "MovieID");

            migrationBuilder.CreateIndex(
                name: "WMovieFK",
                table: "WatchedMovie",
                column: "MovieID");

            migrationBuilder.CreateIndex(
                name: "WUserFK",
                table: "WatchedMovie",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "GMovieFK",
                table: "Genre",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "StaffMovieFK",
                table: "Staff",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
