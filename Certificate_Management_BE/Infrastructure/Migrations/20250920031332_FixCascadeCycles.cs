using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeCycles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAssignations_Users_ApprovedByUserId",
                table: "TraineeAssignations");

            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAssignations_Users_AssignedByUserId",
                table: "TraineeAssignations");

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAssignations_Users_ApprovedByUserId",
                table: "TraineeAssignations",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAssignations_Users_AssignedByUserId",
                table: "TraineeAssignations",
                column: "AssignedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAssignations_Users_ApprovedByUserId",
                table: "TraineeAssignations");

            migrationBuilder.DropForeignKey(
                name: "FK_TraineeAssignations_Users_AssignedByUserId",
                table: "TraineeAssignations");

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAssignations_Users_ApprovedByUserId",
                table: "TraineeAssignations",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TraineeAssignations_Users_AssignedByUserId",
                table: "TraineeAssignations",
                column: "AssignedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
