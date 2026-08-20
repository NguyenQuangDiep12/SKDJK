using SKDJK.Models.enums;

namespace SKDJK.ViewModels
{
    // ViewModel trang Bài học của tôi chỉ hiển thị bài được học gần nhất.
    public sealed class MyLessonPageViewModel
    {
        public MyLessonItemViewModel? LatestLesson { get; set; }
        public bool HasLesson => LatestLesson is not null;
    }

    // ViewModel một dòng bài học dành riêng cho Razor View.
    public sealed class MyLessonItemViewModel
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public string LanguageName { get; set; } = string.Empty;
        public LearningStatus Status { get; set; }
        public decimal CompletionPercent { get; set; }
        public DateTime? LastStudyAt { get; set; }
        public bool IsCompleted => Status == LearningStatus.COMPLETED;
    }

    // ViewModel cha ghép bốn tab nội dung theo đúng wireframe bài học.
    public sealed class LessonStudyViewModel
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public decimal CompletionPercent { get; set; }
        public LearningStatus LearningStatus { get; set; }
        public int? PreviousLessonId { get; set; }
        public int? NextLessonId { get; set; }
        public VocabularyLearningViewModel Vocabulary { get; set; } = new();
        public GrammarLearningViewModel Grammar { get; set; } = new();
        public ListeningLearningViewModel Listening { get; set; } = new();
    }

    // Dữ liệu tab từ vựng và tab luyện nói đơn giản.
    public sealed class VocabularyLearningViewModel
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public int LessonTotal => Items.Count;
        public List<VocabularyItemViewModel> Items { get; set; } = [];
    }

    // Một thẻ từ vựng; audio sẽ được lấy khi bấm nút qua Free Dictionary.
    public sealed class VocabularyItemViewModel
    {
        public int VocabularyId { get; set; }
        public string Word { get; set; } = string.Empty;
        public string Meaning { get; set; } = string.Empty;
        public string? Pronunciation { get; set; }
        public string? Example { get; set; }
        public string? AudioUrl { get; set; }
    }

    // Dữ liệu hiển thị ngữ pháp dựa trên Lesson.Content.
    public sealed class GrammarLearningViewModel
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    // Dữ liệu luyện nghe được chiếu từ Question mà không lộ đáp án đúng.
    public sealed class ListeningLearningViewModel
    {
        public int LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public List<ListeningQuestionViewModel> Questions { get; set; } = [];
    }

    // Một câu luyện nghe an toàn cho client.
    public sealed class ListeningQuestionViewModel
    {
        public int QuestionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? AudioUrl { get; set; }
        public string? ImageUrl { get; set; }
        public int PartNumber { get; set; }
        public List<TestAnswerOptionViewModel> Answers { get; set; } = [];
    }
}
