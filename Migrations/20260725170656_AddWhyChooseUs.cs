using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace caportal.Migrations
{
    /// <inheritdoc />
    public partial class AddWhyChooseUs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WhyChooseUsBadge",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WhyChooseUsStat1Lbl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WhyChooseUsStat1Val",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WhyChooseUsStat2Lbl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WhyChooseUsStat2Val",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WhyChooseUsStat3Lbl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WhyChooseUsStat3Val",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WhyChooseUsStat4Lbl",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WhyChooseUsStat4Val",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WhyChooseUsStatsTitle",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WhyChooseUsSub",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WhyChooseUsTitle",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "CoveredServices",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "");

            migrationBuilder.CreateTable(
                name: "WhyChooseUsItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhyChooseUsItems", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WhyChooseUsItems");

            migrationBuilder.DropColumn(
                name: "WhyChooseUsBadge",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "WhyChooseUsStat1Lbl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "WhyChooseUsStat1Val",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "WhyChooseUsStat2Lbl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "WhyChooseUsStat2Val",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "WhyChooseUsStat3Lbl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "WhyChooseUsStat3Val",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "WhyChooseUsStat4Lbl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "WhyChooseUsStat4Val",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "WhyChooseUsStatsTitle",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "WhyChooseUsSub",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "WhyChooseUsTitle",
                table: "SiteSettings");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "CoveredServices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
