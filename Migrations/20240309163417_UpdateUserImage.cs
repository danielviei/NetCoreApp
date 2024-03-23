using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Img",
                table: "Users",
                newName: "ImagePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Users",
                newName: "Img");
        }
    }
}
