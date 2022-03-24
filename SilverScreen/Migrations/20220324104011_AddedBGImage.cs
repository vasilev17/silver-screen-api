using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SilverScreen.Migrations
{
    public partial class AddedBGImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecificReleaseDate",
                table: "Movies");

            migrationBuilder.AddColumn<string>(
                name: "BGImage",
                table: "Movies",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BGImage",
                table: "Movies");

            migrationBuilder.AddColumn<DateTime>(
                name: "SpecificReleaseDate",
                table: "Movies",
                type: "date",
                nullable: true);
        }
    }
}
