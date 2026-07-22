using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace caportal.Migrations
{
    /// <inheritdoc />
    public partial class AddCoveredServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoveredServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoveredServices", x => x.Id);
                });

            migrationBuilder.AddColumn<string>(
                name: "ServicesBadge",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "What We Cover");

            migrationBuilder.AddColumn<string>(
                name: "ServicesTitle",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Comprehensive Solutions <span>for Your Business</span>");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoveredServices");

            migrationBuilder.DropColumn(
                name: "ServicesBadge",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "ServicesTitle",
                table: "SiteSettings");
        }
    }
}
