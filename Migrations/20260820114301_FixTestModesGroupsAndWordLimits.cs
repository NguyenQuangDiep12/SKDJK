using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SKDJK.Migrations
{
    /// <inheritdoc />
    public partial class FixTestModesGroupsAndWordLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_questions_TestId",
                table: "questions");

            migrationBuilder.AlterColumn<string>(
                name: "TextAnswer",
                table: "useranswers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "tests",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "tests",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Mode",
                table: "tests",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Practice");

            migrationBuilder.AddColumn<int>(
                name: "TotalQuestions",
                table: "testresults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Chụp tổng số câu của các lượt làm cũ trước khi migration bổ sung câu seed mới.
            migrationBuilder.Sql("UPDATE tr SET TotalQuestions = (SELECT COUNT(*) FROM questions q WHERE q.TestId = tr.TestId) FROM testresults tr;");

            migrationBuilder.AddColumn<string>(
                name: "Instruction",
                table: "questions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxWords",
                table: "questions",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20117,
                columns: new[] { "Content", "IsCorrect" },
                values: new object[] { "completed", false });

            migrationBuilder.InsertData(
                table: "answers",
                columns: new[] { "Id", "Content", "IsCorrect", "QuestionId" },
                values: new object[] { 20121, "completion", false, 10106 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10101,
                columns: new[] { "GroupCode", "Instruction", "MaxWords" },
                values: new object[] { "TOEIC-P1-Q01", "Choose the best answer.", null });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10102,
                columns: new[] { "GroupCode", "Instruction", "MaxWords", "Order" },
                values: new object[] { "TOEIC-P2-Q01", "Choose the best answer.", null, 3 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10103,
                columns: new[] { "GroupCode", "Instruction", "MaxWords", "Order" },
                values: new object[] { "TOEIC-P3-G01", "Choose the best answer.", null, 5 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10104,
                columns: new[] { "GroupCode", "Instruction", "MaxWords", "Order" },
                values: new object[] { "TOEIC-P4-G01", "Choose the best answer.", null, 8 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10105,
                columns: new[] { "Instruction", "MaxWords", "Order" },
                values: new object[] { "Choose the best answer.", null, 11 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10106,
                columns: new[] { "Content", "ContextText", "GroupCode", "Instruction", "MaxWords", "Order", "QuestionType" },
                values: new object[] { "Choose the word that best completes blank 1.", "Thank you for your interest. Please complete the attached form before Friday. Our team will review it and reply next week.", "TOEIC-P6-G01", "Choose the best answer.", null, 14, "MultipleChoice" });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10107,
                columns: new[] { "ContextText", "GroupCode", "Instruction", "MaxWords", "Order" },
                values: new object[] { "Hotel notice: Breakfast is served from 6:30 a.m. to 10:00 a.m. The pool closes at 9:00 p.m. Guests can request a late checkout at reception.", "TOEIC-P7-G01", "Choose the best answer.", null, 18 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10201,
                columns: new[] { "GroupCode", "Instruction", "MaxWords", "Order", "PartNumber" },
                values: new object[] { "IELTS-L2-G01", null, null, 2, 2 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10202,
                columns: new[] { "GroupCode", "Instruction", "MaxWords", "Order", "PartNumber" },
                values: new object[] { "IELTS-L1-G01", "NO MORE THAN ONE WORD AND/OR A NUMBER.", 1, 1, 1 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10203,
                columns: new[] { "Content", "GroupCode", "Instruction", "MaxWords" },
                values: new object[] { "Which subject is assigned to Student A?", "IELTS-L3-G01", "Match each student with the correct subject.", null });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10204,
                columns: new[] { "GroupCode", "Instruction", "MaxWords", "Order" },
                values: new object[] { "IELTS-L4-G01", "NO MORE THAN ONE WORD.", 1, 5 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10205,
                columns: new[] { "ContextText", "GroupCode", "Instruction", "MaxWords", "Order", "PartNumber" },
                values: new object[] { "The city library opens Monday to Saturday. In the writer's view, remote access improves study flexibility. The notice gives no information about Sunday opening.", "IELTS-R2-G01", "Do the statements agree with the information in the passage?", null, 8, 2 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10206,
                columns: new[] { "Content", "ContextText", "GroupCode", "Instruction", "MaxWords", "Order" },
                values: new object[] { "The writer believes remote access improves study flexibility.", "The city library opens Monday to Saturday. In the writer's view, remote access improves study flexibility. The notice gives no information about Sunday opening.", "IELTS-R2-G01", "Do the statements agree with the views of the writer?", null, 9 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10207,
                columns: new[] { "Content", "ContextText", "GroupCode", "Instruction", "MaxWords", "Order" },
                values: new object[] { "Solar panels convert sunlight into ____.", "Solar panels capture sunlight and convert it into electricity. Paragraph A explains collection, while Paragraph B describes storage.", "IELTS-R3-G01", "NO MORE THAN ONE WORD.", 1, 10 });

            migrationBuilder.InsertData(
                table: "questions",
                columns: new[] { "Id", "AudioUrl", "Content", "ContextText", "GroupCode", "ImageUrl", "Instruction", "MaxWords", "Order", "PartNumber", "QuestionType", "SectionName", "TestId" },
                values: new object[,]
                {
                    { 10108, "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "Where is the suitcase?", null, "TOEIC-P1-Q02", "/images/toeic-part1.svg", "Choose the best answer.", null, 2, 1, "ListeningChoice", "Listening", 10001 },
                    { 10109, "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "Choose the best response about the reservation.", null, "TOEIC-P2-Q02", null, "Choose the best answer.", null, 4, 2, "ListeningChoice", "Listening", 10001 },
                    { 10110, "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "What time will they meet?", null, "TOEIC-P3-G01", null, "Choose the best answer.", null, 6, 3, "ListeningChoice", "Listening", 10001 },
                    { 10111, "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "What will the woman bring?", null, "TOEIC-P3-G01", null, "Choose the best answer.", null, 7, 3, "ListeningChoice", "Listening", 10001 },
                    { 10112, "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "When will the change begin?", null, "TOEIC-P4-G01", null, "Choose the best answer.", null, 9, 4, "ListeningChoice", "Listening", 10001 },
                    { 10113, "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "What are listeners asked to do?", null, "TOEIC-P4-G01", null, "Choose the best answer.", null, 10, 4, "ListeningChoice", "Listening", 10001 },
                    { 10114, null, "Guests should _____ their keys at reception.", null, null, null, "Choose the best answer.", null, 12, 5, "MultipleChoice", "Reading", 10001 },
                    { 10115, null, "The meeting was _____ until Friday.", null, null, null, "Choose the best answer.", null, 13, 5, "MultipleChoice", "Reading", 10001 },
                    { 10116, null, "Choose the word that best completes blank 2.", "Thank you for your interest. Please complete the attached form before Friday. Our team will review it and reply next week.", "TOEIC-P6-G01", null, "Choose the best answer.", null, 15, 6, "MultipleChoice", "Reading", 10001 },
                    { 10117, null, "When should the form be returned?", "Thank you for your interest. Please complete the attached form before Friday. Our team will review it and reply next week.", "TOEIC-P6-G01", null, "Choose the best answer.", null, 16, 6, "MultipleChoice", "Reading", 10001 },
                    { 10118, null, "What will the team do next?", "Thank you for your interest. Please complete the attached form before Friday. Our team will review it and reply next week.", "TOEIC-P6-G01", null, "Choose the best answer.", null, 17, 6, "MultipleChoice", "Reading", 10001 },
                    { 10119, null, "Where is breakfast served?", "Hotel notice: Breakfast is served from 6:30 a.m. to 10:00 a.m. The pool closes at 9:00 p.m. Guests can request a late checkout at reception.", "TOEIC-P7-G01", null, "Choose the best answer.", null, 19, 7, "MultipleChoice", "Reading", 10001 },
                    { 10120, null, "What closes at 9:00 p.m.?", "Hotel notice: Breakfast is served from 6:30 a.m. to 10:00 a.m. The pool closes at 9:00 p.m. Guests can request a late checkout at reception.", "TOEIC-P7-G01", null, "Choose the best answer.", null, 20, 7, "MultipleChoice", "Reading", 10001 },
                    { 10121, null, "Where can guests request late checkout?", "Hotel notice: Breakfast is served from 6:30 a.m. to 10:00 a.m. The pool closes at 9:00 p.m. Guests can request a late checkout at reception.", "TOEIC-P7-G01", null, "Choose the best answer.", null, 21, 7, "MultipleChoice", "Reading", 10001 },
                    { 10122, null, "What type of text is this?", "Hotel notice: Breakfast is served from 6:30 a.m. to 10:00 a.m. The pool closes at 9:00 p.m. Guests can request a late checkout at reception.", "TOEIC-P7-G01", null, "Choose the best answer.", null, 22, 7, "MultipleChoice", "Reading", 10001 },
                    { 10208, "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "Which subject is assigned to Student B?", null, "IELTS-L3-G01", null, "Match each student with the correct subject.", null, 4, 3, "ListeningChoice", "Listening", 10002 },
                    { 10209, null, "How long may a student reserve a quiet room?", "The campus library now offers quiet rooms and online booking. Students can reserve a room for up to two hours.", "IELTS-R1-G01", null, null, null, 6, 1, "MultipleChoice", "Reading", 10002 },
                    { 10210, null, "How are the rooms booked?", "The campus library now offers quiet rooms and online booking. Students can reserve a room for up to two hours.", "IELTS-R1-G01", null, null, null, 7, 1, "MultipleChoice", "Reading", 10002 },
                    { 10211, null, "Which paragraph describes energy storage?", "Solar panels capture sunlight and convert it into electricity. Paragraph A explains collection, while Paragraph B describes storage.", "IELTS-R3-G01", null, "Choose paragraph A or B.", null, 11, 3, "MultipleChoice", "Reading", 10002 }
                });

            migrationBuilder.UpdateData(
                table: "tests",
                keyColumn: "Id",
                keyValue: 10001,
                columns: new[] { "CreatedAt", "Description", "DurationMinutes", "IsActive", "Mode", "Title" },
                values: new object[] { new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Bài luyện tập nội bộ có đủ Part và group dùng chung audio/passage.", 40, true, "Practice", "TOEIC-style Listening & Reading Practice" });

            migrationBuilder.UpdateData(
                table: "tests",
                keyColumn: "Id",
                keyValue: 10002,
                columns: new[] { "CreatedAt", "Description", "DurationMinutes", "IsActive", "Mode", "Title" },
                values: new object[] { new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Bài luyện tập nội bộ có đủ Listening Part 1-4 và Reading Passage 1-3.", 40, true, "Practice", "IELTS Academic Listening & Reading Practice" });

            migrationBuilder.InsertData(
                table: "answers",
                columns: new[] { "Id", "Content", "IsCorrect", "QuestionId" },
                values: new object[,]
                {
                    { 20301, "Beside the reception desk", true, 10108 },
                    { 20302, "Inside a taxi", false, 10108 },
                    { 20303, "Under the bed", false, 10108 },
                    { 20304, "It is under your name.", true, 10109 },
                    { 20305, "At the bus stop.", false, 10109 },
                    { 20306, "Three copies, please.", false, 10109 },
                    { 20307, "At 9:00 a.m.", true, 10110 },
                    { 20308, "At noon", false, 10110 },
                    { 20309, "After dinner", false, 10110 },
                    { 20310, "The booking documents", true, 10111 },
                    { 20311, "A bicycle", false, 10111 },
                    { 20312, "A lunch menu", false, 10111 },
                    { 20313, "Next Monday", true, 10112 },
                    { 20314, "Last month", false, 10112 },
                    { 20315, "Tomorrow evening", false, 10112 },
                    { 20316, "Check the updated timetable", true, 10113 },
                    { 20317, "Buy a new ticket", false, 10113 },
                    { 20318, "Leave the station", false, 10113 },
                    { 20319, "return", true, 10114 },
                    { 20320, "returns", false, 10114 },
                    { 20321, "returned", false, 10114 },
                    { 20322, "postponed", true, 10115 },
                    { 20323, "postpone", false, 10115 },
                    { 20324, "postponing", false, 10115 },
                    { 20325, "review", true, 10116 },
                    { 20326, "reviewed", false, 10116 },
                    { 20327, "reviewing", false, 10116 },
                    { 20328, "Before Friday", true, 10117 },
                    { 20329, "Next month", false, 10117 },
                    { 20330, "After the meeting", false, 10117 },
                    { 20331, "Reply next week", true, 10118 },
                    { 20332, "Cancel the form", false, 10118 },
                    { 20333, "Call the hotel", false, 10118 },
                    { 20334, "In the first-floor restaurant", true, 10119 },
                    { 20335, "Beside the pool", false, 10119 },
                    { 20336, "At reception", false, 10119 },
                    { 20337, "The pool", true, 10120 },
                    { 20338, "The restaurant", false, 10120 },
                    { 20339, "The front desk", false, 10120 },
                    { 20340, "At reception", true, 10121 },
                    { 20341, "In the restaurant", false, 10121 },
                    { 20342, "Online only", false, 10121 },
                    { 20343, "A hotel notice", true, 10122 },
                    { 20344, "A train ticket", false, 10122 },
                    { 20345, "A job advertisement", false, 10122 },
                    { 20401, "Environmental science", false, 10208 },
                    { 20402, "Modern history", true, 10208 },
                    { 20403, "Business law", false, 10208 },
                    { 20404, "One hour", false, 10209 },
                    { 20405, "Two hours", true, 10209 },
                    { 20406, "A full day", false, 10209 },
                    { 20407, "Online", true, 10210 },
                    { 20408, "By telephone only", false, 10210 },
                    { 20409, "At the front desk only", false, 10210 },
                    { 20410, "Paragraph A", false, 10211 },
                    { 20411, "Paragraph B", true, 10211 },
                    { 20412, "Neither paragraph", false, 10211 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_questions_TestId_Order",
                table: "questions",
                columns: new[] { "TestId", "Order" },
                unique: true);

            // Chuẩn hóa dữ liệu cũ: câu text chỉ lưu TextAnswer, câu choice chỉ lưu AnswerId.
            migrationBuilder.Sql("UPDATE ua SET AnswerId = NULL FROM useranswers ua INNER JOIN questions q ON q.Id = ua.QuestionId WHERE q.QuestionType IN ('FillBlank', 'ListeningFill');");
            migrationBuilder.Sql("UPDATE ua SET TextAnswer = NULL FROM useranswers ua INNER JOIN questions q ON q.Id = ua.QuestionId WHERE q.QuestionType NOT IN ('FillBlank', 'ListeningFill');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_questions_TestId_Order",
                table: "questions");

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20121);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20301);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20302);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20303);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20304);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20305);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20306);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20307);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20308);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20309);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20310);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20311);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20312);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20313);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20314);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20315);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20316);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20317);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20318);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20319);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20320);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20321);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20322);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20323);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20324);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20325);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20326);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20327);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20328);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20329);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20330);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20331);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20332);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20333);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20334);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20335);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20336);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20337);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20338);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20339);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20340);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20341);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20342);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20343);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20344);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20345);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20401);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20402);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20403);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20404);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20405);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20406);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20407);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20408);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20409);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20410);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20411);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20412);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10108);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10109);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10110);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10111);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10112);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10113);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10114);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10115);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10116);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10117);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10118);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10119);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10120);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10121);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10122);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10208);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10209);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10210);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10211);

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "tests");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "tests");

            migrationBuilder.DropColumn(
                name: "Mode",
                table: "tests");

            migrationBuilder.DropColumn(
                name: "TotalQuestions",
                table: "testresults");

            migrationBuilder.DropColumn(
                name: "Instruction",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "MaxWords",
                table: "questions");

            migrationBuilder.AlterColumn<string>(
                name: "TextAnswer",
                table: "useranswers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20117,
                columns: new[] { "Content", "IsCorrect" },
                values: new object[] { "fill in", true });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10101,
                column: "GroupCode",
                value: "T-P1");

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10102,
                columns: new[] { "GroupCode", "Order" },
                values: new object[] { "T-P2", 2 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10103,
                columns: new[] { "GroupCode", "Order" },
                values: new object[] { "T-P3", 3 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10104,
                columns: new[] { "GroupCode", "Order" },
                values: new object[] { "T-P4", 4 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10105,
                column: "Order",
                value: 5);

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10106,
                columns: new[] { "Content", "ContextText", "GroupCode", "Order", "QuestionType" },
                values: new object[] { "Complete the text: Please _____ the attached form.", "Thank you for your interest. Please _____ the attached form before Friday.", "T-P6", 6, "FillBlank" });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10107,
                columns: new[] { "ContextText", "GroupCode", "Order" },
                values: new object[] { "Breakfast is served from 6:30 a.m. to 10:00 a.m. in the first-floor restaurant.", "T-P7", 7 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10201,
                columns: new[] { "GroupCode", "Order", "PartNumber" },
                values: new object[] { "I-L1", 1, 1 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10202,
                columns: new[] { "GroupCode", "Order", "PartNumber" },
                values: new object[] { "I-L2", 2, 2 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10203,
                columns: new[] { "Content", "GroupCode" },
                values: new object[] { "Which subject are the students discussing?", "I-L3" });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10204,
                columns: new[] { "GroupCode", "Order" },
                values: new object[] { "I-L4", 4 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10205,
                columns: new[] { "ContextText", "GroupCode", "Order", "PartNumber" },
                values: new object[] { "The city library opens Monday to Saturday. The notice gives no information about Sunday opening.", "I-R1", 5, 1 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10206,
                columns: new[] { "Content", "ContextText", "GroupCode", "Order" },
                values: new object[] { "The writer believes remote work improves concentration.", "In my view, working from home can help people focus when they have a quiet workspace.", "I-R2", 6 });

            migrationBuilder.UpdateData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10207,
                columns: new[] { "Content", "ContextText", "GroupCode", "Order" },
                values: new object[] { "Complete the sentence: Solar panels convert sunlight into ____.", "Solar panels capture sunlight and convert it into electricity for homes and businesses.", "I-R3", 7 });

            migrationBuilder.UpdateData(
                table: "tests",
                keyColumn: "Id",
                keyValue: 10001,
                columns: new[] { "Description", "DurationMinutes", "Title" },
                values: new object[] { "Bài luyện tập nội bộ mô phỏng cách chia Part của TOEIC Listening & Reading.", 30, "TOEIC-style Listening & Reading - Trung cấp" });

            migrationBuilder.UpdateData(
                table: "tests",
                keyColumn: "Id",
                keyValue: 10002,
                columns: new[] { "Description", "DurationMinutes", "Title" },
                values: new object[] { "Bài luyện tập nội bộ mô phỏng Listening và Reading của IELTS.", 35, "IELTS-style Listening & Reading - Cơ bản" });

            migrationBuilder.CreateIndex(
                name: "IX_questions_TestId",
                table: "questions",
                column: "TestId");
        }
    }
}
