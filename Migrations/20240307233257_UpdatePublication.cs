using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePublication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Publications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Publications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
