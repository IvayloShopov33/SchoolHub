using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChatMessages_AddIsReadAndRemovedRecipientId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipientId",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<string>(
                name: "ClassId",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "ChatMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<string>(
                name: "ClassId",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "RecipientId",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
