using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OverTask.Migrations
{
    /// <inheritdoc />
    public partial class changedisCheckedtoCompleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsChecked",
                table: "ToDoTasks",
                newName: "Completed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Completed",
                table: "ToDoTasks",
                newName: "IsChecked");
        }
    }
}
