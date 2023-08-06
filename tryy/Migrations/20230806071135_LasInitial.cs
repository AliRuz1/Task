using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tryy.Migrations
{
    /// <inheritdoc />
    public partial class LasInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NoteSpecial",
                table: "Products",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.UpdateData(
            //     table: "Products",
            //     keyColumn: "NoteSpecial",
            //     keyValue: null,
            //     column: "NoteSpecial",
            //     value: "");

            migrationBuilder.AlterColumn<string>(
                name: "NoteSpecial",
                table: "Products",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
