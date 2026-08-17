using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SKDJK.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "English"),
                    Code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Level = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Language_Topic",
                        column: x => x.LanguageId,
                        principalTable: "languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Role",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopicId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topic_Lesson",
                        column: x => x.TopicId,
                        principalTable: "topics",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "learningprogresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "NOTSTARTED"),
                    CompletionPercent = table.Column<decimal>(type: "decimal(4,1)", precision: 4, scale: 1, nullable: false, defaultValue: 0m),
                    LastStudyAt = table.Column<DateTime>(type: "Datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_learningprogresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningProgress_Lesson",
                        column: x => x.LessonId,
                        principalTable: "lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LearningProgress_User",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Test_Lesson",
                        column: x => x.LessonId,
                        principalTable: "lessons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "vocabularies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    Word = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Meaning = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Pronunciation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Example = table.Column<string>(type: "text", nullable: true),
                    AudioUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vocabularies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vocabulary_Lesson",
                        column: x => x.LessonId,
                        principalTable: "lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    QuestionType = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    AudioUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_Test",
                        column: x => x.TestId,
                        principalTable: "tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "testresults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    CorrectCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SubmittedAt = table.Column<DateTime>(type: "Datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testresults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResult_Test",
                        column: x => x.TestId,
                        principalTable: "tests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TestResult_User",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pronunciationresults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    VocabularyId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pronunciationresults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PronunciationResult_User",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PronunciationResult_Vocabulary",
                        column: x => x.VocabularyId,
                        principalTable: "vocabularies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "answers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answer_Question",
                        column: x => x.QuestionId,
                        principalTable: "questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "Description", "RoleName" },
                values: new object[,]
                {
                    { 1, null, "ADMIN" },
                    { 2, null, "USER" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "Email", "FullName", "PasswordHash", "RoleId" },
                values: new object[,]
                {
                    { 1, "Bach1994@gmail.com", "Nguyen Van Bach", "$2a$11$6yy1n7ZOuNvwVXiN78nTleFamsf5fpkodEtxod38CaHo9JRnpmQ8q", 1 },
                    { 2, "nguyenquangdiepnx1@gmail.com", "Nguyen Quang Diep", "$2a$11$1S.ZcePBd3lvAcCg..i1beVY/fhIQD9QX4POW1zY.Xk1AhxT53RwW", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_answers_QuestionId",
                table: "answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_languages_Code",
                table: "languages",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_learningprogresses_LessonId",
                table: "learningprogresses",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_learningprogresses_UserId",
                table: "learningprogresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_lessons_TopicId",
                table: "lessons",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_pronunciationresults_UserId",
                table: "pronunciationresults",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_pronunciationresults_VocabularyId",
                table: "pronunciationresults",
                column: "VocabularyId");

            migrationBuilder.CreateIndex(
                name: "IX_questions_TestId",
                table: "questions",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_testresults_TestId",
                table: "testresults",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_testresults_UserId",
                table: "testresults",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tests_LessonId",
                table: "tests",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_topics_LanguageId",
                table: "topics",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_RoleId",
                table: "users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_vocabularies_LessonId",
                table: "vocabularies",
                column: "LessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "answers");

            migrationBuilder.DropTable(
                name: "learningprogresses");

            migrationBuilder.DropTable(
                name: "pronunciationresults");

            migrationBuilder.DropTable(
                name: "testresults");

            migrationBuilder.DropTable(
                name: "questions");

            migrationBuilder.DropTable(
                name: "vocabularies");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "tests");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "lessons");

            migrationBuilder.DropTable(
                name: "topics");

            migrationBuilder.DropTable(
                name: "languages");
        }
    }
}
