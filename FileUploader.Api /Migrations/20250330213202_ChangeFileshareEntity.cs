using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileUploader.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFileshareEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedFiles_Files_FileId",
                table: "SharedFiles");

            migrationBuilder.DropIndex(
                name: "IX_SharedFiles_FileId",
                table: "SharedFiles");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "SharedFiles");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "SharedFiles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "SharedFiles");

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "SharedFiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SharedFiles_FileId",
                table: "SharedFiles",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFiles_Files_FileId",
                table: "SharedFiles",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
