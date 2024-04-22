using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OverTask.Migrations
{
    /// <inheritdoc />
    public partial class pueCluster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "End",
                table: "PersonalUserEvents");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "PersonalUserEvents",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "Start",
                table: "PersonalUserEvents",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "PersonalUserEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "PersonalUserEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "PersonalUserEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "PersonalUserEvents");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "PersonalUserEvents");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "PersonalUserEvents");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "PersonalUserEvents",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PersonalUserEvents",
                newName: "Start");

            migrationBuilder.AddColumn<string>(
                name: "End",
                table: "PersonalUserEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
