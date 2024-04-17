using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMQLSV.Migrations
{
    /// <inheritdoc />
    public partial class newDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Info",
                table: "grades");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "grades");

            migrationBuilder.RenameColumn(
                name: "MaxGrade",
                table: "grades",
                newName: "Score");

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "grades",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Cau1",
                table: "grades",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cau1",
                table: "grades");

            migrationBuilder.RenameColumn(
                name: "Score",
                table: "grades",
                newName: "MaxGrade");

            migrationBuilder.AlterColumn<int>(
                name: "Grade",
                table: "grades",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Info",
                table: "grades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "grades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
