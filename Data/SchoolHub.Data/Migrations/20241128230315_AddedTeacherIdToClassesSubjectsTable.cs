using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedTeacherIdToClassesSubjectsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeacherId",
                table: "ClassesSubjects",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassesSubjects_TeacherId",
                table: "ClassesSubjects",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassesSubjects_Teachers_TeacherId",
                table: "ClassesSubjects",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassesSubjects_Teachers_TeacherId",
                table: "ClassesSubjects");

            migrationBuilder.DropIndex(
                name: "IX_ClassesSubjects_TeacherId",
                table: "ClassesSubjects");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "ClassesSubjects");
        }
    }
}
