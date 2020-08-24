using Microsoft.EntityFrameworkCore.Migrations;

namespace Duckify.Data.ApplicationContext
{
    public partial class _004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowedEmailsStr",
                table: "AppSettings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedEmailsStr",
                table: "AppSettings");
        }
    }
}
