using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SKDJK.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAnswerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "useranswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestResultId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    AnswerId = table.Column<int>(type: "int", nullable: false),
                    TextAnswer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_useranswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAnswer_Answer",
                        column: x => x.AnswerId,
                        principalTable: "answers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserAnswer_Question",
                        column: x => x.QuestionId,
                        principalTable: "questions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserAnswer_TestResult",
                        column: x => x.TestResultId,
                        principalTable: "testresults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_useranswers_AnswerId",
                table: "useranswers",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_useranswers_QuestionId",
                table: "useranswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_useranswers_TestResultId",
                table: "useranswers",
                column: "TestResultId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "useranswers");
        }
    }
}
