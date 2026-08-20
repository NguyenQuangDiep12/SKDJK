using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SKDJK.Migrations
{
    /// <inheritdoc />
    public partial class MakeTopicLevelUnicode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "topics",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            // Khôi phục dấu tiếng Việt cho các bản ghi demo đã từng đi qua cột varchar.
            migrationBuilder.Sql(
                """
                UPDATE [topics] SET [Level] = N'Trung cấp' WHERE [Id] = 10001;
                UPDATE [topics] SET [Level] = N'Cơ bản' WHERE [Id] IN (10002, 10003);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Dùng giá trị ASCII trước khi quay lại varchar để rollback không tạo ký tự '?'.
            migrationBuilder.Sql(
                """
                UPDATE [topics] SET [Level] = 'Intermediate' WHERE [Id] = 10001;
                UPDATE [topics] SET [Level] = 'Beginner' WHERE [Id] IN (10002, 10003);
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "topics",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
