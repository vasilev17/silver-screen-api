using Microsoft.EntityFrameworkCore.Migrations;

namespace SilverScreen.Migrations
{
    public partial class NetflixAndMaturityRemovedAndIMDBChangedToTMDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE `Movies` DROP INDEX `IMDB_U`;");
            migrationBuilder.Sql(@"UPDATE Movies SET IMDB_ID = REPLACE(IMDB_ID, 'tt', '');");
            migrationBuilder.Sql(@"UPDATE Movies SET IMDB_ID = REPLACE(IMDB_ID, 'movie', '');");
            migrationBuilder.Sql(@"UPDATE Movies SET IMDB_ID = REPLACE(IMDB_ID, 'tv', '');");
            migrationBuilder.Sql(@"ALTER TABLE `Movies` CHANGE `IMDB_ID` `TMDB_ID` INT NOT NULL;");
            migrationBuilder.Sql(@"ALTER TABLE `Movies` DROP `MaturityRating`;");
            migrationBuilder.Sql(@"ALTER TABLE `Movies` DROP `NetflixURL`;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE `Movies` ADD `MaturityRating` VARCHAR(5) NULL;");
            migrationBuilder.Sql(@"ALTER TABLE `Movies` ADD `NetflixURL` VARCHAR(100) NULL;");
            migrationBuilder.Sql(@"ALTER TABLE `Movies` CHANGE `TMDB_ID` `IMDB_ID` VARCHAR(15) NOT NULL;");
            migrationBuilder.Sql(@"ALTER TABLE `Movies` ADD CONSTRAINT IMDB_U UNIQUE(`IMDB_ID`);");
        }
    }
}
