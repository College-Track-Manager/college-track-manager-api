using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeTrackAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddStudyTypeToStudentRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudyType",
                table: "Registrations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudyType",
                table: "Registrations");
        }
    }
}
