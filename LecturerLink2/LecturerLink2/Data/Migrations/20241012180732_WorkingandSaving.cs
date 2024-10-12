using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LecturerLink2.Data.Migrations
{
    /// <inheritdoc />
    public partial class WorkingandSaving : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Claims");
        }
    }
}
