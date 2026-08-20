using SKDJK.Models.enums;

namespace SKDJK.Dtos
{
    // DTO danh sách Test và trạng thái bộ lọc.
    public sealed class TestListDto
    {
        public string? Search { get; set; }
        public string? Level { get; set; }
        public TestFormat? Format { get; set; }
        public TestMode? Mode { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public List<string> Levels { get; set; } = [];
        public List<TestListItemDto> Tests { get; set; } = [];
    }

    // DTO một Test trên danh sách người học.
    public sealed class TestListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string LessonTitle { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
        public int DurationMinutes { get; set; }
        public decimal? BestScore { get; set; }
        public int AttemptCount { get; set; }
        public TestFormat Format { get; set; }
        public TestMode Mode { get; set; }
        public bool IsActive { get; set; }
    }

    // DTO toàn bộ dữ liệu an toàn cần để Controller tạo ViewModel làm bài.
    public sealed class TakeTestDto
    {
        public int TestId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string LessonTitle { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public TestFormat Format { get; set; }
        public TestMode Mode { get; set; }
        public List<TestQuestionDto> Questions { get; set; } = [];
        public List<TestQuestionGroupDto> Groups { get; set; } = [];
    }

    // DTO group chứa media hoặc passage dùng chung.
    public sealed class TestQuestionGroupDto
    {
        public string Key { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public int PartNumber { get; set; }
        public string? GroupCode { get; set; }
        public string? ContextText { get; set; }
        public string? AudioUrl { get; set; }
        public string? ImageUrl { get; set; }
        public List<TestQuestionDto> Questions { get; set; } = [];
    }

    // DTO câu hỏi không chứa IsCorrect.
    public sealed class TestQuestionDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public string? ImageUrl { get; set; }
        public string? AudioUrl { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int PartNumber { get; set; }
        public int Order { get; set; }
        public string? ContextText { get; set; }
        public string? GroupCode { get; set; }
        public string? Instruction { get; set; }
        public int? MaxWords { get; set; }
        public List<TestAnswerOptionDto> Answers { get; set; } = [];
    }

    // DTO option an toàn không chứa IsCorrect.
    public sealed class TestAnswerOptionDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    // DTO nhận dữ liệu nộp bài từ Controller sau khi ViewModel đã qua ModelState.
    public sealed class SubmitTestDto
    {
        public int TestId { get; set; }
        public List<QuestionSubmissionDto> Answers { get; set; } = [];
    }

    // DTO một câu trả lời do người học gửi.
    public sealed class QuestionSubmissionDto
    {
        public int QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public string? TextAnswer { get; set; }
    }

    // DTO kết quả một lượt làm từ Service sang Controller.
    public sealed class TestResultDto
    {
        public int ResultId { get; set; }
        public int TestId { get; set; }
        public string TestTitle { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public int CorrectCount { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public TestFormat Format { get; set; }
        public TestMode Mode { get; set; }
        public decimal PassingScore { get; set; } = 70;
        public List<QuestionResultDto> Questions { get; set; } = [];
        public List<TestSectionResultDto> Sections { get; set; } = [];
    }

    // DTO thống kê đúng/tổng theo Section.
    public sealed class TestSectionResultDto
    {
        public string SectionName { get; set; } = string.Empty;
        public int CorrectCount { get; set; }
        public int TotalQuestions { get; set; }
    }

    // DTO chi tiết kết quả của một Question.
    public sealed class QuestionResultDto
    {
        public int QuestionId { get; set; }
        public int Number { get; set; }
        public string Content { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string? UserAnswer { get; set; }
        public string? CorrectAnswer { get; set; }
    }

    // DTO danh sách lịch sử Test.
    public sealed class TestHistoryDto
    {
        public List<TestHistoryItemDto> Items { get; set; } = [];
    }

    // DTO một lượt làm dùng chung cho lịch sử Test và trang Progress.
    public sealed class TestHistoryItemDto
    {
        public int ResultId { get; set; }
        public int TestId { get; set; }
        public string TestTitle { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public int CorrectCount { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public TestFormat Format { get; set; }
        public TestMode Mode { get; set; }
    }

    // DTO danh sách Test quản trị.
    public sealed class AdminTestListDto
    {
        public string? Search { get; set; }
        public TestFormat? Format { get; set; }
        public TestMode? Mode { get; set; }
        public List<AdminTestItemDto> Items { get; set; } = [];
    }

    // DTO một dòng Test quản trị.
    public sealed class AdminTestItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string LessonTitle { get; set; } = string.Empty;
        public TestFormat Format { get; set; }
        public TestMode Mode { get; set; }
        public bool IsActive { get; set; }
        public int DurationMinutes { get; set; }
        public int QuestionCount { get; set; }
    }

    // DTO form Test chuyển hai chiều giữa Controller và Service.
    public sealed class AdminTestFormDto
    {
        public int? Id { get; set; }
        public int LessonId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; } = 30;
        public TestFormat Format { get; set; } = TestFormat.ToeicStyle;
        public TestMode Mode { get; set; } = TestMode.Practice;
        public bool IsActive { get; set; } = true;
        public List<AdminLessonOptionDto> Lessons { get; set; } = [];
    }

    // DTO option Lesson cho form Test.
    public sealed class AdminLessonOptionDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }

    // DTO danh sách Question quản trị.
    public sealed class AdminQuestionListDto
    {
        public int TestId { get; set; }
        public string TestTitle { get; set; } = string.Empty;
        public TestFormat Format { get; set; }
        public TestMode Mode { get; set; }
        public List<AdminQuestionItemDto> Items { get; set; } = [];
    }

    // DTO một dòng Question quản trị.
    public sealed class AdminQuestionItemDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int PartNumber { get; set; }
        public int Order { get; set; }
        public int AnswerCount { get; set; }
    }

    // DTO form Question chuyển hai chiều giữa Controller và Service.
    public sealed class AdminQuestionFormDto
    {
        public int? Id { get; set; }
        public int TestId { get; set; }
        public string Content { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public string SectionName { get; set; } = "Reading";
        public int PartNumber { get; set; } = 1;
        public int Order { get; set; }
        public string? GroupCode { get; set; }
        public string? ContextText { get; set; }
        public string? ImageUrl { get; set; }
        public string? AudioUrl { get; set; }
        public string? Instruction { get; set; }
        public int? MaxWords { get; set; }
        public List<AdminAnswerInputDto> Answers { get; set; } = [];
    }

    // DTO option/đáp án admin gửi vào Service.
    public sealed class AdminAnswerInputDto
    {
        public string? Content { get; set; }
        public bool IsCorrect { get; set; }
    }
}
