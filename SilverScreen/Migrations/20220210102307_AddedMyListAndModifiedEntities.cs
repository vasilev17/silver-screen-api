using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace SilverScreen.Migrations
{
    public partial class AddedMyListAndModifiedEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MoveID",
                table: "MovieStaff",
                newName: "MovieID");

            migrationBuilder.AddColumn<bool>(
                name: "IsSeries",
                table: "Movies",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "MyList",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    MovieID = table.Column<int>(type: "int", nullable: false),
                    Watched = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyList", x => x.ID);
                    table.ForeignKey(
                        name: "WMMovieFK",
                        column: x => x.MovieID,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "WMUserFK",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "WMMovieFK",
                table: "MyList",
                column: "MovieID");

            migrationBuilder.CreateIndex(
                name: "WMUserFK",
                table: "MyList",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MyList");

            migrationBuilder.DropColumn(
                name: "IsSeries",
                table: "Movies");

            migrationBuilder.RenameColumn(
                name: "MovieID",
                table: "MovieStaff",
                newName: "MoveID");
        }
    }
}
