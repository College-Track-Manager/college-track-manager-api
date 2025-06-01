using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeTrackAPI.Migrations
{
    /// <inheritdoc />
    public partial class reg_adminComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminComments",
                table: "Registrations",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminComments",
                table: "Registrations");
        }
    }
}
