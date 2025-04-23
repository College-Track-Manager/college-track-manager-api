using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeTrackAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackRelationshipToStudentRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Track",
                table: "Registrations");

            migrationBuilder.AddColumn<int>(
                name: "TrackId",
                table: "Registrations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_TrackId",
                table: "Registrations",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Tracks_TrackId",
                table: "Registrations",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Tracks_TrackId",
                table: "Registrations");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_TrackId",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "Registrations");

            migrationBuilder.AddColumn<string>(
                name: "Track",
                table: "Registrations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
