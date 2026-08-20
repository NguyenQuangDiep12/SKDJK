using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SKDJK.Migrations
{
    /// <inheritdoc />
    public partial class CompleteCoreFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AnswerId",
                table: "useranswers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "topics",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "tests",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "ToeicStyle");

            migrationBuilder.AlterColumn<decimal>(
                name: "Score",
                table: "testresults",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,2)");

            migrationBuilder.AddColumn<string>(
                name: "ContextText",
                table: "questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupCode",
                table: "questions",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PartNumber",
                table: "questions",
                type: "int",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AddColumn<string>(
                name: "SectionName",
                table: "questions",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Reading");

            // Chuẩn hóa dữ liệu cũ sang tên enum mới và tạo metadata an toàn cho các câu đã tồn tại.
            migrationBuilder.Sql(
                """
                UPDATE [questions]
                SET [QuestionType] = CASE [QuestionType]
                    WHEN 'MULTIPLECHOICE' THEN 'MultipleChoice'
                    WHEN 'FILLINBLANK' THEN 'FillBlank'
                    WHEN 'LISTENING' THEN 'ListeningChoice'
                    ELSE [QuestionType]
                END;

                UPDATE [questions]
                SET [SectionName] = CASE
                        WHEN [QuestionType] IN ('ListeningChoice', 'ListeningFill') THEN 'Listening'
                        ELSE 'Reading'
                    END,
                    [PartNumber] = CASE
                        WHEN [QuestionType] IN ('ListeningChoice', 'ListeningFill') THEN 1
                        ELSE 5
                    END;

                ;WITH [OrderedQuestions] AS
                (
                    SELECT [Id], ROW_NUMBER() OVER (PARTITION BY [TestId] ORDER BY [Id]) AS [RowNumber]
                    FROM [questions]
                )
                UPDATE [questions]
                SET [Order] = [OrderedQuestions].[RowNumber]
                FROM [questions]
                INNER JOIN [OrderedQuestions] ON [questions].[Id] = [OrderedQuestions].[Id];
                """);

            migrationBuilder.InsertData(
                table: "languages",
                columns: new[] { "Id", "Code", "Description", "Name" },
                values: new object[,]
                {
                    { 10001, "en-demo", "Ngôn ngữ dùng cho dữ liệu demo TOEIC/IELTS-style.", "Tiếng Anh" },
                    { 10002, "ja-demo", "Ngôn ngữ mẫu bổ sung.", "Tiếng Nhật" }
                });

            migrationBuilder.InsertData(
                table: "topics",
                columns: new[] { "Id", "Description", "ImageUrl", "LanguageId", "Level", "Name" },
                values: new object[,]
                {
                    { 10001, "Từ vựng và kỹ năng giao tiếp trong các tình huống du lịch.", "/images/topic-travel.svg", 10001, "Trung cấp", "Giao tiếp và du lịch" },
                    { 10002, "Mẫu câu thường gặp trong học tập và công việc.", "/images/topic-work.svg", 10001, "Cơ bản", "Học tập và công việc" },
                    { 10003, "Các mẫu chào hỏi đơn giản.", "/images/topic-japanese.svg", 10002, "Cơ bản", "Chào hỏi tiếng Nhật" }
                });

            migrationBuilder.InsertData(
                table: "lessons",
                columns: new[] { "Id", "Content", "Description", "Title", "TopicId" },
                values: new object[,]
                {
                    { 10001, "Thì hiện tại đơn được dùng để nói về thói quen và sự thật. Cấu trúc: S + V(s/es). Ví dụ: She books a room online.", "Học từ vựng và mẫu câu đặt phòng.", "Đặt phòng khách sạn", 10001 },
                    { 10002, "Dùng Can you tell me...? hoặc How can I get to...? để hỏi đường một cách lịch sự.", "Hỏi đường, vị trí và phương tiện đi lại.", "Hỏi đường và phương tiện", 10001 },
                    { 10003, "Dùng hiện tại đơn để mô tả công việc thường ngày và lịch làm việc.", "Từ vựng văn phòng cơ bản.", "Giao tiếp tại nơi làm việc", 10002 },
                    { 10004, "こんにちは được dùng để chào hỏi vào ban ngày.", "Các lời chào thông dụng.", "Lời chào cơ bản", 10003 }
                });

            migrationBuilder.InsertData(
                table: "tests",
                columns: new[] { "Id", "Description", "DurationMinutes", "Format", "LessonId", "Title" },
                values: new object[,]
                {
                    { 10001, "Bài luyện tập nội bộ mô phỏng cách chia Part của TOEIC Listening & Reading.", 30, "ToeicStyle", 10001, "TOEIC-style Listening & Reading - Trung cấp" },
                    { 10002, "Bài luyện tập nội bộ mô phỏng Listening và Reading của IELTS.", 35, "IeltsStyle", 10003, "IELTS-style Listening & Reading - Cơ bản" }
                });

            migrationBuilder.InsertData(
                table: "vocabularies",
                columns: new[] { "Id", "AudioUrl", "Example", "LessonId", "Meaning", "Pronunciation", "Word" },
                values: new object[,]
                {
                    { 10001, null, "It was a long journey across the country.", 10001, "hành trình", "/ˈdʒɜːni/", "journey" },
                    { 10002, null, "I have a reservation for two nights.", 10001, "đặt chỗ", "/ˌrezəˈveɪʃn/", "reservation" },
                    { 10003, null, "Your luggage is beside the desk.", 10001, "hành lý", "/ˈlʌɡɪdʒ/", "luggage" },
                    { 10004, null, "The station is near the hotel.", 10002, "nhà ga", "/ˈsteɪʃn/", "station" },
                    { 10005, null, "Could you give me directions?", 10002, "phương hướng", "/dəˈrekʃn/", "direction" },
                    { 10006, null, "The meeting starts at nine.", 10003, "cuộc họp", "/ˈmiːtɪŋ/", "meeting" },
                    { 10007, null, "The deadline is Friday.", 10003, "hạn chót", "/ˈdedlaɪn/", "deadline" },
                    { 10008, null, "Hello, nice to meet you.", 10004, "xin chào", "/həˈləʊ/", "hello" }
                });

            migrationBuilder.InsertData(
                table: "questions",
                columns: new[] { "Id", "AudioUrl", "Content", "ContextText", "GroupCode", "ImageUrl", "Order", "PartNumber", "QuestionType", "SectionName", "TestId" },
                values: new object[,]
                {
                    { 10101, "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "What is the person doing?", null, "T-P1", "/images/toeic-part1.svg", 1, 1, "ListeningChoice", "Listening", 10001 },
                    { 10102, "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "Choose the best response to the recorded question.", null, "T-P2", null, 2, 2, "ListeningChoice", "Listening", 10001 },
                    { 10103, "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "Where will the speakers meet?", null, "T-P3", null, 3, 3, "ListeningChoice", "Listening", 10001 },
                    { 10104, "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "What is the announcement mainly about?", null, "T-P4", null, 4, 4, "ListeningChoice", "Listening", 10001 },
                    { 10105, null, "The manager _____ the report every Monday.", null, null, null, 5, 5, "MultipleChoice", "Reading", 10001 },
                    { 10106, null, "Complete the text: Please _____ the attached form.", "Thank you for your interest. Please _____ the attached form before Friday.", "T-P6", null, 6, 6, "FillBlank", "Reading", 10001 },
                    { 10107, null, "When does the hotel serve breakfast?", "Breakfast is served from 6:30 a.m. to 10:00 a.m. in the first-floor restaurant.", "T-P7", null, 7, 7, "MultipleChoice", "Reading", 10001 },
                    { 10201, "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "What type of room does the caller request?", null, "I-L1", null, 1, 1, "ListeningChoice", "Listening", 10002 },
                    { 10202, "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "The museum closes at ____ p.m.", null, "I-L2", null, 2, 2, "ListeningFill", "Listening", 10002 },
                    { 10203, "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "Which subject are the students discussing?", null, "I-L3", null, 3, 3, "ListeningChoice", "Listening", 10002 },
                    { 10204, "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "The lecture focuses on renewable ____.", null, "I-L4", null, 4, 4, "ListeningFill", "Listening", 10002 },
                    { 10205, null, "The library opens every Sunday.", "The city library opens Monday to Saturday. The notice gives no information about Sunday opening.", "I-R1", null, 5, 1, "TrueFalseNotGiven", "Reading", 10002 },
                    { 10206, null, "The writer believes remote work improves concentration.", "In my view, working from home can help people focus when they have a quiet workspace.", "I-R2", null, 6, 2, "YesNoNotGiven", "Reading", 10002 },
                    { 10207, null, "Complete the sentence: Solar panels convert sunlight into ____.", "Solar panels capture sunlight and convert it into electricity for homes and businesses.", "I-R3", null, 7, 3, "FillBlank", "Reading", 10002 }
                });

            migrationBuilder.InsertData(
                table: "answers",
                columns: new[] { "Id", "Content", "IsCorrect", "QuestionId" },
                values: new object[,]
                {
                    { 20101, "She is checking in at a hotel.", true, 10101 },
                    { 20102, "She is driving a bus.", false, 10101 },
                    { 20103, "She is cooking dinner.", false, 10101 },
                    { 20104, "At three o'clock.", true, 10102 },
                    { 20105, "Yes, I can see it.", false, 10102 },
                    { 20106, "For two people.", false, 10102 },
                    { 20107, "At the station.", true, 10103 },
                    { 20108, "Next Monday.", false, 10103 },
                    { 20109, "By email.", false, 10103 },
                    { 20110, "A change to the train schedule.", true, 10104 },
                    { 20111, "A restaurant opening.", false, 10104 },
                    { 20112, "A job interview.", false, 10104 },
                    { 20113, "reviews", true, 10105 },
                    { 20114, "review", false, 10105 },
                    { 20115, "reviewing", false, 10105 },
                    { 20116, "complete", true, 10106 },
                    { 20117, "fill in", true, 10106 },
                    { 20118, "From 6:30 a.m. to 10:00 a.m.", true, 10107 },
                    { 20119, "At noon.", false, 10107 },
                    { 20120, "Only on weekends.", false, 10107 },
                    { 20201, "A single room", true, 10201 },
                    { 20202, "A conference room", false, 10201 },
                    { 20203, "A family suite", false, 10201 },
                    { 20204, "five", true, 10202 },
                    { 20205, "5", true, 10202 },
                    { 20206, "Environmental science", true, 10203 },
                    { 20207, "Modern history", false, 10203 },
                    { 20208, "Business law", false, 10203 },
                    { 20209, "energy", true, 10204 },
                    { 20210, "TRUE", false, 10205 },
                    { 20211, "FALSE", false, 10205 },
                    { 20212, "NOT GIVEN", true, 10205 },
                    { 20213, "YES", true, 10206 },
                    { 20214, "NO", false, 10206 },
                    { 20215, "NOT GIVEN", false, 10206 },
                    { 20216, "electricity", true, 10207 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Đưa tên enum về dạng cũ trước khi quay lại model của migration trước.
            migrationBuilder.Sql(
                """
                UPDATE [questions]
                SET [QuestionType] = CASE [QuestionType]
                    WHEN 'MultipleChoice' THEN 'MULTIPLECHOICE'
                    WHEN 'FillBlank' THEN 'FILLINBLANK'
                    WHEN 'ListeningChoice' THEN 'LISTENING'
                    WHEN 'ListeningFill' THEN 'LISTENING'
                    WHEN 'TrueFalseNotGiven' THEN 'MULTIPLECHOICE'
                    WHEN 'YesNoNotGiven' THEN 'MULTIPLECHOICE'
                    ELSE [QuestionType]
                END;
                """);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20101);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20102);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20103);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20104);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20105);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20106);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20107);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20108);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20109);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20110);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20111);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20112);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20113);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20114);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20115);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20116);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20117);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20118);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20119);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20120);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20201);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20202);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20203);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20204);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20205);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20206);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20207);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20208);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20209);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20210);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20211);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20212);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20213);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20214);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20215);

            migrationBuilder.DeleteData(
                table: "answers",
                keyColumn: "Id",
                keyValue: 20216);

            migrationBuilder.DeleteData(
                table: "vocabularies",
                keyColumn: "Id",
                keyValue: 10001);

            migrationBuilder.DeleteData(
                table: "vocabularies",
                keyColumn: "Id",
                keyValue: 10002);

            migrationBuilder.DeleteData(
                table: "vocabularies",
                keyColumn: "Id",
                keyValue: 10003);

            migrationBuilder.DeleteData(
                table: "vocabularies",
                keyColumn: "Id",
                keyValue: 10004);

            migrationBuilder.DeleteData(
                table: "vocabularies",
                keyColumn: "Id",
                keyValue: 10005);

            migrationBuilder.DeleteData(
                table: "vocabularies",
                keyColumn: "Id",
                keyValue: 10006);

            migrationBuilder.DeleteData(
                table: "vocabularies",
                keyColumn: "Id",
                keyValue: 10007);

            migrationBuilder.DeleteData(
                table: "vocabularies",
                keyColumn: "Id",
                keyValue: 10008);

            migrationBuilder.DeleteData(
                table: "lessons",
                keyColumn: "Id",
                keyValue: 10002);

            migrationBuilder.DeleteData(
                table: "lessons",
                keyColumn: "Id",
                keyValue: 10004);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10101);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10102);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10103);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10104);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10105);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10106);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10107);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10201);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10202);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10203);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10204);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10205);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10206);

            migrationBuilder.DeleteData(
                table: "questions",
                keyColumn: "Id",
                keyValue: 10207);

            migrationBuilder.DeleteData(
                table: "tests",
                keyColumn: "Id",
                keyValue: 10001);

            migrationBuilder.DeleteData(
                table: "tests",
                keyColumn: "Id",
                keyValue: 10002);

            migrationBuilder.DeleteData(
                table: "topics",
                keyColumn: "Id",
                keyValue: 10003);

            migrationBuilder.DeleteData(
                table: "languages",
                keyColumn: "Id",
                keyValue: 10002);

            migrationBuilder.DeleteData(
                table: "lessons",
                keyColumn: "Id",
                keyValue: 10001);

            migrationBuilder.DeleteData(
                table: "lessons",
                keyColumn: "Id",
                keyValue: 10003);

            migrationBuilder.DeleteData(
                table: "topics",
                keyColumn: "Id",
                keyValue: 10001);

            migrationBuilder.DeleteData(
                table: "topics",
                keyColumn: "Id",
                keyValue: 10002);

            migrationBuilder.DeleteData(
                table: "languages",
                keyColumn: "Id",
                keyValue: 10001);

            migrationBuilder.DropColumn(
                name: "Format",
                table: "tests");

            migrationBuilder.DropColumn(
                name: "ContextText",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "GroupCode",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "PartNumber",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "SectionName",
                table: "questions");

            migrationBuilder.AlterColumn<int>(
                name: "AnswerId",
                table: "useranswers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "topics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Score",
                table: "testresults",
                type: "decimal(3,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);
        }
    }
}
