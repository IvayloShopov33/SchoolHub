using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class ClassHomeTeacherIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Classes_HomeroomTeacherId",
                table: "Classes");

            migrationBuilder.AlterColumn<string>(
                name: "HomeroomTeacherId",
                table: "Classes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_HomeroomTeacherId",
                table: "Classes",
                column: "HomeroomTeacherId",
                unique: true,
                filter: "[HomeroomTeacherId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Classes_HomeroomTeacherId",
                table: "Classes");

            migrationBuilder.AlterColumn<string>(
                name: "HomeroomTeacherId",
                table: "Classes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Classes_HomeroomTeacherId",
                table: "Classes",
                column: "HomeroomTeacherId",
                unique: true);
        }
    }
}
