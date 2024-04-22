using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OverTask.Migrations
{
    /// <inheritdoc />
    public partial class minorspellingmistake : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupEvents_Groups_GroupdId",
                table: "GroupEvents");

            migrationBuilder.DropIndex(
                name: "IX_GroupEvents_GroupdId",
                table: "GroupEvents");

            migrationBuilder.DropColumn(
                name: "GroupdId",
                table: "GroupEvents");

            migrationBuilder.CreateIndex(
                name: "IX_GroupEvents_GroupId",
                table: "GroupEvents",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupEvents_Groups_GroupId",
                table: "GroupEvents",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupEvents_Groups_GroupId",
                table: "GroupEvents");

            migrationBuilder.DropIndex(
                name: "IX_GroupEvents_GroupId",
                table: "GroupEvents");

            migrationBuilder.AddColumn<int>(
                name: "GroupdId",
                table: "GroupEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GroupEvents_GroupdId",
                table: "GroupEvents",
                column: "GroupdId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupEvents_Groups_GroupdId",
                table: "GroupEvents",
                column: "GroupdId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
