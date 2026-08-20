using SKDJK.Models.enums;

namespace SKDJK.Dtos
{
    // Chứa toàn bộ dữ liệu của trang học nhưng không truyền entity ra Razor View.
    public sealed class LessonStudyDto
    {
        public int LessonId { get; init; }
        public string LessonTitle { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string TopicName { get; init; } = string.Empty;
        public decimal CompletionPercent { get; init; }
        public LearningStatus LearningStatus { get; init; }
        public int? PreviousLessonId { get; init; }
        public int? NextLessonId { get; init; }
        public VocabularyLearningDto Vocabulary { get; init; } = new();
        public GrammarLearningDto Grammar { get; init; } = new();
        public ListeningLearningDto Listening { get; init; } = new();
    }

    // Chứa dữ liệu thẻ học từ vựng của một bài học.
    public sealed class VocabularyLearningDto
    {
        public int LessonId { get; init; }
        public string LessonTitle { get; init; } = string.Empty;
        public List<VocabularyItemDto> Vocabularies { get; init; } = [];
    }

    // Đại diện cho một từ, không chứa navigation property của EF Core.
    public sealed class VocabularyItemDto
    {
        public int VocabularyId { get; init; }
        public string Word { get; init; } = string.Empty;
        public string Meaning { get; init; } = string.Empty;
        public string? Pronunciation { get; init; }
        public string? Example { get; init; }
    }

    // Dùng nội dung Lesson.Content hiện có cho phần ngữ pháp, tránh tạo bảng thừa.
    public sealed class GrammarLearningDto
    {
        public int LessonId { get; init; }
        public string LessonTitle { get; init; } = string.Empty;
        public string Content { get; init; } = string.Empty;
    }

    // Lấy các câu Listening của bài kiểm tra thuộc Lesson để tái sử dụng dữ liệu hiện có.
    public sealed class ListeningLearningDto
    {
        public int LessonId { get; init; }
        public string LessonTitle { get; init; } = string.Empty;
        public List<ListeningQuestionDto> Questions { get; init; } = [];
    }

    // Không chứa IsCorrect để đáp án không bị lộ ở browser.
    public sealed class ListeningQuestionDto
    {
        public int QuestionId { get; init; }
        public string Content { get; init; } = string.Empty;
        public string? AudioUrl { get; init; }
        public string? ImageUrl { get; init; }
        public int PartNumber { get; init; }
        public List<ListeningAnswerDto> Answers { get; init; } = [];
    }

    // Dữ liệu lựa chọn an toàn cho phần luyện nghe.
    public sealed class ListeningAnswerDto
    {
        public int AnswerId { get; init; }
        public string Content { get; init; } = string.Empty;
    }
}
