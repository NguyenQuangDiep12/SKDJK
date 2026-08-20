using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SKDJK.Migrations
{
    /// <inheritdoc />
    public partial class AddDurationMinutesFieldInTestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "tests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "tests");
        }
    }
}
