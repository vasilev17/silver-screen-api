using Microsoft.EntityFrameworkCore.Migrations;

namespace SilverScreen.Migrations
{
    public partial class AddedFriendOnlyToComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFriendsOnly",
                table: "Comments",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFriendsOnly",
                table: "Comments");
        }
    }
}
