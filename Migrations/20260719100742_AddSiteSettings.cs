using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace caportal.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    Id               = table.Column<int>(type: "int", nullable: false),
                    SiteName         = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteTagline      = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiteEmail        = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SitePhone        = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogoPath         = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogoSmallPath    = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogoAlt          = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeaderBgColor    = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeaderFontColor  = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeaderFontSize   = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeaderFontFamily = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeaderSticky     = table.Column<bool>(type: "bit", nullable: false),
                    ShowTopBar       = table.Column<bool>(type: "bit", nullable: false),
                    TopBarBgColor    = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TopBarFontColor  = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyBgColor      = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyFontColor    = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyFontSize     = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BodyFontFamily   = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeroBgFrom       = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeroBgTo         = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeroTitleColor   = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeroSubColor     = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeroAccentColor  = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeroTitle        = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeroSubtitle     = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccentColor      = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccentColorLight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimaryColor     = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimaryLight     = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FooterBgColor    = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FooterFontColor  = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FooterText       = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeadingFont      = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeadingColor     = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    H1Size           = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    H2Size           = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    H3Size           = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BtnPrimaryBg     = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BtnPrimaryColor  = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BtnBorderRadius  = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "SiteSettings");
        }
    }
}
