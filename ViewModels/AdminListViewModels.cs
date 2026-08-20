using SKDJK.Models.enums;

namespace SKDJK.ViewModels
{
    // Danh sách chủ đề quản trị kèm các bộ lọc hiện tại.
    public sealed class AdminTopicListViewModel
    {
        public string? Search { get; set; }
        public int? LanguageId { get; set; }
        public string? Level { get; set; }
        public List<LanguageOptionViewModel> Languages { get; set; } = [];
        public List<TopicCardViewModel> Items { get; set; } = [];
    }

    // Màn hình hai cột bài học và từ vựng theo wireframe Admin.
    public sealed class AdminLessonPageViewModel
    {
        public int? SelectedLessonId { get; set; }
        public AdminLessonItemViewModel? SelectedLesson { get; set; }
        public List<AdminLessonItemViewModel> Lessons { get; set; } = [];
        public List<AdminVocabularyItemViewModel> Vocabularies { get; set; } = [];
    }

    // Một bài học trên bảng quản trị.
    public sealed class AdminLessonItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
    }

    // Một từ vựng trên bảng quản trị.
    public sealed class AdminVocabularyItemViewModel
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string Word { get; set; } = string.Empty;
        public string? Meaning { get; set; }
        public string? Pronunciation { get; set; }
    }

    // Danh sách bài kiểm tra quản trị, không truyền entity EF trực tiếp cho View.
    public sealed class AdminTestListViewModel
    {
        public string? Search { get; set; }
        public TestFormat? Format { get; set; }
        public TestMode? Mode { get; set; }
        public List<AdminTestItemViewModel> Items { get; set; } = [];
    }

    // Một dòng bài kiểm tra trên dashboard quản trị.
    public sealed class AdminTestItemViewModel
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

    // Danh sách câu hỏi thuộc một bài kiểm tra.
    public sealed class AdminQuestionListViewModel
    {
        public int TestId { get; set; }
        public string TestTitle { get; set; } = string.Empty;
        public TestFormat Format { get; set; }
        public TestMode Mode { get; set; }
        public List<AdminQuestionItemViewModel> Items { get; set; } = [];
    }

    // Một dòng câu hỏi trên bảng quản trị.
    public sealed class AdminQuestionItemViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int PartNumber { get; set; }
        public int Order { get; set; }
        public int AnswerCount { get; set; }
    }
}
