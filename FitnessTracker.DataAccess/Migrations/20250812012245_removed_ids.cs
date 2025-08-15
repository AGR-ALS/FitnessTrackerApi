using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class removed_ids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sets_Exercises_WorkoutId_ExerciseId",
                table: "Sets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sets",
                table: "Sets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exercises",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "ExerciseId",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Exercises");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sets",
                table: "Sets",
                column: "WorkoutId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exercises",
                table: "Exercises",
                column: "WorkoutId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_Exercises_WorkoutId",
                table: "Sets",
                column: "WorkoutId",
                principalTable: "Exercises",
                principalColumn: "WorkoutId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sets_Exercises_WorkoutId",
                table: "Sets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sets",
                table: "Sets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exercises",
                table: "Exercises");

            migrationBuilder.AddColumn<string>(
                name: "ExerciseId",
                table: "Sets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Sets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Exercises",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sets",
                table: "Sets",
                columns: new[] { "WorkoutId", "ExerciseId", "Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exercises",
                table: "Exercises",
                columns: new[] { "WorkoutId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_Exercises_WorkoutId_ExerciseId",
                table: "Sets",
                columns: new[] { "WorkoutId", "ExerciseId" },
                principalTable: "Exercises",
                principalColumns: new[] { "WorkoutId", "Id" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
