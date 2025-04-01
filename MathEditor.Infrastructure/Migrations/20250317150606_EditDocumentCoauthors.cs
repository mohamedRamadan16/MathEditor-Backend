using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MathEditor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditDocumentCoauthors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentCoauthors",
                table: "DocumentCoauthors");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "DocumentCoauthors");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "DocumentCoauthors",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentCoauthors",
                table: "DocumentCoauthors",
                columns: new[] { "DocumentId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentCoauthors",
                table: "DocumentCoauthors");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "DocumentCoauthors",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "DocumentCoauthors",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentCoauthors",
                table: "DocumentCoauthors",
                columns: new[] { "DocumentId", "UserEmail" });
        }
    }
}
