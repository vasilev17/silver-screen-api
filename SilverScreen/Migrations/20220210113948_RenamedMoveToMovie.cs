using Microsoft.EntityFrameworkCore.Migrations;

namespace SilverScreen.Migrations
{
    public partial class RenamedMoveToMovie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE `MovieStaff` CHANGE `MoveID` `MovieID` INT NOT NULL;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE `MovieStaff` CHANGE `MovieID` `MoveID` INT NOT NULL;");
        }
    }
}
