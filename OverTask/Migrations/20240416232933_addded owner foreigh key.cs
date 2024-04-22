using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OverTask.Migrations
{
    /// <inheritdoc />
    public partial class adddedownerforeighkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Groups_OwnerID",
                table: "Groups",
                column: "OwnerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AspNetUsers_OwnerID",
                table: "Groups",
                column: "OwnerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AspNetUsers_OwnerID",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_OwnerID",
                table: "Groups");
        }
    }
}
