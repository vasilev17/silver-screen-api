using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace SilverScreen.Migrations
{
    public partial class AddedAdminRelatedTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountReports",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FakeReports = table.Column<int>(type: "int", nullable: false),
                    Reports = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountReports", x => x.ID);
                    table.ForeignKey(
                        name: "UserFKAR",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BanConfig",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    FakeReportsLimit = table.Column<int>(type: "int", nullable: true),
                    WarningsLimit = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanConfig", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CommentReports",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CommentId = table.Column<int>(type: "int", nullable: false),
                    Contents = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    UnderReview = table.Column<int>(type: "int", nullable: true),
                    ReportedForFalsePositive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReportIsLegit = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentReports", x => x.ID);
                    table.ForeignKey(
                        name: "UserFKCR",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "UserFKRW",
                        column: x => x.UnderReview,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserWarnings",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    IsItBan = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWarnings", x => x.ID);
                    table.ForeignKey(
                        name: "UserFKUW",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UserFKAR",
                table: "AccountReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UserFKCR",
                table: "CommentReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UserFKRW",
                table: "CommentReports",
                column: "UnderReview");

            migrationBuilder.CreateIndex(
                name: "UserFKUW",
                table: "UserWarnings",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountReports");

            migrationBuilder.DropTable(
                name: "BanConfig");

            migrationBuilder.DropTable(
                name: "CommentReports");

            migrationBuilder.DropTable(
                name: "UserWarnings");
        }
    }
}
