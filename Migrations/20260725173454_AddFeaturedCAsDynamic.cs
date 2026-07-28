using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace caportal.Migrations
{
    /// <inheritdoc />
    public partial class AddFeaturedCAsDynamic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FeaturedCAsBadge",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FeaturedCAsSubtitle",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FeaturedCAsTitle",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeaturedCAsBadge",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "FeaturedCAsSubtitle",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "FeaturedCAsTitle",
                table: "SiteSettings");
        }
    }
}
